namespace Celeste.Mod.MintChocolateHelper.Cutscenes;

public static class DialogCommands
{
    private const string modName = "mint_chocolate_helper";

    [OnLoad]
    public static void Load()
    {
        ParserHooks.Load();
        if (!PrismaticHelperImports.IsImported) return;

        PrismaticHelperImports.RegisterTrigger(modName, "disable_textbox_line_limit", (_, level, _) => DisableTextboxLineLimit(level));
        return;

        static IEnumerator DisableTextboxLineLimit(Level level)
        {
            Utils.LogInfo("\n\n\nsdfalksjfhksadlfasdkjghfsddfj sdgfjkhsadfgkjsahhjkhgdhsdf\n\n\n");
            yield return null;
        }
    }

    [OnUnload]
    public static void Unload()
    {
        ParserHooks.Unload();
    }
}