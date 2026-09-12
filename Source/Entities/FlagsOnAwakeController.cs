namespace Celeste.Mod.MintChocolateHelper.Entities;

[UsedImplicitly]
[CustomEntity("MintChocolateHelper/FlagsOnAwake", "MintChocolateHelper/FlagsOnAwakeController")]
public class FlagsOnAwakeController : Entity
{
    private readonly string[] Flags;
    private readonly bool Value;

    public FlagsOnAwakeController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Flags = data.Attr("flags", "Flag1").Split(',');
        Value = data.Bool("value", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        foreach (string flag in Flags)
        {
            scene.SetFlag(flag, Value);
        }
    }
}