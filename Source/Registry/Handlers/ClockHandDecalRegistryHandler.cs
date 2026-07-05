namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

internal class ClockHandDecalRegistryHandler : DecalRegistryHandler
{
    public override string Name => "mint.clockHand";

    private bool AlwaysUpdate;
    private bool RandomStart;
    private bool Backwards;

    private int StopNumber;
    private float TickSpeed;
    private float TickDelay;
    private string AllowTickFlag;

    private EasingFunctions.EasingFunction easingFunction;
    
    [OnLoad]
    internal static void Load()
    {
        DecalRegistry.AddPropertyHandler<ClockHandDecalRegistryHandler>();
    }

    public override void Parse(XmlAttributeCollection xml)
    {
        AlwaysUpdate = GetBool(xml, "alwaysUpdate", false);
        RandomStart = GetBool(xml, "randomStart", false);
        Backwards = GetBool(xml, "backwards", false);

        StopNumber = Get(xml, "stopNumber", 12);
        TickSpeed = Get(xml, "tickSpeed", 1f);
        TickDelay = Get(xml, "delay", 0.1f);
        AllowTickFlag = GetString(xml, "allowTickFlag", "");

        easingFunction = xml.GetEnum("easingFunction", EasingFunctions.EasingFunction.Linear);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new ClockHand(AlwaysUpdate, RandomStart, Backwards, StopNumber, TickSpeed, TickDelay, AllowTickFlag, easingFunction));
    }
}