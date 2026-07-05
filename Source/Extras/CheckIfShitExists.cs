namespace Celeste.Mod.MintChocolateHelper.Extras;

public class CheckIfShitExists
{
    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Level.Update += CheckForShit;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Level.Update -= CheckForShit;
    }

    private static void CheckForShit(On.Celeste.Level.orig_Update orig, Level level)
    {
        foreach (DebrisTweaksController DTController in level.Tracker.GetEntities<DebrisTweaksController>().Cast<DebrisTweaksController>())
        {
            MintChocolateHelperModule.Session.DebrisTweaksWindAffectedControllerGetter = DTController.WindAffected ? (DTController, true) : (null, false);
            if (MintChocolateHelperModule.Session.DebrisTweaksWindAffectedControllerGetter.Exists)
            {
                break;
            }
        }
        
        foreach (DebrisTweaksController DTController in level.Tracker.GetEntities<DebrisTweaksController>().Cast<DebrisTweaksController>())
        {
            MintChocolateHelperModule.Session.DebrisTweaksAlternateFadeoutControllerGetter = DTController.AlternateFadeout ? (DTController, true) : (null, false);
            if (MintChocolateHelperModule.Session.DebrisTweaksAlternateFadeoutControllerGetter.Exists)
            {
                break;
            }
        }
        
        DisableQuickRespawn DQRController = level.Tracker.GetEntity<DisableQuickRespawn>();
        MintChocolateHelperModule.Session.DisableQuickRepawnControllerGetter = (DQRController, DQRController != null);
        
        StylegroundsWhilePaused SWPController = level.Tracker.GetEntity<StylegroundsWhilePaused>();
        MintChocolateHelperModule.Session.StylegroundsWhilePausedControllerGetter = (SWPController, SWPController != null);

        CancelDeathTrigger CDTrigger = level.Tracker.GetEntity<CancelDeathTrigger>();
        MintChocolateHelperModule.Session.CancelDeathTriggerGetter = (CDTrigger, CDTrigger != null);

        orig(level);
    }
}