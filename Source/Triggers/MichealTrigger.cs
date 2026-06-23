using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.MintChocolateHelper.Triggers;
[CustomEntity("MintChocolateHelper/MichealTrigger")]

public class MichealTrigger : Trigger
{
    public MichealTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Audio.Play("event:/MintChocolateHelper/Michael");
    }
}