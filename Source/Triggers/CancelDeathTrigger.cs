namespace Celeste.Mod.MintChocolateHelper.Triggers;

[Tracked]
[CustomEntity("MintChocolateHelper/CancelDeathTrigger")]
public class CancelDeathTrigger : Trigger
{
    private readonly int Delay;
    internal readonly bool DisableQuickRespawn;
    internal readonly bool DontRegisterDeathInStats;
    internal readonly bool KeepFollowers;

    internal readonly string Flag;
    internal readonly object FlagExpression;
    internal readonly bool IsValidExpression;

    public CancelDeathTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Delay = data.Int("delay");
        DisableQuickRespawn = data.Bool("disableQuickRespawn");
        DontRegisterDeathInStats = data.Bool("dontRegisterDeathInStats");
        KeepFollowers = data.Bool("keepFollowers");

        Flag = data.Attr("flag");
        if (FrostHelperImports.SafeTryCreateSessionExpression(Flag, out FlagExpression))
        {
            IsValidExpression = true;
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (MintChocolateHelperModule.Session.PlayerIsPseudoDead)
        {
            Add(new Coroutine(Unkill(player, Delay)));
        }
    }

    private IEnumerator Unkill(Player player, int delay)
    {
        yield return delay / 60f;
        if (!MintChocolateHelperModule.Session.PlayerIsPseudoDead) yield break;

        Level level = player.SceneAs<Level>();
        level.Wipe?.Cancel();
        PseudoDeath.BasicUnkill(player, SearchUtils.GetEntity<PlayerDeadBody>(true));
        
        MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer = true;
        if (level.Session.RespawnPoint is { }) player.Position = level.Session.RespawnPoint.Value;
        yield return null;

        level.Wipe?.Cancel();
        player.Sprite.Scale.X = 1;
        MintChocolateHelperModule.Session.CancelDeathTriggerTeleportingPlayer = false;
    }
}