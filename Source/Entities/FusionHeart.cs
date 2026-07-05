namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/FusionHeart")]
public class FusionHeart : Entity
{
    private readonly Wiggler ScaleWiggler;

    internal readonly string spriteColor;
    private readonly float bloomStr;
    private readonly bool hasLight;

    private readonly Sprite sprite;
    private readonly Sprite outline;
    private readonly ParticleType shineParticle;
    private readonly Wiggler moveWiggler;
    private Vector2 moveWiggleDir;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private float timer;
    private bool collected;
    private readonly bool autoPulse;
    private float bounceSfxDelay;
    private float respawnTimer;

    private static readonly ParticleType P_Regen = new(Seeker.P_Regen);

    private Vector2 heartBreakerBonusSpeed;

    public FusionHeart(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        spriteColor = data.Attr("color", "ff4fed");
        bloomStr = data.Float("bloom", 0.75f);
        hasLight = data.Bool("light", true);
        autoPulse = true;

        heartBreakerBonusSpeed = Vector2.Zero;

        Add(new MirrorReflection());

        Add(sprite = GFX.SpriteBank.Create("FusionHeart"));
        Add(outline = GFX.SpriteBank.Create("FusionHeartOutline"));

        sprite.SetColor(Calc.HexToColor(spriteColor));
        sprite.Play("spin");

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        Add(ScaleWiggler = Wiggler.Create(0.5f, 4f, delegate(float f) {
            sprite.Scale = Vector2.One * (1f + f * 0.25f);
        }));

        Add(bloom = new BloomPoint(bloomStr, 16f));

        Color value = Calc.HexToColor(spriteColor);
        shineParticle = new ParticleType(HeartGem.P_BlueShine) {
            Color = value
        };

        value = Color.Lerp(value, Color.White, 0.5f);
        Add(light = new VertexLight(value, hasLight ? 1f : 0f, 32, 64));
        moveWiggler = Wiggler.Create(0.8f, 2f);
        moveWiggler.StartZero = true;
        Add(moveWiggler);
    }

    public FusionHeart(Vector2 position, Vector2 fullHeartBreakerBonusSpeed, string fullColor, float fullBloomStr, bool fullHasLight, bool fullAutoPulse) : base(position)
    {
        spriteColor = fullColor;
        bloomStr = fullBloomStr;
        hasLight = fullHasLight;
        autoPulse = fullAutoPulse;

        heartBreakerBonusSpeed = fullHeartBreakerBonusSpeed;

        Add(new MirrorReflection());

        Add(sprite = GFX.SpriteBank.Create("FusionHeart"));
        Add(outline = GFX.SpriteBank.Create("FusionHeartOutline"));

        sprite.SetColor(Calc.HexToColor(spriteColor));
        sprite.Play("spin");

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        Add(ScaleWiggler = Wiggler.Create(0.5f, 4f, delegate(float f) {
            sprite.Scale = Vector2.One * (1f + f * 0.25f);
        }));

        Add(bloom = new BloomPoint(bloomStr, 16f));

        Color value = Calc.HexToColor(spriteColor);
        shineParticle = new ParticleType(HeartGem.P_BlueShine) {
            Color = value
        };

        value = Color.Lerp(value, Color.White, 0.5f);
        Add(light = new VertexLight(value, hasLight ? 1f : 0f, 32, 64));
        moveWiggler = Wiggler.Create(0.8f, 2f);
        moveWiggler.StartZero = true;
        Add(moveWiggler);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (scene is not Level) return;

        sprite.OnLoop = delegate(string anim) {
            if (Visible && anim == "spin" && autoPulse)
            {
                Audio.Play("event:/game/general/crystalheart_pulse", Position);
                ScaleWiggler.Start();
                ((Level)Scene).Displacement.AddBurst(Position, 0.35f, 8f, 48f, 0.25f);
            }
        };
    }

