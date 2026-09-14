// ReSharper disable MemberCanBePrivate.Global
using ModInteropImportGenerator;

namespace Celeste.Mod.MintChocolateHelper.ModInterop;

[GenerateImports("FrostHelper")]
public static partial class FrostHelperImports
{
    public static partial bool IsCeilingSpring(Spring spring);
    public static partial Vector2 GetSpringSpeedMultiplier(Spring spring);

    public static partial bool TryCreateSessionExpression(string str, [NotNullWhen(true)] out object expression);
    public static partial bool GetBoolSessionExpressionValue(object expression, Session session);
    
    
    internal static bool SafeIsCeilingSpring(Spring spring) => IsImported && IsCeilingSpring(spring);
    internal static Vector2 SafeGetSpringSpeedMultiplier(Spring spring) => IsImported ? GetSpringSpeedMultiplier(spring) : Vector2.One;

    internal static bool SafeTryCreateSessionExpression(string str, [NotNullWhen(true)] out object expression)
    {
        bool returnValue = TryCreateSessionExpression(str, out object expr);
        expression = expr;
        return IsImported && returnValue;
    }

    internal static bool SafeGetBoolSessionExpressionValue(object expression, Session session) => IsImported && GetBoolSessionExpressionValue(expression, session);
}