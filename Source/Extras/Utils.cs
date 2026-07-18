namespace Celeste.Mod.MintChocolateHelper.Extras;

internal static class Utils
{
    internal static bool LevelIsSafe() => LevelIsSafe(out _);
    internal static bool LevelIsNotSafe() => LevelIsNotSafe(out _);

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

    internal static bool SceneIsSafe(Scene scene) => SceneIsSafe(scene, out _);
    internal static bool SceneIsNotSafe(Scene scene) => SceneIsNotSafe(scene, out _);

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

    internal static bool CheckEntityExistence<T>() where T : Entity => CheckEntityExistence<T>(out _);

    internal static bool CheckEntityExistence<T>(out T Entity, bool TrackIfNeeded = false) where T : Entity
    {
        if (Engine.Scene is not Level level)
        {
            Entity = null;
            return false;
        }

        T entity = TrackIfNeeded ? level.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().FirstOrDefault() : level.Tracker.GetEntity<T>();

        if (entity != null)
        {
            Entity = entity;
            return true;
        }

        Entity = null;
        return false;
    }

    public static void LogError(string message)
    {
        Logger.Log(LogLevel.Error, "Mint Chocolate Helper", message);
    }

    public static void LogInfo(string message)
    {
        Logger.Log(LogLevel.Info, "Mint Chocolate Helper", message);
    }

    public static VariableDefinition AddVariable(this MethodBody self, TypeReference type)
    {
        VariableDefinition variable = new(type);
        self.Variables.Add(variable);
        return variable;
    }
}