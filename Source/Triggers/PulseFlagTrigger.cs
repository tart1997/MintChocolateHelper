using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/PulseFlagTrigger")]

public class PulseFlagTrigger : Trigger
{
    private readonly string Flag;

    private readonly int Frames;

    private readonly bool Invert; 
    
    public PulseFlagTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Flag = data.Attr("flag", "");
        Frames = data.Int("frames", 1);
        Invert = data.Bool("invert", false);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        Add(new Coroutine(TartsAwesomeCodeThatDoesShit()));
    }

    private IEnumerator TartsAwesomeCodeThatDoesShit()
    {
        if (Scene is not Level level) yield break;
        
        level.Session.SetFlag(Flag, !Invert);

        yield return Frames / 60f;
        
        level.Session.SetFlag(Flag, Invert);
    }
}