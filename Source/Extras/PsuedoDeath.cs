namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class PsuedoDeath
{
    private static ILHook FakeDeathHook_origDie;
    private static ILHook ModifyDashSpeedHook_DashCoroutine;

    [OnLoad]
    internal static void Load()
    {
        FakeDeathHook_origDie ??= new ILHook(typeof(Player).GetMethod("orig_Die", BindingFlags.Public | BindingFlags.Instance)!, SkipRemovePlayer);
        ModifyDashSpeedHook_DashCoroutine ??= new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance)!.GetStateMachineTarget()!, ModifyDashSpeed);
        On.Celeste.Level.Reload += PanicRemovePlayerIfPlayerIsStillLoaded;
        On.Celeste.PlayerDeadBody.Update += MovePlayer;
    }

    [OnUnload]
    internal static void Unload()
    {
        FakeDeathHook_origDie?.Dispose();
        FakeDeathHook_origDie = null;
        ModifyDashSpeedHook_DashCoroutine?.Dispose();
        ModifyDashSpeedHook_DashCoroutine = null;
        On.Celeste.Level.Reload -= PanicRemovePlayerIfPlayerIsStillLoaded;
        On.Celeste.PlayerDeadBody.Update -= MovePlayer;
    }

    private static void SkipRemovePlayer(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0056: ldloc.2
        IL_0057: ldloc.0
        IL_0058: stfld class Celeste.Player/'<>c__DisplayClass344_0' Celeste.Player/'<>c__DisplayClass344_1'::'CS$<>8__locals1'
        IL_005d: ldarg.0
        IL_005e: ldarg.0
        IL_005f: ldfld class Celeste.SoundSource Celeste.Player::wallSlideSfx
        IL_0064: callvirt instance void Celeste.Player::Stop(class Celeste.SoundSource)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdloc2(),
                static instr => instr.MatchLdloc0(),
                static instr => instr.MatchStfld(typeof(Player).GetNestedType("<>c__DisplayClass344_1", BindingFlags.NonPublic)!.GetField("CS$<>8__locals1")!),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Player>("wallSlideSfx"),
                static instr => instr.MatchCallvirt<Player>("Stop")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(StorePlayerBullshit);


        /*
        IL_0140: ldarg.0
        IL_0141: ldfld class Celeste.Leader Celeste.Player::Leader
        IL_0146: callvirt instance void Celeste.Leader::LoseFollowers()
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Player>("Leader"),
                static instr => instr.MatchCallvirt<Leader>("LoseFollowers")).LogHookOnFailure(il))
        {
            return;
        }

        ILLabel dontRemoveFollwers = cursor.DefineLabel();

        cursor.EmitDelegate(ShouldKeepFollowers);
        cursor.EmitBrtrue(dontRemoveFollwers);

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Player>("Leader"),
                static instr => instr.MatchCallvirt<Leader>("LoseFollowers")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.MarkLabel(dontRemoveFollwers);


        /*
        IL_01e5: ldarg.0
        IL_01e6: call instance class Monocle.Scene Monocle.Entity::get_Scene()
        IL_01eb: ldarg.0
        IL_01ec: callvirt instance void Monocle.Scene::Remove(class Monocle.Entity)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCall<Entity>("get_Scene"),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCallvirt<Scene>("Remove")).LogHookOnFailure(il))
        {
            return;
        }

        ILLabel dontRemovePlayer = cursor.DefineLabel();

        cursor.EmitDelegate(ShouldSkipRemovePlayer);
        cursor.EmitBrtrue(dontRemovePlayer);

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCall<Entity>("get_Scene"),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCallvirt<Scene>("Remove")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.MarkLabel(dontRemovePlayer);
        cursor.EmitDelegate(FakeKillPlayer);
    }

    private static void ModifyDashSpeed(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_00b9: ldloc.2
        IL_00ba: ldc.r4 240
        IL_00bf: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Multiply(valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32)
        IL_00c4: stloc.3
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdloc2(),
                static instr => instr.MatchLdcR4(240),
                static instr => instr.MatchCall<Vector2>("op_Multiply"),
                static instr => instr.MatchStloc3()).LogHookOnFailure(il))
        {
            return;
        }

        cursor.GotoNext(MoveType.After, static instr => instr.MatchLdcR4(240));
        cursor.EmitDelegate(EatAndReplace);


        /*
        IL_0111: ldloc.1
        IL_0112: ldloc.3
        IL_0113: stfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Celeste.Player::Speed
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdloc1(),
                static instr => instr.MatchLdloc3(),
                static instr => instr.MatchStfld<Player>("Speed")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.GotoNext(MoveType.After, static instr => instr.MatchLdloc3());
        cursor.EmitDelegate(SpeedXFix);
    }

    private static float EatAndReplace(float value)
    {
        Player player = SearchUtils.GetPlayer();

        if (MintChocolateHelperModule.Session.StoredSpeed.HasValue)
        {
            Vector2 val = MintChocolateHelperModule.Session.StoredSpeed.Value;
            Vector2 lastAim = player!.lastAim;
            if (player.OverrideDashDirection.HasValue)
            {
                lastAim = player.CorrectDashPrecision(player.OverrideDashDirection.Value);
            }

            return lastAim.X == 0 && MintChocolateHelperModule.Session.Redirectable ? val.Length() : value;
        }

        return value;
    }

    private static Vector2 SpeedXFix(Vector2 speed)
    {
        if (MintChocolateHelperModule.Session.StoredSpeed.HasValue)
        {
            Vector2 val = MintChocolateHelperModule.Session.StoredSpeed.Value;
            MintChocolateHelperModule.Session.StoredSpeed = null;

            if (Math.Abs(val.X) > Math.Abs(speed.X) && MintChocolateHelperModule.Session.Redirectable)
            {
                speed.X = Math.Abs(val.X) * Math.Sign(speed.X);
            }
            else if (Math.Sign(val.X) == Math.Sign(speed.X) && Math.Abs(val.X) > Math.Abs(speed.X))
            {
                speed.X = val.X;
            }
        }

        return speed;
    }

    private static void StorePlayerBullshit()
    {
        Player player = SearchUtils.GetPlayer();

        MintChocolateHelperModule.Session.DepthBeforePsuedoDeath = player!.Depth;
        MintChocolateHelperModule.Session.WasCollidableBeforePsuedoDeath = player.Collidable;
        MintChocolateHelperModule.Session.WasVisibleBeforePsuedoDeath = player.Visible;
        if (MintChocolateHelperModule.Session.StoreSpeed)
        {
            MintChocolateHelperModule.Session.StoredSpeed = player.Speed;
        }
    }

    private static bool ShouldKeepFollowers()
    {
        bool dontDetachGolden = SearchUtils.GetEntities<CancelDeathTrigger>()!.Any(t => t.KeepFollowers);
        return ShouldSkipRemovePlayer() && (MintChocolateHelperModule.Session.PsuedoDeadKeepFollowers || dontDetachGolden);
    }

    private static bool ShouldSkipRemovePlayer()
    {
        Level level = Utils.GetLevel();

        bool CDTriggerExists = false;
        bool CDTriggerFalseFlagSkip = false;
        foreach (CancelDeathTrigger CDTrigger in level.GetEntities<CancelDeathTrigger>()!)
        {
            if (CDTrigger.Flag != "")
            {
                if (FrostHelperImports.IsImported && CDTrigger.IsValidExpression)
                {
                    if (!FrostHelperImports.GetBoolSessionExpressionValue(CDTrigger.FlagExpression, level?.Session))
                    {
                        CDTriggerFalseFlagSkip = true;
                    }
                }
                else
                {
                    if (!level.GetFlag(CDTrigger.Flag))
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
        if (!ShouldSkipRemovePlayer()) return;

        Player player = SearchUtils.GetPlayer();
        player?.StateMachine.state = 17;
        player?.Collidable = false;
        player?.Visible = false;
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = true;
    }

    private static void PanicRemovePlayerIfPlayerIsStillLoaded(On.Celeste.Level.orig_Reload orig, Level level)
    {
        level.GetPlayer()?.RemoveSelf();

        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;
        MintChocolateHelperModule.Session.LastJesusRefill = null;
        MintChocolateHelperModule.Session.HasJesusRefill = false;
        MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn = false;
        MintChocolateHelperModule.Session.PsuedoDeadKeepFollowers = false;
        MintChocolateHelperModule.Session.StoreSpeed = false;
        MintChocolateHelperModule.Session.StoredSpeed = null;
        MintChocolateHelperModule.Session.TeleportToRefill = false;
        MintChocolateHelperModule.Session.Redirectable = false;

        MintChocolateHelperModule.Session.HasSpeedFlipRefill = false;
        MintChocolateHelperModule.Session.DontRenderSpeedFlipRefillIcon = false;

        MintChocolateHelperModule.Session.HasHeartBreakerDash = false;
        MintChocolateHelperModule.Session.HeartBreakerDashActive = false;

        orig(level);
    }

    private static void MovePlayer(On.Celeste.PlayerDeadBody.orig_Update orig, PlayerDeadBody playerDeadBody)
    {
        Player player = SearchUtils.GetPlayer();
        player?.Speed = Vector2.Zero;

        orig(playerDeadBody);
        if (MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer || !MintChocolateHelperModule.Session.HasJesusRefill || MintChocolateHelperModule.Session.TeleportToRefill) return;

        player?.Position = playerDeadBody.Position;
    }
}