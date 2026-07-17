namespace Celeste.Mod.MintChocolateHelper.Cutscenes;

public static class ParserHooks
{
    internal static void Load()
    {
        IL.Celeste.FancyText.AddNewLine += FancyTextOnAddNewLine;
        IL.Celeste.Textbox.Render += TextboxOnRender;
    }

    internal static void Unload()
    {
        IL.Celeste.FancyText.AddNewLine -= FancyTextOnAddNewLine;
        IL.Celeste.Textbox.Render -= TextboxOnRender;
    }

    private static void FancyTextOnAddNewLine(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_0032: ldarg.0
        // IL_0033: ldfld int32 Celeste.FancyText::currentLine
        // IL_0038: ldarg.0
        // IL_0039: ldfld int32 Celeste.FancyText::linesPerPage
        // IL_003e: ble.s IL_007e

        ILLabel jumpToNewline = null;

        if (!cursor.TryGotoNextBestFit(MoveType.Before,
            i => i.MatchLdarg0(),
            i => i.MatchLdfld<FancyText>("currentLine"),
            i => i.MatchLdarg0(),
            i => i.MatchLdfld<FancyText>("linesPerPage"),
            i => i.MatchBle(out jumpToNewline)))
        {
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!");
            return;
        }

        cursor.EmitBr(jumpToNewline);
    }

    private static void TextboxOnRender(ILContext il)
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
        cursor.MoveAfterLabels();
        cursor.EmitBr(IfBranchEnd);

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

    private static void JustifyTextDown(float num, float num6, Vector2 vector, Vector2 vector2, Vector2 vector3)
    {
        if (Utils.LevelIsNotSafe(out Level level)) return;
        Textbox textbox = level.Tracker.GetEntitiesTrackIfNeeded<Textbox>().Cast<Textbox>().FirstOrDefault();
        textbox?.text.Draw(vector + vector2 + vector3 + new Vector2(3, textbox.lineHeight * (textbox.text.Lines - 1) / 4) * 1.5f + Vector2.UnitY, new Vector2(0.5f, 0.5f), new Vector2(1f, num) * num6, num, textbox.Start);
    }
}