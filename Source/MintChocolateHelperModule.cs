using System;
using System.Reflection;
using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.ModInterop;
using FlaglinesAndSuch;
using MonoMod.ModInterop;

namespace Celeste.Mod.MintChocolateHelper;

// ReSharper disable once ClassNeverInstantiated.Global
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
    
    
    //FEMTO HELPER
    public static bool FemtoHelperLoaded;
    
    public static FieldInfo WindPetalsFade;
    
    //FLAGLINES 
    public static bool FlaglinesLoaded;
    
    public static FieldInfo CustomGodraysFade;
    
    
    
    
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

    public override void Load() 
    {
        typeof(FrostHelperImports).ModInterop();
        StylegroundsWhilePaused.Load();
        HeartBreakerRefill.Load();
        SpeedFlipRefill.Load();
        
        //FEMTO HELPER
        EverestModuleMetadata femtoHelper = new() {
            Name = "FemtoHelper",
            Version = new Version(1, 15 ,9)
        };
        
        FemtoHelperLoaded = Everest.Loader.DependencyLoaded(femtoHelper);

        if (FemtoHelperLoaded)
        {
            WindPetalsFade = typeof(WindPetals).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        //FLAGLINES 
        EverestModuleMetadata FlaglinesAndSuch = new() {
            Name = "FlaglinesAndSuch",
            Version = new Version(1, 6 ,65)
        };
        
        FlaglinesLoaded = Everest.Loader.DependencyLoaded(FlaglinesAndSuch);

        if (FlaglinesLoaded)
        {
            CustomGodraysFade = typeof(CustomGodrays).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }

    public override void Unload() 
    {
        StylegroundsWhilePaused.Unload();
        HeartBreakerRefill.Unload();
        SpeedFlipRefill.Unload();
    }
}