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
    public Color CurrentColor;
    
    public PlayerDistanceFade(float innerRadius, float outerRadius, bool fadeOut) : base(active: true, visible:true)
    {
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        FadeOut = fadeOut;
    }

    public override void EntityAwake()
    {
        base.EntityAwake();
        Decal decal = (Decal)Entity;
        OriginalColor = decal.Color;
    }

    public override void Update()
    {
        base.Update();
        Decal decal = (Decal)Entity;
        decal.Color = OriginalColor;
    }

    public override void Render()
    {
        base.Render();
        
        Decal decal = (Decal)Entity;
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();

        CurrentColor = decal.Color;
        
        if (player != null)
        {
            DecalDistance = (float)Math.Sqrt(Math.Pow(player.X - decal.X, 2) + Math.Pow(player.Y - decal.Y, 2));
            ColorPercentage = Math.Clamp((((OuterRadius - InnerRadius) - (DecalDistance - InnerRadius)) / (OuterRadius - InnerRadius)), 0, 1);
        }

        if (FadeOut)
        {
            decal.Color = player switch {
                null => CurrentColor,
                _ => DecalDistance < InnerRadius ? Color.Transparent : Color.Multiply(CurrentColor, 1 - ColorPercentage)
            };
        }
        else
        {
            decal.Color = player switch {
                null => Color.Transparent,
                _ => DecalDistance > OuterRadius ? Color.Transparent : Color.Multiply(CurrentColor, ColorPercentage)
            };
        }
    }
}
