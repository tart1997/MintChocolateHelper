namespace Celeste.Mod.MintChocolateHelper.Extras;

[ConditionalEntity(nameof(SpeedFlipRefill), nameof(SpeedFlipController))]
public static class SpeedFlip
{
    private class InvertJumpTrail : Component
    {
        private readonly Player Player;
        private readonly Vector2 Scale;
        private readonly Color Color;

        private float dashTrailTimer = 0.1f;

        public InvertJumpTrail(Player player, Vector2 scale, Color color) : base(true, true)
        {
            Player = player;
            Scale = GravityHelperImports.InvertIfPlayerInverted(scale);
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

    private enum DirectionBeforeInvert
    {
        Up,
        Down,
        None
    }

    internal static Hook DisableDash_CanDash;

    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogVerbose($"Loading {nameof(SpeedFlip)} Hooks...");
        Everest.Events.Player.OnAfterUpdate += GroundCheck;
        DisableDash_CanDash ??= new Hook(typeof(Player).GetProperty("CanDash", BindingFlags.Public | BindingFlags.Instance)!.GetMethod!, DisableDash);
        On.Celeste.Player.NormalUpdate += SpeedFlipRefillJump;
    }

    [OnUnload]
    internal static void Unload()
    {
        Everest.Events.Player.OnAfterUpdate -= GroundCheck;
        DisableDash_CanDash?.Dispose();
        DisableDash_CanDash = null;
        On.Celeste.Player.NormalUpdate -= SpeedFlipRefillJump;
    }

    private static void GroundCheck(Player player)
    {
        if (SearchUtils.IfNone<SpeedFlipController>()) return;
        if (player.onGround) MintChocolateHelperModule.Session.SpeedFlipControllerCharges = Math.Max(1, MintChocolateHelperModule.Session.SpeedFlipControllerCharges);
        player.Dashes = MintChocolateHelperModule.Session.SpeedFlipControllerCharges;
    }

    private static bool DisableDash(Func<Player, bool> orig, Player self) => (self.Dead || SearchUtils.IfNone<SpeedFlipController>()) && orig(self);

    private static int SpeedFlipRefillJump(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (SearchUtils.IfNone(out SpeedFlipController speedFlipController) && SearchUtils.IfNone<SpeedFlipRefill>()) return orig(self);

        if (Input.Jump.Pressed && (MintChocolateHelperModule.Session.HasSpeedFlipRefill || MintChocolateHelperModule.Session.SpeedFlipControllerCharges > 0)
                               && !self.OnGround(self.Position, 4) && !self.onGround && !self.WallJumpCheck(3) && !self.WallJumpCheck(-3) && self.jumpGraceTimer <= 0f
                               && self.varJumpTimer <= 0f && (self.StateMachine.state == Player.StNormal || self.StateMachine.State == Player.StDash))
        {
            SpeedFlipRefill lastSpeedFlipRefill = MintChocolateHelperModule.Session.LastSpeedFlipRefill;

            DirectionBeforeInvert directionBeforeInvert = self.Speed.Y switch {
                0 => DirectionBeforeInvert.None,
                > 0 => DirectionBeforeInvert.Down,
                < 0 => DirectionBeforeInvert.Up,
                _ => DirectionBeforeInvert.None
            };

            self.Speed.Y = speedFlipController is { } ? -self.Speed.Y * speedFlipController.ExtraMultiplier : -self.Speed.Y * (lastSpeedFlipRefill?.ExtraMultiplier ?? 1.03f);
            Input.Jump.ConsumeBuffer();

            if (self.Get<InvertJumpTrail>() is { } invertJumpTrail) invertJumpTrail.RemoveSelf();

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
                default:
                    throw new ImpossibleEnumException();
            }

            MintChocolateHelperModule.Session.LastSpeedFlipRefill = null;
            MintChocolateHelperModule.Session.SpeedFlipControllerCharges--;

            if (lastSpeedFlipRefill?.OneUse ?? false) lastSpeedFlipRefill.RemoveSelf();
        }

        return orig(self);
    }
}