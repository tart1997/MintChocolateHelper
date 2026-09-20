// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Celeste.Mod.MintChocolateHelper;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class MintChocolateHelperModuleSession : EverestModuleSession
{
    // Pseudo Death
    internal bool PseudoDeadDisableQuickRespawn => JesusRefillDisableQuickRespawn || CancelDeathTriggerDisableQuickRespawn;
    internal bool PseudoDeadSkipEverestEventOnDie => JesusRefillSkipEverestEventOnDie || CancelDeathTriggerSkipEverestEventOnDie;
    internal bool PseudoDeadAffectRetries => JesusRefillAffectRetries || CancelDeathTriggerAffectRetries;
    internal bool PseudoDeadDontRegisterDeathInStats => JesusRefillDontRegisterDeathInStats || CancelDeathTriggerDontRegisterDeathInStats;
    internal bool PseudoDeadKeepFollowers => JesusRefillKeepFollowers || CancelDeathTriggerKeepFollowers;
    internal bool PlayerCanPseudoDie => HasJesusRefill || ValidCancelDeathTriggers?.Count != 0;
    internal bool PlayerIsPseudoDead {get; set;}

    // Cancel Death Trigger
    [CanBeNull]
    internal List<CancelDeathTrigger> ValidCancelDeathTriggers => SearchUtils.GetEntities<CancelDeathTrigger>()?.Where(t =>
        string.IsNullOrWhiteSpace(t.Flag) || (t.IsValidExpression ? FrostHelperImports.SafeGetBoolSessionExpressionValue(t.FlagExpression, Utils.GetLevel()!.Session) : Utils.GetLevel().GetFlag(t.Flag))).ToList();
    internal bool CancelDeathTriggerDisableQuickRespawn => ValidCancelDeathTriggers?.Any(t => t.DisableQuickRespawn) ?? false;
    internal bool CancelDeathTriggerSkipEverestEventOnDie => ValidCancelDeathTriggers?.Any(t => t.SkipEverestEventOnDie) ?? false;
    internal bool CancelDeathTriggerAffectRetries => ValidCancelDeathTriggers?.Any(t => t.AffectRetries) ?? false;
    internal bool CancelDeathTriggerDontRegisterDeathInStats => ValidCancelDeathTriggers?.Any(t => t.DontRegisterDeathInStats) ?? false;
    internal bool CancelDeathTriggerKeepFollowers => ValidCancelDeathTriggers?.Any(t => t.KeepFollowers) ?? false;
    internal bool CancelDeathTriggerTeleportingPlayer {get; set;}

    // Jesus Refill
    [CanBeNull]
    internal JesusRefill LastJesusRefill {get; set;}
    internal bool HasJesusRefill => LastJesusRefill is { };
    internal bool JesusRefillDisableQuickRespawn => LastJesusRefill is { DisableQuickRespawn: true };
    internal bool JesusRefillSkipEverestEventOnDie => LastJesusRefill is { SkipEverestEventOnDie: true };
    internal bool JesusRefillAffectRetries => LastJesusRefill is { AffectRetries: true };
    internal bool JesusRefillDontRegisterDeathInStats => LastJesusRefill is { DontRegisterDeathInStats: true };
    internal bool JesusRefillKeepFollowers => LastJesusRefill is { KeepFollowers: true };
    internal bool JesusRefillTeleportToRefill => LastJesusRefill is { TeleportToRefill: true };
    internal bool JesusRefillStoreSpeed => LastJesusRefill is { StoreSpeed: true };
    internal Vector2 StoredSpeed => LastJesusRefill?.StoredSpeed ?? Vector2.Zero;
    internal bool JesusRefillRedirectable => LastJesusRefill is { Redirectable: true };

    // Speed Flip
    [CanBeNull]
    internal SpeedFlipRefill LastSpeedFlipRefill {get; set;}
    internal int SpeedFlipControllerCharges {get; set;}
    internal bool HasSpeedFlipRefill => LastSpeedFlipRefill is { };
    internal bool SpeedFlipRefillDisableCollectEffects => LastSpeedFlipRefill is { DisableCollectEffects: true };

    // Heart Breaker Refill
    [CanBeNull]
    internal HeartBreakerRefill LastHeartBreakerRefill {get; set;}
    internal bool HasHeartBreakerDash => LastHeartBreakerRefill is { };
    internal bool HeartBreakerDashActive => HasHeartBreakerDash && (SearchUtils.GetPlayer()?.DashAttacking ?? false);
}