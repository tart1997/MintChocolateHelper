using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;

namespace Celeste.Mod.MintChocolateHelper.ModInterop;
[ModImportName("FrostHelper")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public static class FrostHelperImports
{
    public static Func<Spring,bool> IsCeilingSpring;
    public static Func<Spring,Vector2> GetSpringSpeedMultiplier;
}