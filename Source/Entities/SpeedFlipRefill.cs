namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/SpeedFlipRefill")]
public class SpeedFlipRefill : Entity
{
    private readonly ParticleType P_Shatter;
    private readonly ParticleType P_Regen;
    private readonly ParticleType P_Glow;

    private readonly Sprite sprite;
    private readonly Sprite flash;
    private readonly Image outline;

    private readonly Wiggler wiggler;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;

    private readonly SineWave sine;

    private readonly float RespawnTime;
    private float RespawnTimer;

    private readonly bool DisableAmbientEffects;
    internal readonly bool DisableCollectEffects;
    internal readonly float ExtraMultiplier;
    internal readonly bool OneUse;

    public SpeedFlipRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        OneUse = data.Bool("oneUse");
        DisableAmbientEffects = data.Bool("disableAmbientEffects");
        DisableCollectEffects = data.Bool("disableCollectEffects");
        RespawnTime = data.Float("respawnTime", 2.5f);
        ExtraMultiplier = data.Float("extraMultiplier", 1.03f);

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        P_Shatter = new ParticleType(Refill.P_Shatter) {
            Color = Color.Red,
            Color2 = Color.Blue
        };

        P_Regen = new ParticleType(Refill.P_Regen) {
            Color = Color.MediumVioletRed,
            Color2 = Color.MediumBlue
        };

        P_Glow = new ParticleType(Refill.P_Glow) {
            Color = Color.IndianRed,
            Color2 = Color.DodgerBlue
        };

        Add(outline = new Image(GFX.Game["objects/MintChocolateHelper/Refills/SpeedFlipRefill/outline"]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/SpeedFlipRefill/idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle");
        sprite.CenterOrigin();

        Add(flash = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/SpeedFlipRefill/flash"));
        flash.Add("flash", "", 0.05f);
        flash.OnFinish = delegate {
            flash.Visible = false;
        };

        flash.CenterOrigin();

        Add(wiggler = Wiggler.Create(1f, 4f, v => {
            sprite.Scale = flash.Scale = Vector2.One * (1f + v * 0.2f);
        }));

        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.8f, 16f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));

        sine.Randomize();

        UpdateY();

        Depth = -100;
    }

    public override void Update()
    {
        base.Update();
        if (RespawnTimer > 0f)
        {
            RespawnTimer -= Engine.DeltaTime;
            if (RespawnTimer <= 0f) Respawn();
        }
        else if (Scene.OnInterval(0.1f) && !DisableAmbientEffects && Collidable)
        {
            SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }

        UpdateY();

        light.Alpha = DisableAmbientEffects ? 0f : Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = DisableAmbientEffects ? 0f : light.Alpha * 0.8f;

        if (Scene.OnInterval(2f) && sprite.Visible)
        {
            flash.Play("flash", true);
            flash.Visible = true;
        }
    }

    private void Respawn()
    {
        if (OneUse || Collidable) return;

        Collidable = true;
        sprite.Visible = true;
        outline.Visible = false;
        Depth = -100;
        wiggler.Start();
        if (DisableCollectEffects) return;

        Audio.Play("event:/game/general/diamond_return", Position);
        SceneAs<Level>().ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
    }

    private void UpdateY()
    {
        flash.Y = sprite.Y = bloom.Y = sine.Value * 2f;
    }

    public override void Render()
    {
        if (sprite.Visible) sprite.DrawOutline();
        base.Render();
    }

    private void OnPlayer(Player player)
    {
        if (SearchUtils.IfAny(out SpeedFlipController speedFlipController) || !MintChocolateHelperModule.Session.HasSpeedFlipRefill)
        {
            if (!DisableCollectEffects)
            {
                Audio.Play("event:/game/general/diamond_touch", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }

            Collidable = false;
            Add(new Coroutine(RefillRoutine(player)));
            player.UseRefill(false);
            MintChocolateHelperModule.Session.LastSpeedFlipRefill = this;
            if (speedFlipController is { }) MintChocolateHelperModule.Session.SpeedFlipControllerCharges++;

            RespawnTimer = RespawnTime;
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Level level = SceneAs<Level>();

        Celeste.Freeze(0.05f);
        yield return null;

        level.Shake();
        flash.Visible = false;
        sprite.Visible = false;
        if (!OneUse) outline.Visible = true;

        Depth = 8999;
        if (DisableCollectEffects) yield break;
        yield return 0.05f;

        float num = player.Speed.Angle();
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num - MathF.PI / 2f);
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num + MathF.PI / 2f);
        SlashFx.Burst(Position, num);
    }

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Player.Die += SpeedFlipRefillDie;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Player.Die -= SpeedFlipRefillDie;
    }

    internal static void ResetRefill() => MintChocolateHelperModule.Session.LastSpeedFlipRefill = null;

    private static PlayerDeadBody SpeedFlipRefillDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if (!MintChocolateHelperModule.Session.PlayerCanPseudoDie) ResetRefill();
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }
}