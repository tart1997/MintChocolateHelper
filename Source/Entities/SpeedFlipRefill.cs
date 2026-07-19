namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/SpeedFlipRefill")]
[Tracked]
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

    private readonly bool oneUse;

    private float respawnTimer;
    private readonly bool disableAmbientEffects;
    private readonly bool disableCollectEffects;
    private readonly float respawnTime;

    private readonly float ExtraMultiplier;

    private class InvertJumpTrail : Component
    {
        private readonly Player Player;
        private readonly Vector2 Scale;
        private readonly Color Color;

        private float dashTrailTimer = 0.1f;

        public InvertJumpTrail(Player player, Vector2 scale, Color color) : base(true, true)
        {
            Player = player;
            Scale = scale;
            Color = color;
            TrailManager.Add(Player, Scale, Color);
        }

        public override void Update()
        {
            base.Update();

            if (dashTrailTimer > 0f && !Player.onGround && Player.StateMachine.state != Player.StClimb && Player.StateMachine.state != Player.StDash)
            {
                dashTrailTimer -= Engine.DeltaTime;

                if (dashTrailTimer <= 0f)
                {
                    TrailManager.Add(Player, Scale, Color);
                    dashTrailTimer = 0.1f;
                }
            }
            else
            {
                RemoveSelf();
            }
        }
    }

    public SpeedFlipRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        oneUse = data.Bool("oneUse");
        disableAmbientEffects = data.Bool("disableAmbientEffects");
        disableCollectEffects = data.Bool("disableCollectEffects");
        respawnTime = data.Float("respawnTime", 2.5f);
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
        if (Utils.LevelIsNotSafe(out Level level)) return;

        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn();
            }
        }
        else if (Scene.OnInterval(0.1f) && !disableAmbientEffects && Collidable)
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }

        UpdateY();

        light.Alpha = disableAmbientEffects ? 0f : Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = disableAmbientEffects ? 0f : light.Alpha * 0.8f;

        if (Scene.OnInterval(2f) && sprite.Visible)
        {
            flash.Play("flash", true);
            flash.Visible = true;
        }
    }

    private void Respawn()
    {
        if (Utils.LevelIsNotSafe(out Level level) || oneUse || Collidable) return;

        Collidable = true;
        sprite.Visible = true;
        outline.Visible = false;
        Depth = -100;
        wiggler.Start();
        if (disableCollectEffects) return;

        Audio.Play("event:/game/general/diamond_return", Position);
        level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
    }

    private void UpdateY()
    {
        flash.Y = sprite.Y = bloom.Y = sine.Value * 2f;
    }

    public override void Render()
    {
        if (sprite.Visible)
        {
            sprite.DrawOutline();
        }

        base.Render();
    }

    private void OnPlayer(Player player)
    {
        if (!MintChocolateHelperModule.Session.HasSpeedFlipRefill)
        {
            if (!disableCollectEffects)
            {
                Audio.Play("event:/game/general/diamond_touch", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }

            Collidable = false;
            Add(new Coroutine(RefillRoutine(player)));
            player.UseRefill(false);
            MintChocolateHelperModule.Session.HasSpeedFlipRefill = true;

            if (disableCollectEffects)
            {
                MintChocolateHelperModule.Session.DontRenderSpeedFlipRefillIcon = true;
            }

            respawnTimer = respawnTime;
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        if (Utils.LevelIsNotSafe(out Level level)) yield break;

        Celeste.Freeze(0.05f);
        yield return null;

        level.Shake();
        flash.Visible = false;
        sprite.Visible = false;
        if (!oneUse)
        {
            outline.Visible = true;
        }

        Depth = 8999;
        if (disableCollectEffects) yield break;
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
        On.Celeste.Player.NormalUpdate += SpeedFlipRefillJump;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Player.Die -= SpeedFlipRefillDie;
        On.Celeste.Player.NormalUpdate -= SpeedFlipRefillJump;
    }

    private static PlayerDeadBody SpeedFlipRefillDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (!MintChocolateHelperModule.Session.HasJesusRefill && !Utils.CheckEntityExistence<CancelDeathTrigger>())
        {
            MintChocolateHelperModule.Session.HasSpeedFlipRefill = false;
            MintChocolateHelperModule.Session.DontRenderSpeedFlipRefillIcon = false;
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    private enum DirectionBeforeInvert
    {
        Up,
        Down,
        None
    }

    private static int SpeedFlipRefillJump(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (Utils.LevelIsNotSafe(out Level level)) return orig(self);

        List<Entity> refills = level.Tracker.GetEntities<SpeedFlipRefill>();
        if (refills == null || refills.Count == 0 || refills[0] is not SpeedFlipRefill refill) return orig(self);

        if (Input.Jump.Pressed && MintChocolateHelperModule.Session.HasSpeedFlipRefill && !self.OnGround(self.Position, 4)
            && !self.onGround && !self.WallJumpCheck(3) && !self.WallJumpCheck(-3) && self.jumpGraceTimer <= 0f
            && self.varJumpTimer <= 0f && (self.StateMachine.state == Player.StNormal || self.StateMachine.State == Player.StDash))
        {
            DirectionBeforeInvert directionBeforeInvert = self.Speed.Y switch {
                0 => DirectionBeforeInvert.None,
                > 0 => DirectionBeforeInvert.Down,
                < 0 => DirectionBeforeInvert.Up,
                _ => DirectionBeforeInvert.None
            };

            self.Speed.Y = -self.Speed.Y * refill.ExtraMultiplier;
            Input.Jump.ConsumeBuffer();

            if (self.Get<InvertJumpTrail>() is { } invertJumpTrail)
            {
                invertJumpTrail.RemoveSelf();
            }

            Vector2 scale = new(Math.Abs(self.Sprite.Scale.X) * (float)self.Facing, self.Sprite.Scale.Y);
            switch (directionBeforeInvert)
            {
                case DirectionBeforeInvert.Down:
                    self.Add(new InvertJumpTrail(self, scale, Color.Blue));
                    break;
                case DirectionBeforeInvert.Up:
                    self.Add(new InvertJumpTrail(self, scale, Color.Red));
                    break;
                case DirectionBeforeInvert.None:
                    break;
            }

            MintChocolateHelperModule.Session.HasSpeedFlipRefill = false;
            MintChocolateHelperModule.Session.DontRenderSpeedFlipRefillIcon = false;

            if (refill.oneUse)
            {
                refill.RemoveSelf();
            }
        }

        return orig(self);
    }
}