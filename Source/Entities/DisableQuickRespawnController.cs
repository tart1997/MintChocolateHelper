namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[ConditionalEntity]
[CustomEntity("MintChocolateHelper/DisableQuickRespawn", "MintChocolateHelper/DisableQuickRespawnController")]
public class DisableQuickRespawnController : Entity
{
    private readonly string DisableFlag;
    private readonly object DisableFlagExpression;
    private readonly bool IsValidExpression;

    public DisableQuickRespawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        DisableFlag = data.Attr("disableFlag");

        if (FrostHelperImports.SafeTryCreateSessionExpression(DisableFlag, out DisableFlagExpression)) IsValidExpression = true;
    }

    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogVerbose($"Loading {nameof(DisableQuickRespawnController)} Hooks...");
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


        /*
        IL_0006: ldsfld class Monocle.VirtualButton Celeste.Input::MenuConfirm
        IL_000b: callvirt instance bool Monocle.VirtualButton::get_Pressed()
        IL_0010: brfalse.s IL_0020
        */

        ILLabel anythingYouWant = null;

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdsfld(typeof(Input), "MenuConfirm"),
                static instr => instr.MatchCallvirt<VirtualButton>("get_Pressed"),
                instr => instr.MatchBrfalse(out anythingYouWant)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(DeadBodyCheck);
        cursor.EmitBrtrue(anythingYouWant);
    }

    private static bool DeadBodyCheck() => MintChocolateHelperModule.Session.PseudoDeadDisableQuickRespawn || (SearchUtils.GetEntities<DisableQuickRespawnController>()?.Any(t =>
        string.IsNullOrWhiteSpace(t.DisableFlag)
        || (t.IsValidExpression ? FrostHelperImports.SafeGetBoolSessionExpressionValue(t.DisableFlagExpression, Utils.GetLevel()?.Session) : Utils.GetLevel().GetFlag(t.DisableFlag))) ?? false);
}