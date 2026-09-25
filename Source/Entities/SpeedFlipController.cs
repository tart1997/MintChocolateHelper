namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/SpeedFlipController")]
public class SpeedFlipController : Entity
{
    internal readonly float ExtraMultiplier;

    public SpeedFlipController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        ExtraMultiplier = data.Float("extraMultiplier", 1.03f);
    }
}