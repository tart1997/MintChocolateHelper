using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.ModInterop;
using Celeste.Mod.MintChocolateHelper.Registry.Handlers;
using Celeste.Mod.MintChocolateHelper.Triggers;
using FlaglinesAndSuch;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using VivHelper.Effects;

namespace Celeste.Mod.MintChocolateHelper;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public class MintChocolateHelperModule : EverestModule 
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static MintChocolateHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(MintChocolateHelperModuleSettings);
    public static MintChocolateHelperModuleSettings Settings => (MintChocolateHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(MintChocolateHelperModuleSession);
    public static MintChocolateHelperModuleSession Session => (MintChocolateHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(MintChocolateHelperModuleSaveData);
    public static MintChocolateHelperModuleSaveData SaveData => (MintChocolateHelperModuleSaveData) Instance._SaveData;
    
    
    //<----FEMTO HELPER---->
    public static bool FemtoHelperLoaded;
    
    public static FieldInfo WindPetalsFade;
    
    //<----FLAGLINES---->
    public static bool FlaglinesLoaded;
    
    public static FieldInfo CustomGodraysFade;
    
    //<----VIVHELPER---->
    public static bool VivhelperLoaded;
    
    public static FieldInfo VivCustomRainFade;
    
    //<----COMMUNALHELPER---->
    public static bool CommunalHelperLoaded;

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
    public static ILHook hook_origDie;
    
    
    public override void Load() 
    {
        typeof(FrostHelperImports).ModInterop();
        StylegroundsWhilePaused.Load();
        HeartBreakerRefill.Load();
        SpeedFlipRefill.Load();
        DisableQuickRespawn.Load();
        AlternateDebrisFade.Load();
        CancelDeathTrigger.Load();
        
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
        
        DecalRegistry.AddPropertyHandler<ClockHandDecalRegistryHandler>();
        DecalRegistry.AddPropertyHandler<PlayerDistanceFadeRegistryHandler>();
    }
    
    public static void loadFemtoHelper()
    {
        WindPetalsFade = typeof(WindPetals).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static void loadFlaglines()
    {
        CustomGodraysFade = typeof(CustomGodrays).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static void loadVivHelper()
    {
        VivCustomRainFade = typeof(CustomRain).GetField("visibleFade", BindingFlags.NonPublic | BindingFlags.Instance);
    }
    
    public override void Unload() 
    {
        StylegroundsWhilePaused.Unload();
        HeartBreakerRefill.Unload();
        SpeedFlipRefill.Unload();
        DisableQuickRespawn.Unload();
        AlternateDebrisFade.Unload();
        CancelDeathTrigger.Unload();
    }
}