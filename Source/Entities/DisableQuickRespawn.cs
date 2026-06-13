using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/DisableQuickRespawn")]

// ReSharper disable once ClassNeverInstantiated.Global
public class DisableQuickRespawn : Entity
{
    private readonly string disableFlag;
    
    public DisableQuickRespawn(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        disableFlag = data.Attr("disableFlag");
    }

    internal static void Load()
    {
        IL.Celeste.PlayerDeadBody.Update += PlayerDeadBodyOnUpdate;
    }
    
    internal static void Unload()
    {
        IL.Celeste.PlayerDeadBody.Update -= PlayerDeadBodyOnUpdate;
    }

    private static void PlayerDeadBodyOnUpdate(ILContext il)
    {
        ILCursor cursor = new(il);
        
        // IL_0006: ldsfld class Monocle.VirtualButton Celeste.Input::MenuConfirm
        // IL_000b: callvirt instance bool Monocle.VirtualButton::get_Pressed()
        // IL_0010: brfalse.s IL_0020

        ILLabel anythingYouWant = null;
        
        if (!cursor.TryGotoNext(MoveType.After, 
            instr => instr.MatchLdsfld(typeof(Input),"MenuConfirm"),
            instr => instr.MatchCallvirt<VirtualButton>("get_Pressed"),
            instr => instr.MatchBrfalse(out anythingYouWant)))
        {
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!"); 
            return;
        }

        cursor.EmitDelegate(DeadBodyCheck);
        cursor.EmitBrtrue(anythingYouWant);
    }

    private static bool DeadBodyCheck()
    {
        if (Engine.Scene is not Level level) return false;
            
        List<Entity> entities = level.Tracker.GetEntitiesTrackIfNeeded<DisableQuickRespawn>();

        if (entities == null || entities.Count == 0) return false;
        return entities[0] is DisableQuickRespawn controller && level.Session.GetFlag(controller.disableFlag);
    }
}
