namespace Celeste.Mod.MintChocolateHelper.MintUtils;

internal static class Utils
{
    [UsedImplicitly]
    internal static bool LevelIsSafe() => LevelIsSafe(out _);

    [UsedImplicitly]
    internal static bool LevelIsNotSafe() => LevelIsNotSafe(out _);

    [UsedImplicitly]
    internal static bool LevelIsSafe(out Level level)
    {
        if (Engine.Scene is Level lvl)
        {
            level = lvl;
            return true;
        }

        level = null;
        return false;
    }

    [UsedImplicitly]
    internal static bool LevelIsNotSafe(out Level level)
    {
        if (Engine.Scene is not Level lvl)
        {
            level = null;
            return true;
        }

        level = lvl;
        return false;
    }

    [UsedImplicitly]
    internal static bool SceneIsSafe(Scene scene) => SceneIsSafe(scene, out _);

    [UsedImplicitly]
    internal static bool SceneIsNotSafe(Scene scene) => SceneIsNotSafe(scene, out _);

    [UsedImplicitly]
    internal static bool SceneIsSafe(Scene scene, out Level level)
    {
        if (scene is Level lvl)
        {
            level = lvl;
            return true;
        }

        level = null;
        return false;
    }

    [UsedImplicitly]
    internal static bool SceneIsNotSafe(Scene scene, out Level level)
    {
        if (scene is not Level lvl)
        {
            level = null;
            return true;
        }

        level = lvl;
        return false;
    }

    [UsedImplicitly]
    internal static void LogError(string message)
    {
        Logger.Log(LogLevel.Error, "Mint Chocolate Helper", message);
    }

    [UsedImplicitly]
    internal static void LogInfo(string message)
    {
        Logger.Log(LogLevel.Info, "Mint Chocolate Helper", message);
    }
}