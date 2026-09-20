namespace Celeste.Mod.MintChocolateHelper.Triggers;

[Tracked]
[CustomEntity("MintChocolateHelper/CancelDeathTrigger")]
public class CancelDeathTrigger : Trigger
{
    private readonly int Delay;
    internal readonly bool DisableQuickRespawn;
    internal readonly bool SkipEverestEventOnDie;
    internal readonly bool AffectRetries;
    internal readonly bool DontRegisterDeathInStats;
    internal readonly bool KeepFollowers;

    internal readonly string Flag;
    internal readonly object FlagExpression;
    internal readonly bool IsValidExpression;

    public CancelDeathTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Delay = data.Int("delay");
        DisableQuickRespawn = data.Bool("disableQuickRespawn");
        SkipEverestEventOnDie = data.Bool("skipEverestEventOnDie");
        AffectRetries = data.Bool("affectRetries");
        DontRegisterDeathInStats = data.Bool("dontRegisterDeathInStats");
        KeepFollowers = data.Bool("keepFollowers");

        Flag = data.Attr("flag");
        if (FrostHelperImports.SafeTryCreateSessionExpression(Flag, out FlagExpression)) IsValidExpression = true;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (MintChocolateHelperModule.Session.PlayerIsPseudoDead) Add(new Coroutine(Unkill(Delay)));
    }

    private static IEnumerator Unkill(int delay)
    {
        yield return delay / 60f;
        if (!MintChocolateHelperModule.Session.PlayerIsPseudoDead) yield break;
        MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer = true;
    }
}