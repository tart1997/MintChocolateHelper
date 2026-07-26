namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class PsuedoDeath
{
    private static ILHook FakeDeathHook_origDie;

    [OnLoad]
    internal static void Load()
    {
        FakeDeathHook_origDie ??= new ILHook(typeof(Player).GetMethod("orig_Die", BindingFlags.Public | BindingFlags.Instance)!, SkipRemovePlayer);
        On.Celeste.Level.Reload += PanicRemovePlayerIfPlayerIsStillLoaded;
        On.Celeste.PlayerDeadBody.Update += MovePlayer;
    }

    [OnUnload]
    internal static void Unload()
    {
        FakeDeathHook_origDie?.Dispose();
        FakeDeathHook_origDie = null;
        On.Celeste.Level.Reload -= PanicRemovePlayerIfPlayerIsStillLoaded;
        On.Celeste.PlayerDeadBody.Update -= MovePlayer;
    }

    private static void SkipRemovePlayer(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_0056: ldloc.2
        // IL_0057: ldloc.0
        // IL_0058: stfld class Celeste.Player/'<>c__DisplayClass344_0' Celeste.Player/'<>c__DisplayClass344_1'::'CS$<>8__locals1'
        // IL_005d: ldarg.0
        // IL_005e: ldarg.0
        // IL_005f: ldfld class Celeste.SoundSource Celeste.Player::wallSlideSfx
        // IL_0064: callvirt instance void Celeste.Player::Stop(class Celeste.SoundSource)

        if (!cursor.TryGotoNextBestFit(MoveType.Before,
            static instr => instr.MatchLdloc2(),
            static instr => instr.MatchLdloc0(),
            static instr => instr.MatchStfld(typeof(Player).GetNestedType("<>c__DisplayClass344_1", BindingFlags.NonPublic)!.GetField("CS$<>8__locals1")!),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<Player>("wallSlideSfx"),
            static instr => instr.MatchCallvirt<Player>("Stop")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck!\n\n\n");
            return;
        }

        cursor.EmitDelegate(StorePlayerBullshit);

        // IL_01e5: ldarg.0
        // IL_01e6: call instance class Monocle.Scene Monocle.Entity::get_Scene()
        // IL_01eb: ldarg.0
        // IL_01ec: callvirt instance void Monocle.Scene::Remove(class Monocle.Entity)

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCall<Entity>("get_Scene"),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCallvirt<Scene>("Remove")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck!\n\n\n");
            return;
        }

        ILLabel dontRemovePlayer = cursor.DefineLabel();

        cursor.EmitDelegate(ShouldSkipRemovePlayer);
        cursor.EmitBrtrue(dontRemovePlayer);

        if (!cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCall<Entity>("get_Scene"),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCallvirt<Scene>("Remove")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck!\n\n\n");
            return;
        }

        cursor.MarkLabel(dontRemovePlayer);
        cursor.EmitDelegate(FakeKillPlayer);
    }

    private static void StorePlayerBullshit()
    {
        if (Utils.LevelIsNotSafe(out Level level)) return;

        Player player = level.Tracker.GetEntity<Player>();

        MintChocolateHelperModule.Session.DepthBeforePsuedoDeath = player.Depth;
        MintChocolateHelperModule.Session.WasCollidableBeforePsuedoDeath = player.Collidable;
        MintChocolateHelperModule.Session.WasVisibleBeforePsuedoDeath = player.Visible;
    }

    private static bool ShouldSkipRemovePlayer()
    {
        if (Utils.LevelIsNotSafe(out Level level)) return false;

        bool CDTriggerExists = false;
        bool CDTriggerFalseFlagSkip = false;
        foreach (CancelDeathTrigger CDTrigger in level.Tracker.GetEntities<CancelDeathTrigger>().Cast<CancelDeathTrigger>())
        {
            if (CDTrigger.Flag != "")
            {
                if (FrostHelperImports.IsImported && CDTrigger.IsValidExpression)
                {
                    if (!FrostHelperImports.GetBoolSessionExpressionValue(CDTrigger.FlagExpression, level.Session))
                    {
                        CDTriggerFalseFlagSkip = true;
                    }
                }
                else
                {
                    if (!level.Session.GetFlag(CDTrigger.Flag))
                    {
                        CDTriggerFalseFlagSkip = true;
                    }
                }
            }
            CDTriggerExists = true;
        }

        return (CDTriggerExists && !CDTriggerFalseFlagSkip) || MintChocolateHelperModule.Session.HasJesusRefill;
    }

    private static void FakeKillPlayer()
    {
        if (Utils.LevelIsNotSafe(out Level level) || !ShouldSkipRemovePlayer()) return;

        Player player = level.Tracker.GetEntity<Player>();
        player.StateMachine.state = 17;
        player.Collidable = false;
        player.Visible = false;
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = true;
    }

    private static void PanicRemovePlayerIfPlayerIsStillLoaded(On.Celeste.Level.orig_Reload orig, Level level)
    {
        Player player = level.Tracker.GetEntity<Player>();
        player?.RemoveSelf();

        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;
        MintChocolateHelperModule.Session.HasJesusRefill = false;
        MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn = false;

        MintChocolateHelperModule.Session.HasSpeedFlipRefill = false;
        MintChocolateHelperModule.Session.DontRenderSpeedFlipRefillIcon = false;

        MintChocolateHelperModule.Session.HasHeartBreakerDash = false;
        MintChocolateHelperModule.Session.HeartBreakerDashActive = false;

        orig(level);
    }

    private static void MovePlayer(On.Celeste.PlayerDeadBody.orig_Update orig, PlayerDeadBody playerDeadBody)
    {
        orig(playerDeadBody);
        if (Utils.LevelIsNotSafe(out Level level) || MintChocolateHelperModule.Session.PsuedoDeathTeleportingPlayer || !MintChocolateHelperModule.Session.HasJesusRefill) return;

        Player player = level.Tracker.GetEntity<Player>();
        player?.Position = playerDeadBody.Position;
    }
}