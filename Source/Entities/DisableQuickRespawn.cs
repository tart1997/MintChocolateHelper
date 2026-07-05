namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/DisableQuickRespawn")]
[Tracked]
public class DisableQuickRespawn : Entity
{
    private readonly string DisableFlag;
    private readonly object DisableFlagExpression;
    private readonly bool IsValidExpression;

    public DisableQuickRespawn(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        DisableFlag = data.Attr("disableFlag");
        if (FrostHelperImports.IsImported && FrostHelperImports.TryCreateSessionExpression(DisableFlag, out DisableFlagExpression))
        {
            IsValidExpression = true;
        }
    }

    [OnLoad]
    internal static void Load()
    {
        IL.Celeste.PlayerDeadBody.Update += PlayerDeadBodyOnUpdate;
    }

    [OnUnload]
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

        if (!cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchLdsfld(typeof(Input), "MenuConfirm"),
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
        if (Engine.Scene is not Level level) return false;
        if (MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn) return true;
        if (!MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter.Exists) return false;
        if (MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter.DQRController.DisableFlag == "") return false;

        if (FrostHelperImports.IsImported && MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter.DQRController.IsValidExpression)
        {
            return FrostHelperImports.GetBoolSessionExpressionValue(MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter.DQRController.DisableFlagExpression, level.Session);
        }

        return level.Session.GetFlag(MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter.DQRController.DisableFlag);
    }
}