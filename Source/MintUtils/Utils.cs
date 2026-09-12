namespace Celeste.Mod.MintChocolateHelper.MintUtils;

internal static class Utils
{
    [UsedImplicitly]
    internal static void Log(LogLevel logLevel , string message)
    {
        Logger.Log(logLevel, MintChocolateHelperModule.ModName, message);
    }
    
    [UsedImplicitly]
    internal static bool LevelIsSafe() => Engine.Scene is Level;

    [UsedImplicitly]
    internal static bool LevelIsNotSafe() => Engine.Scene is not Level;

    [UsedImplicitly]
    internal static bool LevelIsSafe([NotNullWhen(true)] out Level level)
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
    internal static bool LevelIsNotSafe([NotNullWhen(false)] out Level level)
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
    internal static bool SceneIsSafe(Scene scene) => scene is Level;

    [UsedImplicitly]
    internal static bool SceneIsNotSafe(Scene scene) => scene is not Level;

    [UsedImplicitly]
    internal static bool SceneIsSafe(Scene scene, [NotNullWhen(true)] out Level level)
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
    internal static bool SceneIsNotSafe(Scene scene, [NotNullWhen(false)] out Level level)
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
        [CanBeNull]
        [UsedImplicitly]
        internal Level AsLevel()
        {
            return scene as Level;
        }
        
        [UsedImplicitly]
        internal bool GetFlag(string flag)
        {
            return scene.AsLevel()!.Session.GetFlag(flag);
        }

        [UsedImplicitly]
        internal void SetFlag(string flag, bool setTo = true)
        {
            scene.AsLevel()?.Session.SetFlag(flag, setTo);
        }
    }
    
    [CanBeNull]
    [UsedImplicitly]
    internal static Level GetLevel()
    {
        return Engine.Scene as Level;
    }

    [UsedImplicitly]
    internal static bool GetFlag(string flag)
    {
        return GetLevel()!.Session.GetFlag(flag);
    }

    [UsedImplicitly]
    internal static void SetFlag(string flag, bool setTo = true)
    {
        GetLevel()?.Session.SetFlag(flag, setTo);
    }

    [UsedImplicitly]
    internal static T LogValue<T>(this T val)
    {
        Log(LogLevel.Info, $"{typeof(T).Name}: {val}");
        return val;
    }
}