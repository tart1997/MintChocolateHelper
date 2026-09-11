// TODO: Done Here :)
namespace Celeste.Mod.MintChocolateHelper.MintUtils;

internal static class Utils
{
    [UsedImplicitly]
    internal static void Log(LogLevel logLevel , string message)
    {
        Logger.Log(logLevel, MintChocolateHelperModule.ModName, message);
    }
    
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

    extension(Level level)
    {
        [UsedImplicitly]
        internal bool GetFlag(string flag)
        {
            return level.Session.GetFlag(flag);
        }

        [UsedImplicitly]
        internal void SetFlag(string flag, bool setTo = true)
        {
            level.Session.SetFlag(flag, setTo);
        }
    }
    
    extension(Scene scene)
    {
        [UsedImplicitly]
        internal bool GetFlag(string flag)
        {
            return ((Level)scene).Session.GetFlag(flag);
        }

        [UsedImplicitly]
        internal void SetFlag(string flag, bool setTo = true)
        {
            ((Level)scene).Session.SetFlag(flag, setTo);
        }
    }

    [UsedImplicitly]
    internal static bool GetFlag(string flag)
    {
        return ((Level)Engine.Scene).Session.GetFlag(flag);
    }

    [UsedImplicitly]
    internal static void SetFlag(string flag, bool setTo = true)
    {
        ((Level)Engine.Scene).Session.SetFlag(flag, setTo);
    }
}