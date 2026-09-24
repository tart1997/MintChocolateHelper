using HookUtils = Celeste.Mod.Helpers.HookUtils;

namespace Celeste.Mod.MintChocolateHelper.Module;

public static class MintChocolateHelperHookLoading
{
    internal static void TryDisableInlining()
    {
        HookUtils.TryDisableInlining(typeof(Platform).GetMethod(nameof(Platform.OnShake), BindingFlags.Public | BindingFlags.Instance));
        HookUtils.TryDisableInlining(typeof(Player).GetMethod(nameof(Player.DashEnd), BindingFlags.NonPublic | BindingFlags.Instance));
    }

    internal static void LoadAllHooks()
    {
        LifecycleMethods.OnLoad();
        CustomDialogCommands.Load();

        foreach (Type type in typeof(MintChocolateHelperHookLoading).Assembly.GetTypes())
        {
            if (type.GetCustomAttribute<CustomCommandAttribute>() is { } || type.GetCustomAttribute<ConditionalEntityAttribute>() is { })
            {
                type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
            }
        }
    }

    private static readonly Dictionary<(Type, MethodInfo), string[]> AllCustomCommands = [];
    private static readonly Dictionary<MethodInfo, string[]> AllConditionalObjects = [];

    internal static void LoadAllConditionalObjects()
    {
        AllCustomCommands.Clear();
        AllConditionalObjects.Clear();

        foreach (Type type in typeof(MintChocolateHelperHookLoading).Assembly.GetTypes())
        {
            if (type.GetCustomAttribute<CustomCommandAttribute>() is { })
            {
                if (type.GetField("CommandStrings", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) is not ValueTuple<string, McTrigger>[] commandStrings) continue;
                MethodInfo method = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic);

                List<string> finalCommandStrings = [];
                foreach ((string commandString, McTrigger _) in commandStrings)
                {
                    finalCommandStrings.Add(commandString);
                }

                AllCustomCommands.Add((type, method), [.. finalCommandStrings.Select(x => $"{{{x}}}")]);
            }
            else if (type.GetCustomAttribute<ConditionalEntityAttribute>() is { } loadAttribute)
            {
                List<string> dependencies = [.. loadAttribute.Dependencies.Select(x => $"MintChocolateHelper/{x}")];

                if (type.GetCustomAttribute<CustomEntityAttribute>() is { } entityAttribute)
                {
                    dependencies.AddRange(entityAttribute.IDs);
                }

                MethodInfo method = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic);
                if (method is { }) AllConditionalObjects.Add(method, [.. dependencies]);
            }
        }
    }

    internal static void ConditionalLoad()
    {
        Utils.LogVerbose("Loading MintChocolateHelper Basics...");
        LifecycleMethods.OnLoad();
        On.Celeste.LevelLoader.ctor += LevelLoader_ctor;
    }

    public static void Unload()
    {
        Utils.LogVerbose("Unloading MintChocolateHelper...");
        LifecycleMethods.OnUnload();
        On.Celeste.LevelLoader.ctor -= LevelLoader_ctor;
    }

    private static void LevelLoader_ctor(On.Celeste.LevelLoader.orig_ctor orig, LevelLoader self, Session session, Vector2? startposition)
    {
        orig(self, session, startposition);
        LifecycleMethods.OnUnload();

        Utils.LogVerbose("Loading MintChocolateHelper Hooks...");

        List<(Type, MethodInfo)> commandsToLoad = [];
        List<MethodInfo> objectsToLoad = [];

        foreach (string dialog in Dialog.Language.Dialog.Values)
        {
            foreach (((Type type, MethodInfo method), string[] commandStrings) in AllCustomCommands)
            {
                if (!commandsToLoad.Contains((type, method)) && commandStrings.Any(commandString => dialog.Contains(commandString)))
                {
                    commandsToLoad.Add((type, method));
                }
            }
        }

        foreach (LevelData level in session.MapData.Levels)
        {
            foreach (EntityData entity in level.Entities.Distinct())
            {
                foreach ((MethodInfo method, string[] dependecyList) in AllConditionalObjects)
                {
                    if (!objectsToLoad.Contains(method) && dependecyList.Any(dependecy => dependecy == entity.Name))
                    {
                        objectsToLoad.Add(method);
                        break;
                    }
                }
            }

            foreach (EntityData trigger in level.Triggers.Distinct())
            {
                foreach ((MethodInfo method, string[] dependecyList) in AllConditionalObjects)
                {
                    if (!objectsToLoad.Contains(method) && dependecyList.Any(dependecy => dependecy == trigger.Name))
                    {
                        objectsToLoad.Add(method);
                        break;
                    }
                }
            }
        }

        if (MintChocolateHelperModule.Settings.ForceUniversalAnimatedTiles)
        {
            MethodInfo method = typeof(UniversalAnimatedTilesController).GetMethod(nameof(UniversalAnimatedTilesController.Load), BindingFlags.NonPublic | BindingFlags.Static);
            if (!objectsToLoad.Contains(method)) objectsToLoad.Add(method);
        }

        if (commandsToLoad.Count != 0) CustomDialogCommands.Load();
        foreach ((Type _, MethodInfo method) in commandsToLoad)
        {
            method?.Invoke(null, null);
        }

        foreach (MethodInfo method in objectsToLoad)
        {
            method.Invoke(null, null);
        }
    }
}