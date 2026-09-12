namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

public static class BreakTextBoxCommand
{
    private class McTrigger : FancyText.Trigger
    {
        public readonly bool DisableTextShrink;

        public McTrigger(bool disableTextShrink)
        {
            Silent = true;
            DisableTextShrink = disableTextShrink;
        }
    }

    [OnLoad]
    internal static void Load()
    {
        IL.Celeste.FancyText.Parse += ParseCommand;
        IL.Celeste.FancyText.AddNewLine += SkipAddNewPage;

        On.Celeste.Textbox.Render += ShouldJustifyTextDown;
        IL.Celeste.Textbox.Render += JustifyTextDownHook;
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.FancyText.Parse -= ParseCommand;
        IL.Celeste.FancyText.AddNewLine -= SkipAddNewPage;

        On.Celeste.Textbox.Render -= ShouldJustifyTextDown;
        IL.Celeste.Textbox.Render -= JustifyTextDownHook;
    }

    private static void ParseCommand(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0231: ldarg.0
        IL_0232: ldfld class Celeste.FancyText/Text Celeste.FancyText::group
        IL_0237: ldfld class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node> Celeste.FancyText/Text::Nodes
        IL_023c: newobj instance void Celeste.FancyText/NewPage::.ctor()
        IL_0241: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node>::Add(!0)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<FancyText>("group"),
                static instr => instr.MatchLdfld<FancyText.Text>("Nodes"),
                static instr => instr.MatchNewobj<FancyText.NewPage>(".ctor"),
                static instr => instr.MatchCallvirt<List<FancyText.Node>>("Add")).LogHookOnFailure(il))
        {
            return;
        }

        Utils.Log(LogLevel.Info, "MintChocolateHelper is hooking into FancyText.Parse, please let me know if something explodes!");
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate(ResetDisableLineLimit);


        /*
        IL_02bc: ldstr "/>>"
        IL_02c1: callvirt instance bool [mscorlib]System.String::Equals(string)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdstr("/>>"),
                static instr => instr.MatchCallvirt<string>("Equals")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldloc_S, il.Method.Body.Variables[7]);
        cursor.EmitDelegate(SetDisableLineLimit);
    }

    private static void ResetDisableLineLimit(FancyText text)
    {
        DynamicData parserData = DynamicData.For(text);
        parserData.SetFalse("MintChocolateHelper:DisableLineLimit");
    }

    private static void SetDisableLineLimit(FancyText text, string s)
    {
        DynamicData parserData = DynamicData.For(text);
        FancyText.Text? group = parserData.Get<FancyText.Text?>("group");
        List<FancyText.Node> nodes = group?.Nodes;
        switch (s)
        {
            case "vvv":
                nodes?.Add(new McTrigger(false));
                parserData.SetTrue("MintChocolateHelper:DisableLineLimit");
                break;
            case "VVV":
                nodes?.Add(new McTrigger(true));
                parserData.SetTrue("MintChocolateHelper:DisableLineLimit");
                break;
        }
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

    private static bool ShouldSkipAddNewPage(FancyText text)
    {
        DynamicData parserData = DynamicData.For(text);
        return parserData.TryGetTrue("MintChocolateHelper:DisableLineLimit");
    }

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

    private static void ShouldJustifyTextDown(On.Celeste.Textbox.orig_Render orig, Textbox self)
    {
        FancyText.Text text = self.text;

        bool HasBreakTextBoxCommand = false;
        bool hasdisableTextShrink = false;

        for (int i = self.Start; i < text.Nodes.Count; i++)
        {
            if (text.Nodes[i] is McTrigger trigger)
            {
                HasBreakTextBoxCommand = true;
                if (trigger.DisableTextShrink)
                {
                    hasdisableTextShrink = true;
                }
            }
            else if (text.Nodes[i] is FancyText.NewPage)
            {
                break;
            }
        }

        DynamicData selfData = DynamicData.For(text);
        selfData.Set("MintChocolateHelper:JustifyTextDownwards", HasBreakTextBoxCommand);
        selfData.Set("MintChocolateHelper:DisableTextShrink", hasdisableTextShrink);

        orig(self);
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
            textbox.text.Draw(vector + vector2 + vector3 - new Vector2(0, textbox.linesPerPage * textbox.lineHeight / 2) - Vector2.UnitY * 1.5f, new Vector2(0.5f, 0f), new Vector2(1f, num), num, textbox.Start);
        }
        else
        {
            textbox.text.Draw(vector + vector2 + vector3 - new Vector2(0, textbox.linesPerPage * textbox.lineHeight / 2) - Vector2.UnitY * 1.5f, new Vector2(0.5f, 0f), new Vector2(1f, num) * num6, num, textbox.Start);
        }
    }
}