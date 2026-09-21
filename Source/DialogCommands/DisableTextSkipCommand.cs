namespace Celeste.Mod.MintChocolateHelper.DialogCommands;

public static class DisableTextSkipCommand
{
    private class DisableTextSkipTrigger : McTrigger
    {
        internal DisableTextSkipTrigger(bool active, params List<string> args) : base(args)
        {
            OnReadAction = (_, _) => SearchUtils.GetEntity<Textbox>(true)?.autoPressContinue = active;
        }
    }

    [OnLoad]
    internal static void Load()
    {
        CustomDialogCommands.Register(">/>", new DisableTextSkipTrigger(true));
        CustomDialogCommands.Register(">>>", new DisableTextSkipTrigger(false));
    }
}