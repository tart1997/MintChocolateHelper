namespace Celeste.Mod.MintChocolateHelper.Extras;

[ConditionalEntity(nameof(JesusRefill), nameof(CancelDeathTrigger))]
public static class PseudoDeath
{
    internal const string PseudoDeathFlag = "MintChocolateHelper_PseudoDeath";

    private static ILHook FakeDeathHook_origDie;
    //private static ILHook ExtendGroundCheck_origUpdate;
    private static ILHook ModifyDashSpeed_DashCoroutine;

    [ConditionalOnLoad]
    internal static void Load()
    {
        Utils.LogDebug($"Loading {nameof(PseudoDeath)} Hooks...");
        IL.Celeste.Player.Die += SilenceEverestEventDie;
        FakeDeathHook_origDie ??= new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Die), BindingFlags.Public | BindingFlags.Instance)!, PseudoDie);
        //ExtendGroundCheck_origUpdate ??= new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Update), BindingFlags.Public | BindingFlags.Instance)!, ExtendGroundCheck);
        On.Celeste.Player.Update += Resurrection;
        IL.Celeste.Level.Update += EvenOnRetry;
        On.Celeste.Level.Reload += PanicReset;
        ModifyDashSpeed_DashCoroutine ??= new ILHook(typeof(Player).GetMethod(nameof(Player.DashCoroutine), BindingFlags.NonPublic | BindingFlags.Instance)!.GetStateMachineTarget()!, ModifyDashSpeed);
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.Player.Die -= SilenceEverestEventDie;
        FakeDeathHook_origDie?.Dispose();
        FakeDeathHook_origDie = null;
        //ExtendGroundCheck_origUpdate?.Dispose();
        //ExtendGroundCheck_origUpdate = null;
        On.Celeste.Player.Update -= Resurrection;
        IL.Celeste.Level.Update -= EvenOnRetry;
        On.Celeste.Level.Reload -= PanicReset;
        ModifyDashSpeed_DashCoroutine?.Dispose();
        ModifyDashSpeed_DashCoroutine = null;
    }

    private static bool PlayerCanPseudoDie() => MintChocolateHelperModule.Session.PlayerCanPseudoDie;
    private static bool PseudoDeadWithRefill() => MintChocolateHelperModule.Session.PlayerIsPseudoDead && MintChocolateHelperModule.Session.HasJesusRefill;
    private static bool PseudoDeadDontRegisterDeathInStats() => MintChocolateHelperModule.Session.PseudoDeadDontRegisterDeathInStats;
    private static bool PseudoDeadKeepFollowers() => MintChocolateHelperModule.Session.PseudoDeadKeepFollowers;
    private static bool PseudoDeadSkipEverestEventOnDie() => MintChocolateHelperModule.Session.PseudoDeadSkipEverestEventOnDie;
    private static bool NOTPseudoDeadAffectRetries() => !MintChocolateHelperModule.Session.PseudoDeadAffectRetries;


    private static void SilenceEverestEventDie(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0016: ldloc.1
        IL_0017: brfalse.s IL_0082
        */

        ILLabel Skip = null;

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdloc1(),
                instr => instr.MatchBrfalse(out Skip)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(PseudoDeadSkipEverestEventOnDie);
        cursor.EmitBrtrue(Skip);
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
        IL_0156: ldarg.0
        IL_0157: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::get_Zero()
        IL_015c: stfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Celeste.Player::Speed
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCall<Vector2>("get_Zero"),
                static instr => instr.MatchStfld<Player>("Speed")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(StoreSpeed);

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCall<Vector2>("get_Zero"),
                static instr => instr.MatchStfld<Player>("Speed")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(SetState);


        /*
        IL_016d: ldarg.0
        IL_016e: ldc.i4.0
        IL_016f: stfld bool Monocle.Entity::Collidable
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdcI4(0),
                static instr => instr.MatchStfld<Entity>("Collidable")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(SetVisible);


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

        cursor.EmitDelegate(PlayerCanPseudoDie);
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
        cursor.EmitDelegate(SetPseudoDie);
    }

    private static void StoreSpeed() => MintChocolateHelperModule.Session.LastJesusRefill?.StoredSpeed = SearchUtils.GetPlayer()?.Speed ?? Vector2.Zero;
    private static void SetState() => SearchUtils.GetPlayer()?.StateMachine.State = 17;
    private static void SetVisible() => SearchUtils.GetPlayer()?.Visible = false;

    private static void SetPseudoDie()
    {
        if (MintChocolateHelperModule.Session.PlayerCanPseudoDie)
        {
            MintChocolateHelperModule.Session.PlayerIsPseudoDead = true;
            Utils.SetFlag(PseudoDeathFlag);
        }
    }


    private static void Resurrection(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer
            || (MintChocolateHelperModule.Session.PlayerIsPseudoDead
                && MintChocolateHelperModule.Session.HasJesusRefill
                && (Input.DashPressed || Input.CrouchDashPressed)))
        {
            Level level = self.SceneAs<Level>();
            PlayerDeadBody playerDeadBody = level.GetEntity<PlayerDeadBody>(true);
            JesusRefill lastJesusRefill = MintChocolateHelperModule.Session.LastJesusRefill;
            level.Wipe?.Cancel();

            if (MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer)
            {
                if (level.Session.RespawnPoint is { }) self.Position = level.Session.RespawnPoint.Value;
            }
            else
            {
                if (MintChocolateHelperModule.Session.JesusRefillTeleportToRefill)
                {
                    self.Position = lastJesusRefill!.Center + new Vector2(0, (int)(self.Height / 2));
                }
                else
                {
                    Vector2 fallback = level.Session.RespawnPoint ?? Vector2.Zero;
                    self.Position = playerDeadBody?.Position.Round() ?? fallback;
                }
            }

            playerDeadBody?.hair.Entity = self;
            playerDeadBody?.sprite.Entity = self;
            playerDeadBody?.light.Entity = self;
            playerDeadBody?.RemoveSelf();
            if (MintChocolateHelperModule.Session.PseudoDeadAffectRetries) level.RetryPlayerCorpse = null;

            self.Dead = false;
            self.Depth = 0;
            self.Speed = MintChocolateHelperModule.Session.StoredSpeed;
            self.dashCooldownTimer = 0;
            self.StateMachine.Locked = false;
            self.StateMachine.State = 0;
            self.Collidable = true;
            self.Visible = true;
            self.Scene = Engine.Scene;
            if (lastJesusRefill is { }) self.UseRefill(false);

            self.Add(new Coroutine(FuckingBullshit(level, self)));
            if (lastJesusRefill?.oneUse ?? false) lastJesusRefill.RemoveSelf();
        }

        orig(self);
    }

    private static IEnumerator FuckingBullshit(Level level, Player player)
    {
        bool temp = MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer;
        MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer = false;

        yield return null;
        level.Wipe?.Cancel();
        player.Sprite.Scale.X = 1;

        MintChocolateHelperModule.Session.PlayerIsPseudoDead = false;
        level.SetFlag(PseudoDeathFlag, false);
        if (!temp) MintChocolateHelperModule.Session.LastJesusRefill = null;
    }

    // private static void ExtendGroundCheck(ILContext il)
    // {
    //     ILCursor cursor = new(il);
    //     
    //     
    //     /*
    //     IL_0266: ldarg.0
    //     IL_0267: ldarg.0
    //     IL_0268: ldfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Monocle.Entity::Position
    //     IL_026d: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::get_UnitY()
    //     IL_0272: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
    //     IL_0277: call instance !!0 Monocle.Entity::CollideFirst<class Celeste.Solid>(valuetype [FNA]Microsoft.Xna.Framework.Vector2)
    //     IL_027c: stloc.1
    //     */
    //     
    //     if (cursor.TryGotoNextBestFit(MoveType.Before,
    //             static instr => instr.MatchLdarg0(),
    //             static instr => instr.MatchLdarg0(),
    //             static instr => instr.MatchLdfld<Entity>("Position"),
    //             static instr => instr.MatchCall<Vector2>("get_UnitY"),
    //             static instr => instr.MatchCall<Vector2>("op_Addition"),
    //             static instr => instr.MatchCall<Entity>("CollideFirst"),
    //             static instr => instr.MatchStloc1()).LogHookOnFailure(il))
    //     {
    //         return;
    //     }
    //
    //     ILLabel dontExend1 = cursor.DefineLabel();
    //
    //     cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchCall<Vector2>("get_UnitY"));
    //     cursor.EmitLdarg0();
    //     cursor.EmitDelegate(ShouldExtendGroundCheck);
    //     cursor.EmitBrfalse(dontExend1);
    //     cursor.EmitLdcR4(3f);
    //     cursor.EmitDelegate<Func<Vector2, float, Vector2>>(Vector2.Multiply);
    //
    //     cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchCall<Vector2>("op_Addition"));
    //     cursor.MarkLabel(dontExend1);
    //     
    //     
    //     /*
    //     IL_0280: ldarg.0
    //     IL_0281: ldarg.0
    //     IL_0282: ldfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Monocle.Entity::Position
    //     IL_0287: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::get_UnitY()
    //     IL_028c: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
    //     IL_0291: call instance !!0 Monocle.Entity::CollideFirstOutside<class Celeste.JumpThru>(valuetype [FNA]Microsoft.Xna.Framework.Vector2)
    //     IL_0296: stloc.1
    //     */
    //     
    //     if (cursor.TryGotoNextBestFit(MoveType.Before,
    //             static instr => instr.MatchLdarg0(),
    //             static instr => instr.MatchLdarg0(),
    //             static instr => instr.MatchLdfld<Entity>("Position"),
    //             static instr => instr.MatchCall<Vector2>("get_UnitY"),
    //             static instr => instr.MatchCall<Vector2>("op_Addition"),
    //             static instr => instr.MatchCall<Entity>("CollideFirstOutside"),
    //             static instr => instr.MatchStloc1()).LogHookOnFailure(il))
    //     {
    //         return;
    //     }
    //     
    //     ILLabel dontExend2 = cursor.DefineLabel();
    //
    //     cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchCall<Vector2>("get_UnitY"));
    //     cursor.EmitLdarg0();
    //     cursor.EmitDelegate(ShouldExtendGroundCheck);
    //     cursor.EmitBrfalse(dontExend2);
    //     cursor.EmitLdcR4(3f);
    //     cursor.EmitDelegate<Func<Vector2, float, Vector2>>(Vector2.Multiply);
    //
    //     cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchCall<Vector2>("op_Addition"));
    //     cursor.MarkLabel(dontExend2);
    // }
    //
    // private static bool ShouldExtendGroundCheck(Player player)
    // {
    //     return idfk man nothing here works;
    // }

    private static void EvenOnRetry(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0378: ldarg.0
        IL_0379: ldfld class Celeste.PlayerDeadBody Celeste.Level::RetryPlayerCorpse
        IL_037e: brtrue.s IL_038b
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Level>("RetryPlayerCorpse"),
                instr => instr.MatchBrtrue(out _)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.Index--;

        ILLabel skip = cursor.DefineLabel();

        cursor.EmitBrfalse(skip);
        cursor.EmitDelegate(NOTPseudoDeadAffectRetries);


        /*
        IL_0380: ldarg.0
        IL_0381: call instance void Monocle.Scene::Update()
        */

        cursor.TryGotoNextBestFit(MoveType.Before,
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCall<Scene>("Update"));

        cursor.MarkLabel(skip);
    }

    private static void PanicReset(On.Celeste.Level.orig_Reload orig, Level self)
    {
        if (MintChocolateHelperModule.Session.LastJesusRefill?.SkipEverestEventOnDie ?? false)
            typeof(Everest.Events.Player).GetMethod("Die", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, [self.GetPlayer()]);
        MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer = false;
        JesusRefill.ResetRefill();
        SpeedFlipRefill.ResetRefill();
        HeartBreakerRefill.ResetRefill();
        self.GetPlayer()?.RemoveSelf();

        orig(self);
    }

    private static void ModifyDashSpeed(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_00c5: ldloc.1
        IL_00c6: ldflda valuetype [FNA]Microsoft.Xna.Framework.Vector2 Celeste.Player::beforeDashSpeed
        IL_00cb: ldfld float32 [FNA]Microsoft.Xna.Framework.Vector2::X
        IL_00d0: call int32 [mscorlib]System.Math::Sign(float32)
        IL_00d5: ldloc.3
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdloc1(),
                static instr => instr.MatchLdflda<Player>("beforeDashSpeed"),
                static instr => instr.MatchLdfld<Vector2>("X"),
                static instr => instr.MatchCall(typeof(Math), "Sign"),
                static instr => instr.MatchLdloc3()).LogHookOnFailure(il))
        {
            return;
        }

        ILLabel redirectable = cursor.DefineLabel();

        cursor.EmitDelegate(PseudoDeadWithRefill);
        cursor.EmitBrtrue(redirectable);


        /*
        IL_0107: ldfld float32 [FNA]Microsoft.Xna.Framework.Vector2::X
        IL_010c: stfld float32 [FNA]Microsoft.Xna.Framework.Vector2::X
        IL_0111: ldloc.1
        IL_0112: ldloc.3
        IL_0113: stfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Celeste.Player::Speed
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdfld<Vector2>("X"),
                static instr => instr.MatchStfld<Vector2>("X"),
                static instr => instr.MatchLdloc1(),
                static instr => instr.MatchLdloc3(),
                static instr => instr.MatchStfld<Player>("Speed")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.MarkLabel(redirectable);
        cursor.EmitLdloc3();
        cursor.EmitDelegate(Redirect);
    }

    private static void Redirect(Vector2 speed)
    {
        if (!PseudoDeadWithRefill()) return;
        Player player = SearchUtils.GetPlayer();

        if (!MintChocolateHelperModule.Session.JesusRefillStoreSpeed)
        {
            player?.Speed = speed.SafeNormalize(240f);
            return;
        }

        if (MintChocolateHelperModule.Session.JesusRefillRedirectable)
        {
            if (Math.Abs(player!.beforeDashSpeed.X) > Math.Abs(speed.X))
            {
                speed.X = Math.Abs(player.beforeDashSpeed.X) * Math.Sign(speed.X);
            }
            player.Speed = speed.SafeNormalize(Math.Max(240f, MintChocolateHelperModule.Session.StoredSpeed.Length()));
        }
        else
        {
            if (Math.Sign(player!.beforeDashSpeed.X) == Math.Sign(speed.X) && Math.Abs(player.beforeDashSpeed.X) > Math.Abs(speed.X))
            {
                speed.X = player.beforeDashSpeed.X;
            }
            player.Speed = speed;
        }
    }
}