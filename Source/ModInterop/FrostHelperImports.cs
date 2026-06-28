using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using ModInteropImportGenerator;

namespace Celeste.Mod.MintChocolateHelper.ModInterop;
[GenerateImports("FrostHelper")]
public static partial class FrostHelperImports
{
    public static partial bool IsCeilingSpring(Spring spring);
    public static partial Vector2 GetSpringSpeedMultiplier(Spring spring);
    
    public static partial bool TryCreateSessionExpression(string str, [NotNullWhen(true)] out object expression);
    public static partial bool GetBoolSessionExpressionValue(object expression, Session session);
}