namespace Celeste.Mod.MintChocolateHelper.Triggers;
[CustomEntity("MintChocolateHelper/CancelDeathTrigger")]
[Tracked]

public class CancelDeathTrigger : Trigger
{
    private readonly int Delay;
    private readonly bool UnregisterDeathInStats;

    public CancelDeathTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Delay = data.Int("delay");
        UnregisterDeathInStats = data.Bool("unregisterDeathInStats");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (MintChocolateHelperModule.Session.PlayerIsPsuedoDead)
        {
            Add(new Coroutine(Unkill(Delay)));
        }
    }

    private IEnumerator Unkill(int delay)
    {
        if (Scene is not Level) yield break;
        yield return delay / 60f;

        Level level = SceneAs<Level>();
        level.Wipe?.Cancel();

        Session session = level.Session;
        Player player = level.Tracker.GetEntity<Player>();

        PlayerDeadBody playerDeadBody = Scene.Tracker.GetEntitiesTrackIfNeeded<PlayerDeadBody>().Cast<PlayerDeadBody>().FirstOrDefault();
        playerDeadBody?.hair.Entity = player;
        playerDeadBody?.sprite.Entity = player;
        playerDeadBody?.light.Entity = player;
        playerDeadBody?.RemoveSelf();
        

        if (UnregisterDeathInStats)
        {
            --session.Deaths;
            --session.DeathsInCurrentLevel;
            --SaveData.Instance.TotalDeaths;
            --SaveData.Instance.Areas_Safe[session.Area.ID].Modes[(int) session.Area.Mode].Deaths;
            Stats.Increment(Stat.DEATHS, -1);
            StatsForStadia.Increment(StadiaStat.DEATHS, -1);
        }

        player.Dead = false;
        player.Depth = MintChocolateHelperModule.Session.CDT_Depth;
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        player.Collidable = MintChocolateHelperModule.Session.CDT_Collidable;
        player.Visible = MintChocolateHelperModule.Session.CDT_Visible;
        if (Scene is not null) player.Scene = Scene;
        MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;

        Debug.Assert(level.Session.RespawnPoint != null);
        player.Position = level.Session.RespawnPoint.Value;

        //This kinda sucks... I would prefer to just kill whatever rouge tweener that forces me to do this, but I've tried everything I can think of to do so. ¯\_(ツ)_/¯

        yield return null;
        player.Sprite.Scale.X = 1;
    }
}