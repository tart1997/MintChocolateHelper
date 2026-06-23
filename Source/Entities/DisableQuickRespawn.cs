using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.MintChocolateHelper.Entities;
[CustomEntity("MintChocolateHelper/DisableQuickRespawn")]
[Tracked]

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

        if (!cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchLdsfld(typeof(Input),"MenuConfirm"),
            static instr => instr.MatchCallvirt<VirtualButton>("get_Pressed"),
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
        return Engine.Scene is Level level
               && MintChocolateHelperModule.Session.DisableQuickRepawnControllerExists.Item1
               && level.Session.GetFlag(MintChocolateHelperModule.Session.DisableQuickRepawnControllerExists.Item2.disableFlag);
    }
}