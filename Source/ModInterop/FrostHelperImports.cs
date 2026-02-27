using System;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;
// ReSharper disable UnassignedField.Global

namespace Celeste.Mod.MintChocolateHelper.ModInterop;

[ModImportName("FrostHelper")]
public static class FrostHelperImports
{
    public static Func<Spring,bool> IsCeilingSpring;
    public static Func<Spring,Vector2> GetSpringSpeedMultiplier;
}
