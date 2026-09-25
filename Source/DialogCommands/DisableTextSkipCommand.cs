namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

[CustomCommand]
public static class DisableTextSkipCommand
{
    private static readonly (string, McTrigger)[] CommandStrings = [
        (">/>", new DisableTextSkipTrigger(true)),
        (">>>", new DisableTextSkipTrigger(false))
    ];

    private class DisableTextSkipTrigger : McTrigger
    {
        internal DisableTextSkipTrigger(bool active, params string[] args) : base(args)
        {
            OnReadAction = (_, _) => SearchUtils.GetEntity<Textbox>(true)?.autoPressContinue = active;
        }
    }

    [OnLoad]
    internal static void RegisterCommands()
    {
        Utils.LogVerbose($"Registering Custom Dialog Command: {nameof(DisableTextSkipTrigger)}...");
        CommandStrings.Register();
    }

    [OnUnload]
    internal static void Unload() => CommandStrings.Unregister();
}