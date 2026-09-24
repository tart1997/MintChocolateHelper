namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

[CustomCommand]
public static class AutoNextPageCommand
{
    private static readonly (string, McTrigger)[] CommandStrings = [
        ("==>", new AutoNextPageTrigger(true)),
        ("-->", new AutoNextPageTrigger(false))
    ];
    
     private class AutoNextPageTrigger : McTrigger
    {
        internal AutoNextPageTrigger(bool active, params string[] args) : base(args)
        {
            OnReadAction = (data, _) => data.Set("MintChocolateHelper:AutoNextPage", active);
        }
    }

    [OnLoad]
    internal static void RegisterCommands()
    {
        Utils.LogVerbose($"Registering Custom Dialog Command: {nameof(AutoNextPageCommand)}...");
        CommandStrings.Register();
    }
    
    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogVerbose($"Loading {nameof(AutoNextPageCommand)} Hooks...");
        On.Celeste.Textbox.ContinuePressed += TextboxOnContinuePressed;
    }

    [OnUnload]
    internal static void Unload() => On.Celeste.Textbox.ContinuePressed -= TextboxOnContinuePressed;

    private static bool TextboxOnContinuePressed(On.Celeste.Textbox.orig_ContinuePressed orig, Textbox self)
        => DynamicData.For(self.text).TryGetTrue("MintChocolateHelper:AutoNextPage") && !self.autoPressContinue || orig(self);
}