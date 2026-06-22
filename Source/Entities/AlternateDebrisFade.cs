using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/AlternateDebrisFade")]

// ReSharper disable once ClassNeverInstantiated.Global
public class AlternateDebrisFade : Entity
{
    public AlternateDebrisFade(EntityData data, Vector2 offset) : base( data.Position + offset)
    {
    }

    public static void Load()
    {
        IL.Celeste.Debris.Update += DebrisOnUpdate;
    }
    
    public static void Unload()
    {
        IL.Celeste.Debris.Update -= DebrisOnUpdate;
    }

    private static void DebrisOnUpdate(ILContext il)
    {
        ILCursor cursor = new(il);
        
        // IL_01d2: ldarg.0
        // IL_01d3: ldfld class Monocle.Image Celeste.Debris::image
        // IL_01d8: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_White()
        // IL_01dd: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_Gray()

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<Debris>("image"),
            static instr => instr.MatchCall<Color>("get_White"),
            static inste => inste.MatchCall<Color>("get_Gray")))
        {
            Logger.Info("debug",$"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!"); 
            return;
        }
        
        ILLabel replaceColorLerp = cursor.DefineLabel();

        cursor.EmitDelegate(ShouldReplaceColorLerp);
        cursor.EmitBrtrue(replaceColorLerp);

        
        // IL_01ed: ldarg.0
        // IL_01ee: ldfld float32 Celeste.Debris::alpha
        // IL_01f3: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::op_Multiply(valuetype [FNA]Microsoft.Xna.Framework.Color, float32)
        // IL_01f8: stfld valuetype [FNA]Microsoft.Xna.Framework.Color Monocle.GraphicsComponent::Color
        
        if (!cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchLdarg(0),
            static instr => instr.MatchLdfld<Debris>("alpha"),
            static instr => instr.MatchCall<Color>("op_Multiply"),
            static instr => instr.MatchStfld<GraphicsComponent>("Color")))
        {
            Logger.Info("debug",$"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!"); 
            return;
        }
        
        cursor.MarkLabel(replaceColorLerp);
        cursor.EmitDelegate(ReplaceColorLerp);
    }
    
    private static bool ShouldReplaceColorLerp()
    {
        if (Engine.Scene is not Level level) return false;
        
        List<Entity> ADFControllers = level.Tracker.GetEntitiesTrackIfNeeded<AlternateDebrisFade>();
        
        if (ADFControllers == null || ADFControllers.Count == 0) return false;
        return ADFControllers[0] is AlternateDebrisFade;
    }

    private static void ReplaceColorLerp()
    {
        if (Engine.Scene is not Level level) return;
        
        List<Entity> ADFControllers = level.Tracker.GetEntitiesTrackIfNeeded<AlternateDebrisFade>();
        if (ADFControllers == null || ADFControllers.Count == 0) return;

        
        foreach (Debris debris in level.Tracker.GetEntitiesTrackIfNeeded<Debris>().Cast<Debris>())
        {
            debris?.image.Color = Color.White * (debris.lifeTimer / 1.5f) * debris.alpha;
        }
    }
}