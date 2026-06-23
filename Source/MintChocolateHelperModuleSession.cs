using Celeste.Mod.MintChocolateHelper.Entities;

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession 
{
    //Heart Breaker Refill
    internal bool HasHeartBreakerDash {get; set;}
    internal bool HeartBreakerDashActive {get; set;}


    //SpeedFlipRefill
    internal bool HasSpeedFlipRefill {get; set;}


    //CancelDeathTrigger
    internal bool PlayerIsPsuedoDead {get; set;}
    internal int CDT_Depth {get; set;}
    internal bool CDT_Active {get; set;}
    internal bool CDT_Collidable {get; set;}
    internal bool CDT_Visible {get; set;}

    //Controller Existence Markers
    internal bool DebrisTweaksWindAffectedControllerExists {get; set;}
    internal bool DebrisTweaksAlternateFadeoutControllerExists {get; set;}
    internal (bool, DisableQuickRespawn) DisableQuickRepawnControllerExists {get; set;}
    internal (bool, StylegroundsWhilePaused) StylegroundsWhilePausedControllerExists {get; set;}
    internal bool CancelDeathTriggerExists {get; set;}
}