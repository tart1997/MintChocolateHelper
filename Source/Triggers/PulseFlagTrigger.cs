namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/PulseFlagTrigger")]
[UsedImplicitly]
public class PulseFlagTrigger : Trigger
{
    private readonly string Flag;
    private readonly int Frames;
    private readonly bool Invert;

    public PulseFlagTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Flag = data.Attr("flag");
        Frames = data.Int("frames", 1);
        Invert = data.Bool("invert");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Add(new Coroutine(Pulse()));
    }

    private IEnumerator Pulse()
    {
        if (Utils.LevelIsNotSafe(out Level level)) yield break;

        level.Session.SetFlag(Flag, !Invert);
        yield return Frames / 60f;
        level.Session.SetFlag(Flag, Invert);
    }
}