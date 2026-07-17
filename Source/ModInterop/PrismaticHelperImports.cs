namespace Celeste.Mod.MintChocolateHelper.ModInterop;

[GenerateImports("PrismaticHelper.CutsceneTriggers")]
public static partial class PrismaticHelperImports
{
    public static partial void RegisterTrigger(string modName, string triggerName, Func<Player, Level, List<string>, IEnumerator> effect);
}