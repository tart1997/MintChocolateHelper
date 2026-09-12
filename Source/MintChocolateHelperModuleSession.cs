using Celeste.Mod.MintChocolateHelper.Entities;

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession
{
    // Heart Breaker Refill
    internal bool HasHeartBreakerDash {get; set;}
    internal bool HeartBreakerDashActive {get; set;}

    // Speed Flip Refill
    internal bool HasSpeedFlipRefill {get; set;}
    internal bool DontRenderSpeedFlipRefillIcon {get; set;}

    // Jesus Refill
    internal JesusRefill LastJesusRefill {get; set;}
    internal bool HasJesusRefill {get; set;}
    internal bool TeleportToRefill {get; set;}
    internal bool Redirectable {get; set;}
    internal bool StoreSpeed {get; set;}
    internal Vector2? StoredSpeed {get; set;}
    internal bool JesusRefillDisableQuickRespawn {get; set;}

    // Cancel Death Trigger
    internal bool CancelDeathTriggerTeleportingPlayer {get; set;}

    // Psuedo Death
    internal bool PlayerIsPsuedoDead {get; set;}
    internal bool PsuedoDeadKeepFollowers {get; set;}
    internal int DepthBeforePsuedoDeath {get; set;}
    internal bool WasCollidableBeforePsuedoDeath {get; set;}
    internal bool WasVisibleBeforePsuedoDeath {get; set;}
}