// ReSharper disable MemberCanBePrivate.Global

namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession
{
    // Pseudo Death
    internal bool PlayerIsPseudoDead {get; set;}
    internal bool PseudoDeadDisableQuickRespawn => JesusRefillDisableQuickRespawn || CancelDeathTriggerDisableQuickRespawn;
    internal bool PseudoDeadDontRegisterDeathInStats => JesusRefillDontRegisterDeathInStats || CancelDeathTriggerDontRegisterDeathInStats;
    internal bool PseudoDeadKeepFollowers => JesusRefillKeepFollowers || CancelDeathTriggerKeepFollowers;
    internal int DepthBeforePseudoDeath {get; set;}
    internal bool WasCollidableBeforePseudoDeath {get; set;}
    internal bool WasVisibleBeforePseudoDeath {get; set;}

    // Cancel Death Trigger
    internal static bool CancelDeathTriggerDisableQuickRespawn => SearchUtils.GetEntities<CancelDeathTrigger>()?.Any(t => t.DisableQuickRespawn) ?? false;
    internal static bool CancelDeathTriggerDontRegisterDeathInStats => SearchUtils.GetEntities<CancelDeathTrigger>()?.Any(t => t.DontRegisterDeathInStats) ?? false;
    internal static bool CancelDeathTriggerKeepFollowers => SearchUtils.GetEntities<CancelDeathTrigger>()?.Any(t => t.KeepFollowers) ?? false;
    internal bool CancelDeathTriggerTeleportingPlayer {get; set;}

    // Jesus Refill
    [CanBeNull]
    internal JesusRefill LastJesusRefill {get; set;}
    internal bool JesusRefillBufferedTeleport {get; set;}
    internal bool HasJesusRefill => LastJesusRefill is { };
    internal bool JesusRefillDisableQuickRespawn => LastJesusRefill is { DisableQuickRespawn: true };
    internal bool JesusRefillDontRegisterDeathInStats => LastJesusRefill is { DontRegisterDeathInStats: true };
    internal bool JesusRefillKeepFollowers => LastJesusRefill is { KeepFollowers: true };
    internal bool JesusRefillTeleportToRefill => LastJesusRefill is { TeleportToRefill: true };
    internal bool JesusRefillStoreSpeed => LastJesusRefill is { StoreSpeed: true };
    internal bool JesusRefillRedirectable => LastJesusRefill is { Redirectable: true };
    internal Vector2? StoredSpeed {get; set;}

    // Speed Flip Refill
    internal bool HasSpeedFlipRefill {get; set;}
    internal bool DontRenderSpeedFlipRefillIcon {get; set;}

    // Heart Breaker Refill
    internal bool HasHeartBreakerDash {get; set;}
    internal bool HeartBreakerDashActive {get; set;}
}