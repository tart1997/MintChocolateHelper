using MonoMod.ModInterop;

namespace Celeste.Mod.MintChocolateHelper.Exports;

/// <summary>
///     Provides export functions for other mods to import.
///     If you do not need to export any functions, delete this class and the corresponding call
///     to ModInterop() in <see cref="MintChocolateHelperModule.Load" />
/// </summary>
[ModExportName("MintChocolateHelper")]
public static class MintChocolateHelperExports
{
    public static int MintChocolateHelperInteropVersion => 1;
}