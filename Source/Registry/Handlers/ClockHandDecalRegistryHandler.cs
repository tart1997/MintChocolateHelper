using System.Xml;
using Celeste.Mod.MintChocolateHelper.Registry.Components;
using Celeste.Mod.Registry.DecalRegistryHandlers;

namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

internal class ClockHandDecalRegistryHandler : DecalRegistryHandler
{
    private bool AlwaysUpdate;
    private bool RandomStart;
    private bool Backwards;

    private int StopNumber;
    private float TickSpeed;
    private float TickDelay;
    
    private string AllowTickFlag;
    
    private string EasingFunctionString;
    
    
    public override string Name => "mint.clockHand";
    
    
    public override void Parse(XmlAttributeCollection xml)
    {
        AlwaysUpdate = GetBool(xml, "alwaysUpdate", false);
        RandomStart = GetBool(xml, "randomStart", false);
        Backwards = GetBool(xml, "backwards", false);
        
        StopNumber = Get(xml, "stopNumber", 12);
        TickSpeed = Get(xml, "tickSpeed", 1f);
        TickDelay = Get(xml, "delay", 0.1f);
        
        AllowTickFlag = GetString(xml, "allowTickFlag", "");
        
        EasingFunctionString = GetString(xml, "easingFunction", "");
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new ClockHand(AlwaysUpdate, RandomStart, Backwards, StopNumber, TickSpeed, TickDelay, AllowTickFlag, EasingFunctionString));
    }
}
