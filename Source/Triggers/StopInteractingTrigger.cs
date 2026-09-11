namespace Celeste.Mod.MintChocolateHelper.Triggers;

[UsedImplicitly]
[CustomEntity("MintChocolateHelper/StopInteractingTrigger")]
public class StopInteractingTrigger : Trigger
{
    public StopInteractingTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        foreach (Lookout lookout in SearchUtils.GetEntities<Lookout>())
        {
            lookout.StopInteracting();
        }
    }
}