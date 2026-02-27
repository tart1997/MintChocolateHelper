using System;
using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.ModInterop;
using MonoMod.ModInterop;

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModule : EverestModule 
{
    public static MintChocolateHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(MintChocolateHelperModuleSettings);
    public static MintChocolateHelperModuleSettings Settings => (MintChocolateHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(MintChocolateHelperModuleSession);
    public static MintChocolateHelperModuleSession Session => (MintChocolateHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(MintChocolateHelperModuleSaveData);
    public static MintChocolateHelperModuleSaveData SaveData => (MintChocolateHelperModuleSaveData) Instance._SaveData;
    
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
    }

    public override void Unload() 
    {
        StylegroundsWhilePaused.Unload();
        HeartBreakerRefill.Unload();
    }
}