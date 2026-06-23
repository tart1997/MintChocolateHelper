using System.Linq;
using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.Triggers;

namespace Celeste.Mod.MintChocolateHelper.Extras;

public class CheckIfShitExists
{
    internal static void Load()
    {
        On.Celeste.Level.Update += CheckForShit;
    }

    internal static void Unload()
    {
        On.Celeste.Level.Update += CheckForShit;
    }

    private static void CheckForShit(On.Celeste.Level.orig_Update orig, Level level)
    {
        MintChocolateHelperModule.Session.DebrisTweaksWindAffectedControllerExists = level.Tracker.GetEntities<DebrisTweaksController>().Cast<DebrisTweaksController>().Any(ADFController => ADFController.WindAffected);
        MintChocolateHelperModule.Session.DebrisTweaksAlternateFadeoutControllerExists = level.Tracker.GetEntities<DebrisTweaksController>().Cast<DebrisTweaksController>().Any(ADFController => ADFController.AlternateFadeout);
        DisableQuickRespawn DQRController = level.Tracker.GetEntity<DisableQuickRespawn>();
        MintChocolateHelperModule.Session.DisableQuickRepawnControllerExists = (DQRController != null, DQRController);
        StylegroundsWhilePaused SWPController = level.Tracker.GetEntity<StylegroundsWhilePaused>();
        MintChocolateHelperModule.Session.StylegroundsWhilePausedControllerExists = (SWPController != null, SWPController);
        MintChocolateHelperModule.Session.CancelDeathTriggerExists = level.Tracker.GetEntity<CancelDeathTrigger>() != null;

        orig(level);
    }
}