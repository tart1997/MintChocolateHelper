using System.Xml;
using Celeste.Mod.MintChocolateHelper.Registry.Components;
using Celeste.Mod.Registry.DecalRegistryHandlers;

namespace Celeste.Mod.MintChocolateHelper.Registry.Handlers;

internal class PlayerDistanceFadeRegistryHandler : DecalRegistryHandler
{
    private float InnerRadius;
    private float OuterRadius;
    private bool FadeOut;
    
    
    public override string Name => "Mint.PlayerDistanceFade";
    
    
    public override void Parse(XmlAttributeCollection xml)
    {
        InnerRadius = Get(xml, "InnerRadius",50f);
        OuterRadius = Get(xml, "OuterRadius", 80f);
        FadeOut = GetBool(xml, "FadeOut", false);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new PlayerDistanceFade(InnerRadius, OuterRadius, FadeOut));
    }
}
