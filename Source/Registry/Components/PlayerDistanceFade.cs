namespace Celeste.Mod.MintChocolateHelper.Registry.Components;

public class PlayerDistanceFade : Component
{
    private readonly float InnerRadius;
    private readonly float OuterRadius;
    private readonly bool FadeOut;
    private readonly PlayerDistanceFadeRegistryHandler.DeathBehavior DeathBehavior;
    private readonly float DeathFadeSpeedMultiplier;

    private float DecalDistance;
    private Color OriginalColor;
    private Color CurrentColor;
    private float ColorPercentage;
    private float LastPercentage;

    private bool BeginFadeOut;
    private Color FadeoutOriginalColor;
    private Color FadeoutTargetColor;
    private float FadeoutTimer;

    public PlayerDistanceFade(float innerRadius, float outerRadius, bool fadeOut, PlayerDistanceFadeRegistryHandler.DeathBehavior deathBehavior, float deathFadeSpeedMultiplier) : base(true, true)
    {
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        FadeOut = fadeOut;
        DeathBehavior = deathBehavior;
        DeathFadeSpeedMultiplier = deathFadeSpeedMultiplier;
    }

    public override void EntityAwake()
    {
        base.EntityAwake();
        Decal decal = (Decal)Entity;

        BeginFadeOut = false;
        OriginalColor = decal.Color;
        FadeoutTimer = 0f;
    }

    public override void Update()
    {
        base.Update();
        Decal decal = (Decal)Entity;

        decal.Color = OriginalColor;
    }

    public override void Render()
    {
        base.Render();
        if (Utils.LevelIsNotSafe(out Level level)) return;

        Decal decal = (Decal)Entity;
        Player player = level.Tracker.GetEntity<Player>();

        CurrentColor = decal.Color;

        if (player != null)
        {
            DecalDistance = (float)Math.Sqrt(Math.Pow(player.X - decal.X, 2) + Math.Pow(player.Y - decal.Y, 2));
            ColorPercentage = Math.Clamp((OuterRadius - InnerRadius - (DecalDistance - InnerRadius)) / (OuterRadius - InnerRadius), 0, 1);
        }

        if (FadeOut)
        {
            Color fadedColor = DecalDistance < InnerRadius ? Color.Transparent : Color.Multiply(CurrentColor, 1 - LastPercentage);

            if (player == null && DeathBehavior == PlayerDistanceFadeRegistryHandler.DeathBehavior.fadeOut)
            {
                FadeoutOriginalColor = fadedColor;
                FadeoutTargetColor = CurrentColor;
                BeginFadeOut = true;
            }
            else
            {
                decal.Color = player switch {
                    null => DeathBehavior == PlayerDistanceFadeRegistryHandler.DeathBehavior.staySame ? fadedColor : CurrentColor,
                    _ => fadedColor
                };
            }
        }
        else
        {
            Color fadedColor = DecalDistance > OuterRadius ? Color.Transparent : Color.Multiply(CurrentColor, LastPercentage);

            if (player == null && DeathBehavior == PlayerDistanceFadeRegistryHandler.DeathBehavior.fadeOut)
            {
                FadeoutOriginalColor = fadedColor;
                FadeoutTargetColor = Color.Transparent;
                BeginFadeOut = true;
            }
            else
            {
                decal.Color = player switch {
                    null => DeathBehavior == PlayerDistanceFadeRegistryHandler.DeathBehavior.staySame ? fadedColor : Color.Transparent,
                    _ => fadedColor
                };
            }
        }

        if (BeginFadeOut)
        {
            Color result = new();

            if (FadeoutTimer < 1)
            {
                result = Color.Lerp(FadeoutOriginalColor, FadeoutTargetColor, FadeoutTimer);
                FadeoutTimer += Engine.DeltaTime * DeathFadeSpeedMultiplier;
            }

            decal.Color = result;
        }

        LastPercentage = ColorPercentage;
    }
}