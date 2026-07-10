namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/DisableQuickRespawn", "MintChocolateHelper/DisableQuickRespawnController")]
[Tracked]
public class DisableQuickRespawnController : Entity
{
    private readonly string DisableFlag;
    private readonly object DisableFlagExpression;
    private readonly bool IsValidExpression;

    public DisableQuickRespawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
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
        if (Utils.LevelIsNotSafe(out Level level)) return false;
        if (MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn) return true;
        if (!Utils.CheckEntityExistence(out DisableQuickRespawnController DQRController)) return false;
        if (DQRController.DisableFlag == "") return false;

        if (FrostHelperImports.IsImported && DQRController.IsValidExpression)
        {
            return FrostHelperImports.GetBoolSessionExpressionValue(DQRController.DisableFlagExpression, level.Session);
        }

        return level.Session.GetFlag(DQRController.DisableFlag);
    }
}