    public override void Update()
    {
        bounceSfxDelay -= Engine.DeltaTime;
        timer += Engine.DeltaTime;

        if (collected && respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
        }

        if (collected && respawnTimer <= 0f)
        {
            respawnTimer = 0f;
            collected = false;
            Collidable = true;
            Visible = true;
            Audio.Play("event:/game/general/diamond_return", Position);
            bloom.Alpha = bloomStr;
            light.Alpha = hasLight ? 1f : 0f;
            ScaleWiggler.Start();
        }

        base.Update();

        sprite.Position = Vector2.UnitY * (float)Math.Sin(timer * 2f) * 2f + moveWiggleDir * moveWiggler.Value * -8f;

        List<Sprite> sprites = [outline];

        foreach (Sprite other in sprites)
        {
            other.Position = sprite.Position;
            other.Scale = sprite.Scale;
            if (other.CurrentAnimationID != sprite.CurrentAnimationID)
            {
                other.Play(sprite.CurrentAnimationID);
            }
            other.SetAnimationFrame(sprite.CurrentAnimationFrame);
        }

        if (Visible && Scene.OnInterval(0.1f))
        {
            SceneAs<Level>().Particles.Emit(shineParticle, 1, Center, Vector2.One * 8f);
        }
    }

    private void OnPlayer(Player player)
    {
        if (Scene is not Level level || collected || level.Frozen) return;

        if (bounceSfxDelay <= 0f)
        {
            Audio.Play("event:/game/general/crystalheart_bounce", Position);
            bounceSfxDelay = 0.1f;
        }

        if (player.DashAttacking && MintChocolateHelperModule.Session.HeartBreakerDashActive)
        {
            MintChocolateHelperModule.Session.HasHeartBreakerDash = false;
            MintChocolateHelperModule.Session.HeartBreakerDashActive = false;

            P_Regen.Color = Calc.HexToColor(spriteColor);
            P_Regen.Color2 = Color.White;

            player.ExplodeLaunch(Center, false, true);

            Add(new Coroutine(FullDashHitColliderDisableTimer()));

            Audio.Play("event:/MintChocolateHelper/heart_break", Center);
            Audio.Play("event:/new_content/game/10_farewell/puffer_splode", Center);

            level.Shake();
            level.Displacement.AddBurst(Center, 0.4f, 12f, 36f, 0.5f);
            level.Displacement.AddBurst(Center, 0.4f, 24f, 48f, 0.5f);
            level.Displacement.AddBurst(Center, 0.4f, 36f, 60f, 0.5f);
            for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
            {
                Vector2 position = Center + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
                level.Particles.Emit(P_Regen, position, num);
            }

            RemoveSelf();

            player.dashAttackTimer = 0;

            FusionHeartHalf leftHalf = new(Center - Vector2.UnitX * Width / 4, -(Vector2.UnitX * 2) - Vector2.UnitX * (heartBreakerBonusSpeed.Length() / 2), "9a9ddb", 0.75f, 1f, 1f, 1f, 1f, true, false);

            FusionHeartHalf rightHalf = new(Center + Vector2.UnitX * Width / 4, Vector2.UnitX * 2 + Vector2.UnitX * (heartBreakerBonusSpeed.Length() / 2), "9a9ddb", 0.75f, 1f, 1f, 1f, 1f, true, true);

            Scene.Add(leftHalf);
            Scene.Add(rightHalf);

            leftHalf.Add(new Coroutine(leftHalf.HalfDashHitColliderDisableTimer()));
            rightHalf.Add(new Coroutine(rightHalf.HalfDashHitColliderDisableTimer()));

            return;
        }

        int dashes = Math.Max(player.Dashes, 2);
        player.PointBounce(Center);
        player.Dashes = dashes;
        player.StateMachine.state = 0;
        moveWiggler.Start();
        ScaleWiggler.Start();
        moveWiggleDir = (Center - player.Center).SafeNormalize(Vector2.UnitY);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
    }

    internal IEnumerator FullDashHitColliderDisableTimer()
    {
        if (Scene is not Level) yield break;

        Collidable = false;
        yield return 5 / 60f;

        Collidable = true;
    }
}