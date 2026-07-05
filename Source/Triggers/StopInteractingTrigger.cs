namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/StopInteractingTrigger")]
public class StopInteractingTrigger : Trigger
{
    public StopInteractingTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (Scene is not Level level) return;

        foreach (Lookout lookout in level.Tracker.GetEntitiesTrackIfNeeded<Lookout>().Cast<Lookout>())
        {
            lookout.StopInteracting();
        }
    }
}