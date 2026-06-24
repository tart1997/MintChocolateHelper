using Celeste.Mod.MintChocolateHelper.Entities;
using Celeste.Mod.MintChocolateHelper.Triggers;

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession 
{
    //Heart Breaker Refill
    internal bool HasHeartBreakerDash {get; set;}
    internal bool HeartBreakerDashActive {get; set;}


    //Speed Flip Refill
    internal bool HasSpeedFlipRefill {get; set;}


    //Jesus Refill
    internal bool HasJesusRefill {get; set;}


    //Cancel Death Trigger
    internal bool PlayerIsPsuedoDead {get; set;}
    internal int CDT_Depth {get; set;}
    internal bool CDT_Collidable {get; set;}
    internal bool CDT_Visible {get; set;}

    //Controller Getters
    internal (DebrisTweaksController DTWAController, bool Exists) DebrisTweaksWindAffectedControllerGetter {get; set;}
    internal (DebrisTweaksController DTAFController, bool Exists) DebrisTweaksAlternateFadeoutControllerGetter {get; set;}
    internal (DisableQuickRespawn DQRController, bool Exists) DisableQuickRepawnControllerGetter {get; set;}
    internal (StylegroundsWhilePaused SWPController, bool Exists) StylegroundsWhilePausedControllerGetter {get; set;}
    internal (CancelDeathTrigger CDTrigger, bool Exists) CancelDeathTriggerGetter {get; set;}
}