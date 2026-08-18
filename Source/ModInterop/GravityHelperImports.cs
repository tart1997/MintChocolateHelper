namespace Celeste.Mod.MintChocolateHelper.ModInterop;

[GenerateImports("GravityHelper")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static partial class GravityHelperImports
{
    public static partial bool IsPlayerInverted();

    internal static Vector2 InvertIfPlayerInverted(Vector2 v) => IsImported && IsPlayerInverted() ? new Vector2(v.X, -v.Y) : v;
}