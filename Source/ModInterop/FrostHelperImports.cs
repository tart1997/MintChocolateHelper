using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;
// ReSharper disable UnassignedField.Global

namespace Celeste.Mod.MintChocolateHelper.ModInterop;

[ModImportName("FrostHelper")]
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public static class FrostHelperImports
{
    public static Func<Spring,bool> IsCeilingSpring;
    public static Func<Spring,Vector2> GetSpringSpeedMultiplier;
}
