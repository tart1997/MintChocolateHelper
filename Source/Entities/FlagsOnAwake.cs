using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MintChocolateHelper.Entities;
[CustomEntity("MintChocolateHelper/FlagsOnAwake")]

public class FlagsOnAwake : Entity
{
    private readonly string[] Flags;
    private readonly bool Value;

    public FlagsOnAwake(EntityData data, Vector2 offset) : base( data.Position + offset)
    {
        Flags = data.Attr("flags", "Flag1").Split(',');
        Value = data.Bool("value", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (scene is not Level level) return;

        foreach (string flag in Flags)
        {
            level.Session.SetFlag(flag, Value);
        }
    }
}