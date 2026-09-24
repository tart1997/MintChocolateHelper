// namespace Celeste.Mod.MintChocolateHelper.DialogCommands;
//
// [CustomCommand]
// public static class SwitchFontCommand
// {
//     private static readonly (string, McTrigger)[] CommandStrings = [
//         ("switchFont", new SwitchFontTrigger(false)),
//         ("resetFont", new SwitchFontTrigger(true))
//     ];
//     
//     private class SwitchFontTrigger : McTrigger
//     {
//         internal SwitchFontTrigger(bool resetFont, params string[] args) : base(args)
//         {
//             // OnReadAction = (data, _) => {
//             //     data.
//             // };
//         }
//     }
//     
//     [OnLoad]
//     internal static void RegisterCommands()
//     {
//         Utils.LogVerbose($"Registering Custom Dialog Command: {nameof(SwitchFontCommand)}...");
//         CommandStrings.Register();
//     }
// }
// TODO: this