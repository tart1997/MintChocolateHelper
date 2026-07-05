namespace Celeste.Mod.MintChocolateHelper.Entities;
[CustomEntity("MintChocolateHelper/FusionTarget")]
[Tracked]

public class FusionTarget : Entity
{
    public FusionTarget(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(16f, 16f, -8f, -8f);
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.HollowRect(Collider,Color.DarkBlue);
    }
}