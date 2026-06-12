using System.Xml;
using Celeste.Mod.MintChocolateHelper.Registry.Components;
using Celeste.Mod.Registry.DecalRegistryHandlers;

namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

internal class PlayerDistanceFadeRegistryHandler : DecalRegistryHandler
{
    public override string Name => "mint.playerDistanceFade";
    
    private float InnerRadius;
    private float OuterRadius;
    private bool FadeOut;
    
    public override void Parse(XmlAttributeCollection xml)
    {
        InnerRadius = Get(xml, "innerRadius",50f);
        OuterRadius = Get(xml, "outerRadius", 80f);
        FadeOut = GetBool(xml, "fadeOut", false);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new PlayerDistanceFade(InnerRadius, OuterRadius, FadeOut));
    }
}
