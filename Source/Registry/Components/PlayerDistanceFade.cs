using System;
using Microsoft.Xna.Framework;
using Monocle;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Celeste.Mod.MintChocolateHelper.Registry.Components;

internal class PlayerDistanceFade : Component
{
    public float InnerRadius;
    public float OuterRadius;
    public bool FadeOut;
    
    public float DecalDistance;
    public float ColorPercentage;
    public Color OriginalColor;
    
    public PlayerDistanceFade(float innerRadius, float outerRadius, bool fadeOut) : base(active: true, visible:true)
    {
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        FadeOut = fadeOut;
        
        DecalDistance = 0;
        ColorPercentage = 0;
        OriginalColor = Color.White;
    }

    public override void EntityAwake()
    {
        Decal decal = (Decal)Entity;
        OriginalColor = decal.Color;
    }

    public override void Update()
    {
        Decal decal = (Decal)Entity;
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();

        DecalDistance = (float)Math.Sqrt(Math.Pow(player.X - decal.X, 2) + Math.Pow(player.Y - decal.Y, 2));
        ColorPercentage = (((OuterRadius - InnerRadius) - (DecalDistance - InnerRadius)) / (OuterRadius - InnerRadius));

        if (!FadeOut)
        {
            if (DecalDistance < InnerRadius)
            {
                decal.Color = OriginalColor;
            }
            else if (DecalDistance > OuterRadius)
            {
                decal.Color = Color.Transparent;
            }
            else
            {
                decal.Color = Color.Multiply(OriginalColor, ColorPercentage);
            }
        }
        else
        {
            if (DecalDistance < InnerRadius)
            {
                decal.Color = Color.Transparent;
            }
            else if (DecalDistance > OuterRadius)
            {
                decal.Color = OriginalColor;
            }
            else
            {
                decal.Color = Color.Multiply(OriginalColor, 1 - ColorPercentage);
            }
        }
    }
}
