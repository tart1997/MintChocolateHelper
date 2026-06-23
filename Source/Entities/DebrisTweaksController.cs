using System;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/DebrisTweaksController")]

public class DebrisTweaksController : Entity
{
    private readonly bool AlternateFadeout;
    private readonly bool WindAffected;
    
    public DebrisTweaksController(EntityData data, Vector2 offset) : base( data.Position + offset)
    {
        AlternateFadeout = data.Bool("alternateFadeout");
        WindAffected = data.Bool("windAffected");
    }

    public static void Load()
    {
        On.Celeste.Debris.Update += DebrisOnUpdate;
        IL.Celeste.Debris.Update += DebrisILUpdate;
    }
    
    public static void Unload()
    {
        On.Celeste.Debris.Update -= DebrisOnUpdate;
        IL.Celeste.Debris.Update -= DebrisILUpdate;
    }

    private static void DebrisOnUpdate(On.Celeste.Debris.orig_Update orig, Debris debris)
    {
        #region Make Wind Affected
        
            if (Engine.Scene is not Level level)
            {
                orig(debris);
                return;
            }
            DebrisTweaksController ADFController = level.Tracker.GetEntity<DebrisTweaksController>();
            if (ADFController is not { WindAffected: true })
            {
                orig(debris);
                return;
            }

            orig(debris);
            
            if (!(level.Wind.X > 0 && debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X + 2, debris.Position.Y))))
            {
                if (!(level.Wind.X < 0 && debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X - 2, debris.Position.Y))))
                {
                    debris.Position.X += (level.Wind.X / 300);
                }
            }

            if (!(level.Wind.Y > 0 && debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X, debris.Position.Y + 2))))
            {
                if (!(level.Wind.Y < 0 && debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X, debris.Position.Y - 2))))
                {
                    debris.Position.Y += (level.Wind.Y / 300);
                }
            }
            
        #endregion
    }

    private static void DebrisILUpdate(ILContext il)
    {
        ILCursor cursor = new(il);
        
        #region Alternate Fadeout Hook
        
            // IL_01d2: ldarg.0
            // IL_01d3: ldfld class Monocle.Image Celeste.Debris::image
            // IL_01d8: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_White()
            // IL_01dd: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_Gray()

            if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<Debris>("image"),
                static instr => instr.MatchCall<Color>("get_White"),
                static instr => instr.MatchCall<Color>("get_Gray")))
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
            
        #endregion
    }
    
    #region Alternate Fadeout Code
    
        private static bool ShouldReplaceColorLerp()
        {
            if (Engine.Scene is not Level level) return false;
            
            DebrisTweaksController ADFController = level.Tracker.GetEntity<DebrisTweaksController>();
            return ADFController is { AlternateFadeout: true };
        }

        private static void ReplaceColorLerp()
        {
            if (Engine.Scene is not Level level) return;
            DebrisTweaksController ADFController = level.Tracker.GetEntity<DebrisTweaksController>();
            if (ADFController is not { AlternateFadeout: true }) return;
            
            foreach (Debris debris in level.Tracker.GetEntitiesTrackIfNeeded<Debris>().Cast<Debris>())
            {
                debris?.image.Color = Color.White * (debris.lifeTimer / 1.5f) * debris.alpha;
            }
        }
        
    #endregion
}