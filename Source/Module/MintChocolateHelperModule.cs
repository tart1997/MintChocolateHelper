namespace Celeste.Mod.MintChocolateHelper.Module;

public class MintChocolateHelperModule : EverestModule
{
    internal static string ModName => "Mint Chocolate Helper";

    [UsedImplicitly]
    public static MintChocolateHelperModule Instance {get; set;}

    public override Type SettingsType => typeof(MintChocolateHelperModuleSettings);
    public static MintChocolateHelperModuleSettings Settings => (MintChocolateHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(MintChocolateHelperModuleSession);
    public static MintChocolateHelperModuleSession Session => (MintChocolateHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(MintChocolateHelperModuleSaveData);
    public static MintChocolateHelperModuleSaveData SaveData => (MintChocolateHelperModuleSaveData)Instance._SaveData;

    public MintChocolateHelperModule()
    {
        Instance = this;
        #if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(ModName, LogLevel.Verbose);
        #else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(MintChocolateHelperModule), LogLevel.Info);
        #endif
    }

    // OPTIONAL DEPENDENCIES GO HERE
    internal static bool FemtoHelperLoaded;
    internal static bool FlaglinesLoaded;
    internal static bool VivhelperLoaded;

    public override void Load()
    {
        Utils.LogVerbose("Running TryDisableInlining...");
        MintChocolateHelperHookLoading.TryDisableInlining();
        
        Utils.LogVerbose("Loading Optional Dependencies...");
        #region Optional Dependency Loading
            EverestModuleMetadata femtoHelper = new() {
                Name = "FemtoHelper",
                Version = new Version(1, 15, 9)
            };
            FemtoHelperLoaded = Everest.Loader.DependencyLoaded(femtoHelper);

            EverestModuleMetadata FlaglinesAndSuch = new() {
                Name = "FlaglinesAndSuch",
                Version = new Version(1, 6, 65)
            };
            FlaglinesLoaded = Everest.Loader.DependencyLoaded(FlaglinesAndSuch);

            EverestModuleMetadata VivHelper = new() {
                Name = "VivHelper",
                Version = new Version(1, 14, 10)
            };
            VivhelperLoaded = Everest.Loader.DependencyLoaded(VivHelper);
        #endregion

        Utils.LogVerbose("Loading ModInterop...");
        FrostHelperImports.Load();
        GravityHelperImports.Load();

        if (Everest.Flags.IsHeadless)
        {
            Utils.LogDebug("Celeste Is Headless! Loading All Hooks Unconditionally...");
            MintChocolateHelperHookLoading.LoadAllHooks();
            return;
        }

        Utils.LogVerbose("Finding Conditional Objects...");
        MintChocolateHelperHookLoading.LoadAllConditionalObjects();
        
        Utils.LogDebug("Loading HookLoaders...");
        MintChocolateHelperHookLoading.InitializeHookLoader();
    }

    public override void Unload()
    {
        Utils.LogVerbose("Unloading MintChocolateHelper...");
        LifecycleMethods.OnUnload();
        MintChocolateHelperHookLoading.Unload();
    }
}