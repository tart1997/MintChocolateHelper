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

        foreach (Type type in ReflectionUtils.MintChocolateHelperTypes)
        {
            foreach (MethodInfo loadMethod in type.GetMethods(ReflectionUtils.All))
            {
                loadMethod.SimpleInvokeIf(loadMethod.HasAttribute<ConditionalOnLoadAttribute>());
            }
        }
    }

    private static readonly Dictionary<(Type, MethodInfo, MethodInfo), string[]> AllCustomCommands = [];
    private static readonly Dictionary<MethodInfo, string[]> AllConditionalObjects = [];

    internal static void LoadAllConditionalObjects()
    {
        AllCustomCommands.Clear();
        AllConditionalObjects.Clear();

        foreach (Type type in ReflectionUtils.MintChocolateHelperTypes)
        {
            if (type.HasAttribute<CustomCommandAttribute>())
            {
                if (type.GetField("CommandStrings", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) is not ValueTuple<string, McTrigger>[] commandStrings) continue;

                List<string> finalCommandStrings = [];
                foreach ((string commandString, McTrigger _) in commandStrings)
                {
                    finalCommandStrings.Add(commandString);
                }

                AllCustomCommands.Add((type, type.GetMethodWith<ConditionalOnLoadAttribute>(), type.GetMethodWith<OnUnloadAttribute>()), [.. finalCommandStrings.Select(x => $"{{{x}}}")]);
            }
            else if (type.HasAttribute(out ConditionalEntityAttribute loadAttribute))
            {
                List<string> dependencies = [.. loadAttribute.Dependencies.Select(x => $"MintChocolateHelper/{x}")];

                if (type.HasAttribute(out CustomEntityAttribute entityAttribute))
                {
                    dependencies.AddRange(entityAttribute.IDs);
                }

                AllConditionalObjects.Add(type.GetMethodWith<ConditionalOnLoadAttribute>() ?? throw new InvalidOperationException("Conditional Object Missing Conditional OnLoad Method!"), [.. dependencies]);
            }
        }
    }

    internal static void InitializeHookLoader()
    {
        On.Celeste.LevelLoader.ctor += LevelLoader_ctor;
        On.Celeste.Dialog.RefreshLanguages += DialogOnRefreshLanguages;
    }

    public static void Unload()
    {
        On.Celeste.LevelLoader.ctor -= LevelLoader_ctor;
        On.Celeste.Dialog.RefreshLanguages -= DialogOnRefreshLanguages;
    }

    private static void LevelLoader_ctor(On.Celeste.LevelLoader.orig_ctor orig, LevelLoader self, Session session, Vector2? startposition)
    {
        orig(self, session, startposition);
        Utils.LogVerbose("Unloading Hooks...");
        LifecycleMethods.OnUnload();
        
        Utils.LogVerbose("Loading Basics...");
        LifecycleMethods.OnLoad();
        
        Utils.LogVerbose("Loading Custom Command Hooks...");
        LoadCommandHooks();
        
        Utils.LogVerbose("Loading Entity Hooks...");
        LoadEntityHooks(session);
    }
    
    private static void DialogOnRefreshLanguages(On.Celeste.Dialog.orig_RefreshLanguages orig)
    {
        orig();
        Utils.LogVerbose("Reloading Custom Commands...");

        CustomDialogCommands.Unload();
        foreach ((Type _, MethodInfo _, MethodInfo unloadMethod) in AllCustomCommands.Keys.Distinct())
        {
            unloadMethod.SimpleInvoke();
        }

        LoadCommandHooks();
    }

    private static void LoadCommandHooks()
    {
        List<(Type, MethodInfo)> commandsToLoad = [];
        foreach (string dialog in Dialog.Language.Dialog.Values)
        {
            foreach (((Type type, MethodInfo loadMethod, MethodInfo _), string[] commandStrings) in AllCustomCommands)
            {
                if (!commandsToLoad.Contains((type, loadMethod)) && commandStrings.Any(commandString => dialog.Contains(commandString)))
                {
                    commandsToLoad.Add((type, loadMethod));
                }
            }
        }
        
        if (commandsToLoad.Count != 0) CustomDialogCommands.Load();
        foreach ((Type _, MethodInfo loadMethod) in commandsToLoad.Distinct())
        {
            loadMethod?.SimpleInvoke();
        }
    }
    
    private static void LoadEntityHooks(Session session)
    {
        List<MethodInfo> objectsToLoad = [];
        foreach (LevelData level in session.MapData.Levels)
        {
            foreach (EntityData entity in level.Entities.Distinct())
            {
                foreach ((MethodInfo loadMethod, string[] dependecyList) in AllConditionalObjects)
                {
                    if (!objectsToLoad.Contains(loadMethod) && dependecyList.Any(dependecy => dependecy == entity.Name))
                    {
                        objectsToLoad.Add(loadMethod);
                    }
                }
            }

            foreach (EntityData trigger in level.Triggers.Distinct())
            {
                foreach ((MethodInfo loadMethod, string[] dependecyList) in AllConditionalObjects)
                {
                    if (!objectsToLoad.Contains(loadMethod) && dependecyList.Any(dependecy => dependecy == trigger.Name))
                    {
                        objectsToLoad.Add(loadMethod);
                    }
                }
            }
        }

        if (MintChocolateHelperModule.Settings.ForceUniversalAnimatedTiles)
        {
            objectsToLoad.Add(typeof(UniversalAnimatedTilesController).GetMethod(nameof(UniversalAnimatedTilesController.Load), BindingFlags.NonPublic | BindingFlags.Static));
        }

        foreach (MethodInfo loadMethod in objectsToLoad.Distinct())
        {
            loadMethod?.SimpleInvoke();
        }
    }
}