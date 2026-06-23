using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.MintChocolateHelper.Triggers;
[CustomEntity("MintChocolateHelper/CancelDeathTrigger")]
[Tracked]

public class CancelDeathTrigger : Trigger
{
    private readonly int Delay;
    private readonly bool UnregisterDeathInStats;

    public CancelDeathTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Delay = data.Int("delay");
        UnregisterDeathInStats = data.Bool("unregisterDeathInStats");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (MintChocolateHelperModule.Session.PlayerIsPsuedoDead)
        {
            Add(new Coroutine(Unkill(Delay)));
        }
    }

    private IEnumerator Unkill(int delay)
    {
        if (Scene is not Level) yield break;
        yield return delay / 60f;

        Level level = SceneAs<Level>();
        level.Wipe?.Cancel();

        Session session = level.Session;
        Player player = level.Tracker.GetEntity<Player>();


        foreach (PlayerDeadBody playerDeadBody in Scene.Tracker.GetEntitiesTrackIfNeeded<PlayerDeadBody>().Cast<PlayerDeadBody>())
        {
            if (playerDeadBody == null) break;

            playerDeadBody.hair.Entity = player;
            playerDeadBody.sprite.Entity = player;
            playerDeadBody.light.Entity = player;
            playerDeadBody.RemoveSelf();
        }

        if (UnregisterDeathInStats)
        {
            --session.Deaths;
            --session.DeathsInCurrentLevel;
            --SaveData.Instance.TotalDeaths;
            --SaveData.Instance.Areas_Safe[session.Area.ID].Modes[(int) session.Area.Mode].Deaths;
            Stats.Increment(Stat.DEATHS, -1);
            StatsForStadia.Increment(StadiaStat.DEATHS, -1);
        }

        player.Dead = false;
        player.Depth = MintChocolateHelperModule.Session.CDT_Depth;
        player.StateMachine.Locked = false;
        player.Active = MintChocolateHelperModule.Session.CDT_Active;
        player.Collidable = MintChocolateHelperModule.Session.CDT_Collidable;
        player.Visible = MintChocolateHelperModule.Session.CDT_Visible;
        if (Scene is not null) player.Scene = Scene;
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;

        Debug.Assert(level.Session.RespawnPoint != null);
        player.Position = level.Session.RespawnPoint.Value;
    }

    internal static void Load()
    {
        MintChocolateHelperModule.CDTriggerHook_origDie = new ILHook(typeof(Player).GetMethod("orig_Die", BindingFlags.Public | BindingFlags.Instance)!, SkipRemovePlayer);
        On.Celeste.Level.Reload += PanicRemovePlayerIfPlayerIsStillLoaded;
    }

    internal static void Unload()
    {
        MintChocolateHelperModule.CDTriggerHook_origDie?.Dispose();
        On.Celeste.Level.Reload -= PanicRemovePlayerIfPlayerIsStillLoaded;
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

        if (!cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchLdarg0(),
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
        if (Engine.Scene is not Level level) return;
        Player player = level.Tracker.GetEntity<Player>();

        MintChocolateHelperModule.Session.CDT_Depth = player.Depth;
        MintChocolateHelperModule.Session.CDT_Active = player.Active;
        MintChocolateHelperModule.Session.CDT_Collidable = player.Collidable;
        MintChocolateHelperModule.Session.CDT_Visible = player.Visible;
    }

    private static bool ShouldSkipRemovePlayer()
    {
        return Engine.Scene is Level && MintChocolateHelperModule.Session.CancelDeathTriggerExists;
    }

    private static void FakeKillPlayer()
    {
        if (Engine.Scene is not Level level || !MintChocolateHelperModule.Session.CancelDeathTriggerExists) return;
        Player player = level.Tracker.GetEntity<Player>();

        player.Active = false;
        player.Collidable = false;
        player.Visible = false;
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = true;
    }

    private static void PanicRemovePlayerIfPlayerIsStillLoaded(On.Celeste.Level.orig_Reload orig, Level level)
    {
        if (Engine.Scene is not Level) return;
        Player player = level.Tracker.GetEntity<Player>();

        player?.RemoveSelf();
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;
        orig(level);
    }
}