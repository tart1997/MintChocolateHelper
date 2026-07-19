namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession
{
    // Heart Breaker Refill
    internal bool HasHeartBreakerDash {get; set;}
    internal bool HeartBreakerDashActive {get; set;}

    // Speed Flip Refill
    internal bool HasSpeedFlipRefill {get; set;}
    internal bool DontRenderSpeedFlipRefillIcon {get; set;}

    // Psuedo Death
    internal bool PlayerIsPsuedoDead {get; set;}
    internal int DepthBeforePsuedoDeath {get; set;}
    internal bool WasCollidableBeforePsuedoDeath {get; set;}
    internal bool WasVisibleBeforePsuedoDeath {get; set;}

    // Jesus Refill
    internal bool HasJesusRefill {get; set;}
    internal bool JesusRefillDisableQuickRespawn {get; set;}

    // Cancel Death Trigger
    internal bool PsuedoDeathTeleportingPlayer {get; set;}
}