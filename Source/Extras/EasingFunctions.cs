namespace Celeste.Mod.MintChocolateHelper.Extras;

public class EasingFunctions
{
    public enum EasingFunction
    {
        Linear,
        BackIn,
        BackOut,
        BackInOut,
        BigBackIn,
        BigBackOut,
        BigBackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        CubeIn,
        CubeOut,
        CubeInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        QuadIn,
        QuadOut,
        QuadInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        SineIn,
        SineOut,
        SineInOut
    }

    internal static float Linear(float t) => Ease.Linear(t);
    internal static float BackIn(float t) => Ease.BackIn(t);
    internal static float BackOut(float t) => Ease.BackOut(t);
    internal static float BackInOut(float t) => Ease.BackInOut(t);
    internal static float BigBackIn(float t) => Ease.BigBackIn(t);
    internal static float BigBackOut(float t) => Ease.BigBackOut(t);
    internal static float BigBackInOut(float t) => Ease.BigBackInOut(t);
    internal static float BounceIn(float t) => Ease.BounceIn(t);
    internal static float BounceOut(float t) => Ease.BounceOut(t);
    internal static float BounceInOut(float t) => Ease.BounceInOut(t);
    internal static float CubeIn(float t) => Ease.CubeIn(t);
    internal static float CubeOut(float t) => Ease.CubeOut(t);
    internal static float CubeInOut(float t) => Ease.CubeInOut(t);
    internal static float ElasticIn(float t) => Ease.ElasticIn(t);
    internal static float ElasticOut(float t) => Ease.ElasticOut(t);
    internal static float ElasticInOut(float t) => Ease.ElasticInOut(t);
    internal static float ExpoIn(float t) => Ease.ExpoIn(t);
    internal static float ExpoOut(float t) => Ease.ExpoOut(t);
    internal static float ExpoInOut(float t) => Ease.ExpoInOut(t);
    internal static float QuadIn(float t) => Ease.QuadIn(t);
    internal static float QuadOut(float t) => Ease.QuadOut(t);
    internal static float QuadInOut(float t) => Ease.QuadInOut(t);
    internal static float QuintIn(float t) => Ease.QuintIn(t);
    internal static float QuintOut(float t) => Ease.QuintOut(t);
    internal static float QuintInOut(float t) => Ease.QuintInOut(t);
    internal static float SineIn(float t) => Ease.SineIn(t);
    internal static float SineOut(float t) => Ease.SineOut(t);
    internal static float SineInOut(float t) => Ease.SineInOut(t);

    internal delegate float Easer(float t);
}