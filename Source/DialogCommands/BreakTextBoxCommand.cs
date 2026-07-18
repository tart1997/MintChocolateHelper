namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

[SuppressMessage("Usage", "CL0001:Lambda passed to ILCursor.EmitDelegate")]
public static class BreakTextBoxCommand
{
    [OnLoad]
    internal static void Load()
    {
        IL.Celeste.FancyText.Parse += ParseCommand;
        IL.Celeste.FancyText.AddNewLine += SkipAddNewPage;

        IL.Celeste.Textbox.Render += JustifyTextDownHook;
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.FancyText.Parse -= ParseCommand;
        IL.Celeste.FancyText.AddNewLine -= SkipAddNewPage;

        IL.Celeste.Textbox.Render -= JustifyTextDownHook;
    }

    private static void ParseCommand(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_0231: ldarg.0
        // IL_0232: ldfld class Celeste.FancyText/Text Celeste.FancyText::group
        // IL_0237: ldfld class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node> Celeste.FancyText/Text::Nodes
        // IL_023c: newobj instance void Celeste.FancyText/NewPage::.ctor()
        // IL_0241: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node>::Add(!0)

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<FancyText>("group"),
            static instr => instr.MatchLdfld<FancyText.Text>("Nodes"),
            static instr => instr.MatchNewobj<FancyText.NewPage>(".ctor"),
            static instr => instr.MatchCallvirt<List<FancyText.Node>>("Add")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck! 1\n\n\n");
            return;
        }
        Utils.LogInfo("MintChocolateHelper is hooking into FancyText.Parse, please let me know if something explodes!");

        cursor.Emit(OpCodes.Ldarg_0); // this
        cursor.EmitDelegate<Action<FancyText>>(text => {
            DynamicData parserData = new(text);
            parserData.Set("MintChocolateHelper:DisableLineLimit", false);
        });


        // IL_02bc: ldstr "/>>"
        // IL_02c1: callvirt instance bool [mscorlib]System.String::Equals(string)

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdstr("/>>"),
            static instr => instr.MatchCallvirt<string>("Equals")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck! 2\n\n\n");
            return;
        }

        cursor.Emit(OpCodes.Ldarg_0); // this
        cursor.Emit(OpCodes.Ldloc_S, il.Method.Body.Variables[7]); // s
        cursor.EmitDelegate<Action<FancyText, string>>((text, s) => {
            DynamicData parserData = new(text);
            FancyText.Text group = parserData.Get<FancyText.Text>("group");
            if (s == "VVV")
            {
                parserData.Set("MintChocolateHelper:DisableLineLimit", true);
                DynamicData.For(group).Set("MintChocolateHelper:JustifyTextDownwards", true);
            }
        });
    }

    private static void SkipAddNewPage(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_0032: ldarg.0
        // IL_0033: ldfld int32 Celeste.FancyText::currentLine
        // IL_0038: ldarg.0
        // IL_0039: ldfld int32 Celeste.FancyText::linesPerPage
        // IL_003e: ble.s IL_007e

        ILLabel jumpToNewline = null;

        if (!cursor.TryGotoNextBestFit(MoveType.Before,
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<FancyText>("currentLine"),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<FancyText>("linesPerPage"),
            instr => instr.MatchBle(out jumpToNewline)))
        {
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!");
            return;
        }

        cursor.EmitLdarg0();
        cursor.EmitDelegate(ShouldSkipAddNewPage);
        cursor.EmitBrtrue(jumpToNewline);
    }

    private static bool ShouldSkipAddNewPage(FancyText text)
    {
        DynamicData parserData = new(text);
        return parserData.TryGet("MintChocolateHelper:DisableLineLimit", out bool? DisableLineLimit) && DisableLineLimit == true;
    }

    private static void JustifyTextDownHook(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_04f0: ldarg.0
        // IL_04f1: ldfld class Celeste.FancyText/Text Celeste.Textbox::text
        // IL_04f6: ldloc.3
        // IL_04f7: ldloc.s 8
        // IL_04f9: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<Textbox>("text"),
            static instr => instr.MatchLdloc3(),
            static instr => instr.MatchLdloc(8),
            static instr => instr.MatchCall<Vector2>("op_Addition")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck! 1\n\n\n");
            return;
        }

        ILLabel IfBranchEnd = cursor.DefineLabel();
        cursor.EmitDelegate(TryJustifyTextDown);
        cursor.EmitBrtrue(IfBranchEnd);

        // IL_0526: ldloc.1
        // IL_0527: ldarg.0
        // IL_0528: callvirt instance int32 Celeste.Textbox::get_Start()
        // IL_052d: ldc.i4 2147483647
        // IL_0532: callvirt instance void Celeste.FancyText/Text::Draw(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32, int32, int32)

        if (!cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchLdloc1(),
            static instr => instr.MatchLdarg0(),
            static instr => instr.MatchCallvirt<Textbox>("get_Start"),
            static instr => instr.MatchLdcI4(int.MaxValue),
            static instr => instr.MatchCallvirt<FancyText.Text>("Draw")))
        {
            Logger.Info("debug", $"\n\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck! 2\n\n\n");
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
        if (Utils.LevelIsNotSafe(out Level level)) return false;
        Textbox textbox = level.Tracker.GetEntitiesTrackIfNeeded<Textbox>().Cast<Textbox>().FirstOrDefault();
        FancyText.Text text = textbox?.text;
        if (text is null) return false;

        DynamicData selfData = new(text);
        return selfData.TryGet("MintChocolateHelper:JustifyTextDownwards", out bool? JustifyTextDownwards) && JustifyTextDownwards == true;
    }

    private static void JustifyTextDown(float num, float num6, Vector2 vector, Vector2 vector2, Vector2 vector3)
    {
        if (Utils.LevelIsNotSafe(out Level level) || !TryJustifyTextDown()) return;
        Textbox textbox = level.Tracker.GetEntitiesTrackIfNeeded<Textbox>().Cast<Textbox>().FirstOrDefault();
        textbox?.text.Draw(vector + vector2 + vector3 - new Vector2(0, textbox.linesPerPage * textbox.lineHeight / 2) - Vector2.UnitY * 1.5f, new Vector2(0.5f, 0f), new Vector2(1f, num) * num6, num, textbox.Start);
    }
}