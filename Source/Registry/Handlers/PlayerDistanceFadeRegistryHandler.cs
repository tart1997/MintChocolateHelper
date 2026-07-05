namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

public class PlayerDistanceFadeRegistryHandler : DecalRegistryHandler
{
    public override string Name => "mint.playerDistanceFade";

    public enum DeathBehaivor
    {
        staySame,
        snapTo,
        fadeOut
    }

    private float InnerRadius;
    private float OuterRadius;
    private bool FadeOut;
    private DeathBehaivor deathBehaivor;
    private float DeathFadeSpeedMultiplier;
    
    [OnLoad]
    internal static void Load()
    {
        DecalRegistry.AddPropertyHandler<PlayerDistanceFadeRegistryHandler>();
    }

    public override void Parse(XmlAttributeCollection xml)
    {
        InnerRadius = Get(xml, "innerRadius", 50f);
        OuterRadius = Get(xml, "outerRadius", 80f);
        FadeOut = GetBool(xml, "fadeOut", false);
        deathBehaivor = xml.GetEnum("deathBehaivor", DeathBehaivor.staySame);
        DeathFadeSpeedMultiplier = Get(xml, "deathFadeSpeedMultiplier", 1f);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new PlayerDistanceFade(InnerRadius, OuterRadius, FadeOut, deathBehaivor, DeathFadeSpeedMultiplier));
    }
}