using System;
using System.Reflection;
using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.Extras;
using Celeste.Mod.MintChocolateHelper.ModInterop;
using Celeste.Mod.MintChocolateHelper.Registry.Handlers;
using FlaglinesAndSuch;
using MonoMod.RuntimeDetour;
using VivHelper.Effects;

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModule : EverestModule 
{
    private static MintChocolateHelperModule Instance { get; set; }

    public override Type SettingsType => typeof(MintChocolateHelperModuleSettings);
    public static MintChocolateHelperModuleSettings Settings => (MintChocolateHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(MintChocolateHelperModuleSession);
    public static MintChocolateHelperModuleSession Session => (MintChocolateHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(MintChocolateHelperModuleSaveData);
    public static MintChocolateHelperModuleSaveData SaveData => (MintChocolateHelperModuleSaveData) Instance._SaveData;

    #region Snappy Styleground Controller Bullshit
        internal static bool FemtoHelperLoaded;
        internal static FieldInfo WindPetalsFade;

        internal static bool FlaglinesLoaded;
        internal static FieldInfo CustomGodraysFade;

        internal static bool VivhelperLoaded;
        internal static FieldInfo VivCustomRainFade;
    #endregion

    public MintChocolateHelperModule()
    {
        Instance = this;
        #if DEBUG
                // debug builds use verbose logging
                Logger.SetLogLevel(nameof(MintChocolateHelperModule), LogLevel.Verbose);
        #else
                // release builds use info logging to reduce spam in log files
                Logger.SetLogLevel(nameof(MintChocolateHelperModule), LogLevel.Info);
        #endif
    }

    // MANUAL HOOKS GO HERE
    public static ILHook FakeDeathHook_origDie;

    public override void Load() 
    {
        FrostHelperImports.Load();
        CheckIfShitExists.Load();
        StylegroundsWhilePaused.Load();
        HeartBreakerRefill.Load();
        SpeedFlipRefill.Load();
        DisableQuickRespawn.Load();
        DebrisTweaksController.Load();
        FakeDeath.Load();
        JesusRefill.Load();

        DecalRegistry.AddPropertyHandler<ClockHandDecalRegistryHandler>();
        DecalRegistry.AddPropertyHandler<PlayerDistanceFadeRegistryHandler>();

        #region More Snappy Styleground Controller Bullshit
            //<----FEMTO HELPER---->
            EverestModuleMetadata femtoHelper = new() {
                Name = "FemtoHelper",
                Version = new Version(1, 15 ,9)
            };

            FemtoHelperLoaded = Everest.Loader.DependencyLoaded(femtoHelper);

            if (FemtoHelperLoaded)
            {
                loadFemtoHelper();
            }

            //<----FLAGLINES---->
            EverestModuleMetadata FlaglinesAndSuch = new() {
                Name = "FlaglinesAndSuch",
                Version = new Version(1, 6 ,65)
            };

            FlaglinesLoaded = Everest.Loader.DependencyLoaded(FlaglinesAndSuch);

            if (FlaglinesLoaded)
            {
                loadFlaglines();
            }

            //<----VIVHELPER---->
            EverestModuleMetadata VivHelper = new() {
                Name = "VivHelper",
                Version = new Version(1, 14 ,10)
            };

            VivhelperLoaded = Everest.Loader.DependencyLoaded(VivHelper);

            if (VivhelperLoaded)
            {
                loadVivHelper();
            }
        #endregion
    }

    #region Even More Snappy Styleground Controller Bullshit
        private static void loadFemtoHelper()
        {
            WindPetalsFade = typeof(WindPetals).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void loadFlaglines()
        {
            CustomGodraysFade = typeof(CustomGodrays).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void loadVivHelper()
        {
            VivCustomRainFade = typeof(CustomRain).GetField("visibleFade", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    #endregion

    public override void Unload() 
    {
        CheckIfShitExists.Unload();
        StylegroundsWhilePaused.Unload();
        HeartBreakerRefill.Unload();
        SpeedFlipRefill.Unload();
        DisableQuickRespawn.Unload();
        DebrisTweaksController.Unload();
        FakeDeath.Unload();
        JesusRefill.Unload();
    }
}