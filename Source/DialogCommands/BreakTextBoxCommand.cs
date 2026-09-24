namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

[CustomCommand]
public static class BreakTextBoxCommand
{
    private static readonly (string, McTrigger)[] CommandStrings = [
        ("vvv", new BreakTextBoxTrigger(false)),
        ("VVV", new BreakTextBoxTrigger(true))
    ];

    private class BreakTextBoxTrigger : McTrigger
    {
        internal BreakTextBoxTrigger(bool disableTextShrink, params string[] args) : base(args)
        {
            ParseCommandAction = (data, _) => data.SetTrue("MintChocolateHelper:DisableLineLimit");
            ParseNewPageAction = (data, _) => data.SetFalse("MintChocolateHelper:DisableLineLimit");
            WhileOnPageAction = (data, _) => {
                data.SetTrue("MintChocolateHelper:JustifyTextDownwards");
                data.Set("MintChocolateHelper:DisableTextShrink", disableTextShrink);
            };
        }
    }

    [OnLoad]
    internal static void RegisterCommands()
    {
        Utils.LogVerbose($"Registering Custom Dialog Command: {nameof(BreakTextBoxTrigger)}...");
        CommandStrings.Register();
    }

    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogVerbose($"Loading {nameof(BreakTextBoxTrigger)} Hooks...");
        IL.Celeste.FancyText.AddNewLine += SkipAddNewPage;
        IL.Celeste.Textbox.Render += JustifyTextDownHook;
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.FancyText.AddNewLine -= SkipAddNewPage;
        IL.Celeste.Textbox.Render -= JustifyTextDownHook;
    }

    private static void SkipAddNewPage(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0032: ldarg.0
        IL_0033: ldfld int32 Celeste.FancyText::currentLine
        IL_0038: ldarg.0
        IL_0039: ldfld int32 Celeste.FancyText::linesPerPage
        IL_003e: ble.s IL_007e
        */

        ILLabel jumpToNewline = null;

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<FancyText>("currentLine"),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<FancyText>("linesPerPage"),
                instr => instr.MatchBle(out jumpToNewline)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitLdarg0();
        cursor.EmitDelegate(ShouldSkipAddNewPage);
        cursor.EmitBrtrue(jumpToNewline);
    }

    private static bool ShouldSkipAddNewPage(FancyText text) => DynamicData.For(text).TryGetTrue("MintChocolateHelper:DisableLineLimit");

    private static void JustifyTextDownHook(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_04f0: ldarg.0
        IL_04f1: ldfld class Celeste.FancyText/Text Celeste.Textbox::text
        IL_04f6: ldloc.3
        IL_04f7: ldloc.s 8
        IL_04f9: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Textbox>("text"),
                static instr => instr.MatchLdloc3(),
                static instr => instr.MatchLdloc(8),
                static instr => instr.MatchCall<Vector2>("op_Addition")).LogHookOnFailure(il))
        {
            return;
        }

        ILLabel IfBranchEnd = cursor.DefineLabel();

        cursor.EmitDelegate(TryJustifyTextDown);
        cursor.EmitBrtrue(IfBranchEnd);


        /*
        IL_0526: ldloc.1
        IL_0527: ldarg.0
        IL_0528: callvirt instance int32 Celeste.Textbox::get_Start()
        IL_052d: ldc.i4 2147483647
        IL_0532: callvirt instance void Celeste.FancyText/Text::Draw(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32, int32, int32)
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdloc1(),
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchCallvirt<Textbox>("get_Start"),
                static instr => instr.MatchLdcI4(int.MaxValue),
                static instr => instr.MatchCallvirt<FancyText.Text>("Draw")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.MarkLabel(IfBranchEnd);
        cursor.EmitLdloc(1);
        cursor.EmitLdloc(10);
        cursor.EmitLdloc(3);
        cursor.EmitLdloc(8);
        cursor.EmitLdloc(9);
        cursor.EmitDelegate(JustifyTextDown);
    }

    private static bool TryJustifyTextDown()
    {
        FancyText.Text text = SearchUtils.GetEntity<Textbox>(true)?.text;
        if (text is null) return false;

        DynamicData selfData = DynamicData.For(text);
        return selfData.TryGetTrue("MintChocolateHelper:JustifyTextDownwards");
    }

    private static void JustifyTextDown(float num, float num6, Vector2 vector, Vector2 vector2, Vector2 vector3)
    {
        if (!TryJustifyTextDown()) return;

        Textbox textbox = SearchUtils.GetEntity<Textbox>(true);
        FancyText.Text text = textbox?.text;
        if (text is null) return;

        DynamicData selfData = DynamicData.For(text);

        if (selfData.TryGetTrue("MintChocolateHelper:DisableTextShrink"))
        {
            textbox.text.Draw(
                vector + vector2 + vector3 - new Vector2(0, textbox.linesPerPage * textbox.lineHeight / 2) - Vector2.UnitY * 1.5f,
                new Vector2(0.5f, 0f), new Vector2(1f, num), num, textbox.Start);
        }
        else
        {
            textbox.text.Draw(
                vector + vector2 + vector3 - new Vector2(0, textbox.linesPerPage * textbox.lineHeight / 2) - Vector2.UnitY * 1.5f,
                new Vector2(0.5f, 0f), new Vector2(1f, num) * num6, num, textbox.Start);
        }
    }
}