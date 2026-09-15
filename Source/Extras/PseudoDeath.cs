namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class PseudoDeath
{
    private static ILHook FakeDeathHook_origDie;
    private static ILHook ModifyDashSpeedHook_DashCoroutine;

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Player.Die += PlayerOnDie;
        FakeDeathHook_origDie ??= new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Die), BindingFlags.Public | BindingFlags.Instance)!, PseudoDie);
        On.Celeste.PlayerDeadBody.Update += MovePlayer;
        On.Celeste.Level.Reload += PanicRemovePlayerIfPlayerIsStillLoaded;
        
        ModifyDashSpeedHook_DashCoroutine ??= new ILHook(typeof(Player).GetMethod(nameof(Player.DashCoroutine), BindingFlags.NonPublic | BindingFlags.Instance)!.GetStateMachineTarget()!, ModifyDashSpeed);
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Player.Die -= PlayerOnDie;
        FakeDeathHook_origDie?.Dispose();
        FakeDeathHook_origDie = null;
        On.Celeste.PlayerDeadBody.Update -= MovePlayer;
        On.Celeste.Level.Reload -= PanicRemovePlayerIfPlayerIsStillLoaded;
        
        ModifyDashSpeedHook_DashCoroutine?.Dispose();
        ModifyDashSpeedHook_DashCoroutine = null;
    }

    private static bool PlayerShouldPseudoDie()
    {
        Level level = Utils.GetLevel();
        return MintChocolateHelperModule.Session.HasJesusRefill || (SearchUtils.GetEntities<CancelDeathTrigger>()
            ?.Any(t => !string.IsNullOrWhiteSpace(t.Flag) && (t.IsValidExpression ? !FrostHelperImports.SafeGetBoolSessionExpressionValue(t.FlagExpression, level?.Session) : !level.GetFlag(t.Flag))) ?? false);
    }

    private static bool PseudoDeadDontRegisterDeathInStats() => MintChocolateHelperModule.Session.PseudoDeadDontRegisterDeathInStats;
    private static bool PseudoDeadKeepFollowers() => MintChocolateHelperModule.Session.PseudoDeadKeepFollowers;

    private static PlayerDeadBody PlayerOnDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        // if (Input.CrouchDash.bufferCounter > 0 || Input.Dash.bufferCounter > 0)
        // {
        //     MintChocolateHelperModule.Session.JesusRefillBufferedTeleport = true;
        //
        //     return null;
        // }

        MintChocolateHelperModule.Session.DepthBeforePseudoDeath = self.Depth;
        MintChocolateHelperModule.Session.WasCollidableBeforePseudoDeath = self.Collidable;
        MintChocolateHelperModule.Session.WasVisibleBeforePseudoDeath = self.Visible;
        if (MintChocolateHelperModule.Session.JesusRefillStoreSpeed)
        {
            MintChocolateHelperModule.Session.StoredSpeed = self.Speed;
        }

        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    private static void PseudoDie(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0069: ldarg.3
        IL_006a: brfalse.s IL_00b6
        */

        ILLabel Skip = null;

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg3(),
                instr => instr.MatchBrfalse(out Skip)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(PseudoDeadDontRegisterDeathInStats);
        cursor.EmitBrtrue(Skip);


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

        cursor.EmitDelegate(PseudoDeadKeepFollowers);
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

        cursor.EmitDelegate(PlayerShouldPseudoDie);
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

    private static void FakeKillPlayer()
    {
        if (!PlayerShouldPseudoDie()) return;

        Player player = SearchUtils.GetPlayer();
        player?.StateMachine.state = 17;
        player?.Collidable = false;
        player?.Visible = false;
        MintChocolateHelperModule.Session.PlayerIsPseudoDead = true;
    }

    private static void MovePlayer(On.Celeste.PlayerDeadBody.orig_Update orig, PlayerDeadBody playerDeadBody)
    {
        Player player = SearchUtils.GetPlayer();
        player?.Speed = Vector2.Zero;

        orig(playerDeadBody);
        if (MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer || !MintChocolateHelperModule.Session.HasJesusRefill || MintChocolateHelperModule.Session.JesusRefillTeleportToRefill) return;

        player?.Position = playerDeadBody.Position;
    }

    internal static void BasicUnkill(Player player, PlayerDeadBody playerDeadBody)
    {
        playerDeadBody.hair.Entity = player;
        playerDeadBody.sprite.Entity = player;
        playerDeadBody.light.Entity = player;
        playerDeadBody.RemoveSelf();

        player.Dead = false;
        player.Depth = MintChocolateHelperModule.Session.DepthBeforePseudoDeath;
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        player.Collidable = MintChocolateHelperModule.Session.WasCollidableBeforePseudoDeath;
        player.Visible = MintChocolateHelperModule.Session.WasVisibleBeforePseudoDeath;
        if (playerDeadBody.Scene is { } scene) player.Scene = scene;
        MintChocolateHelperModule.Session.PlayerIsPseudoDead = false;
    }

    private static void PanicRemovePlayerIfPlayerIsStillLoaded(On.Celeste.Level.orig_Reload orig, Level level)
    {
        JesusRefill.ResetRefill();
        SpeedFlipRefill.ResetRefill();
        HeartBreakerRefill.ResetRefill();

        if (MintChocolateHelperModule.Session.PseudoDeadDontRegisterDeathInStats)
        {
            level.Session.Deaths++;
            level.Session.DeathsInCurrentLevel++;
            SaveData.Instance.AddDeath(level.Session.Area);
        }

        level.GetPlayer()?.RemoveSelf();

        orig(level);
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

            return lastAim.X == 0 && MintChocolateHelperModule.Session.JesusRefillRedirectable ? Math.Max(value, val.Length()) : value;
        }

        return value;
    }

    private static Vector2 SpeedXFix(Vector2 speed)
    {
        if (MintChocolateHelperModule.Session.StoredSpeed.HasValue)
        {
            Vector2 val = MintChocolateHelperModule.Session.StoredSpeed.Value;
            MintChocolateHelperModule.Session.StoredSpeed = null;

            if (MintChocolateHelperModule.Session.JesusRefillRedirectable)
            {
                if (Math.Abs(val.X) > Math.Abs(speed.X))
                {
                    speed.X = Math.Abs(val.X) * Math.Sign(speed.X);
                }
            }
            else
            {
                if (Math.Sign(val.X) == Math.Sign(speed.X) && Math.Abs(val.X) > Math.Abs(speed.X))
                {
                    speed.X = val.X;
                }
            }
        }

        return speed;
    }
}