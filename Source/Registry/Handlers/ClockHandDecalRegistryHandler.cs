using System.Xml;
using Celeste.Mod.MintChocolateHelper.Registry.Components;
using Celeste.Mod.Registry.DecalRegistryHandlers;

namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

internal class ClockHandDecalRegistryHandler : DecalRegistryHandler
{
    private bool Persistent;
    private bool AlwaysUpdate;
    private bool RandomStart;
    private bool Backwards;

    private int StopNumber;
    private float TickSpeed;
    private float Delay;
    
    private string TickableFlag;
    
    
    public override string Name => "Mint.ClockHand";
    
    
    public override void Parse(XmlAttributeCollection xml)
    {
        Persistent = GetBool(xml, "Persistent", false);
        AlwaysUpdate = GetBool(xml, "AlwaysUpdate", false);
        RandomStart = GetBool(xml, "RandomStart", false);
        Backwards = GetBool(xml, "Backwards", false);
        
        StopNumber = Get(xml, "StopNumber", 12);
        TickSpeed = Get(xml, "TickSpeed", 1);
        Delay = Get(xml, "Delay", 0.1f);
        
        TickableFlag = GetString(xml, "TickableFlag", "");
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new ClockHand(Persistent, AlwaysUpdate, RandomStart, Backwards, StopNumber, TickSpeed, Delay, TickableFlag));
    }
}
