namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

public static class AutoNextPageCommand
{
    private class AutoNextPageTrigger : McTrigger
    {
        internal AutoNextPageTrigger(bool active, params List<string> args) : base(args)
        {
            OnReadAction = (data, _) => data.Set("MintChocolateHelper:AutoNextPage", active);
        }
    }

    [OnLoad]
    internal static void Load()
    {
        CustomDialogCommands.Register("==>", new AutoNextPageTrigger(true));
        CustomDialogCommands.Register("-->", new AutoNextPageTrigger(false));

        On.Celeste.Textbox.ContinuePressed += TextboxOnContinuePressed;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Textbox.ContinuePressed -= TextboxOnContinuePressed;
    }

    private static bool TextboxOnContinuePressed(On.Celeste.Textbox.orig_ContinuePressed orig, Textbox self)
        => DynamicData.For(self.text).TryGetTrue("MintChocolateHelper:AutoNextPage") && !self.autoPressContinue || orig(self);
}