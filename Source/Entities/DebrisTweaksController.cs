namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/DebrisTweaksController")]
[Tracked]
public class DebrisTweaksController : Entity
{
    private readonly bool AlternateFadeout;
    private readonly bool WindAffected;
    private readonly bool PlayerAffected;

    public DebrisTweaksController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        AlternateFadeout = data.Bool("alternateFadeout");
        WindAffected = data.Bool("windAffected");
        PlayerAffected = data.Bool("playerAffected");
    }

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Debris.ctor += DebrisOnctor;
        On.Celeste.Debris.Update += DebrisOnUpdate;
        IL.Celeste.Debris.Update += DebrisILUpdate;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Debris.ctor -= DebrisOnctor;
        On.Celeste.Debris.Update -= DebrisOnUpdate;
        IL.Celeste.Debris.Update -= DebrisILUpdate;
    }

    private static void DebrisOnctor(On.Celeste.Debris.orig_ctor orig, Debris debris)
    {
        orig(debris);
        if (!Utils.CheckEntityExistence(out DebrisTweaksController DTController)) return;

        DynamicData debrisData = DynamicData.For(debris);
        debrisData.Set("WindAffected", DTController.WindAffected);
        debrisData.Set("WindDisturbance", Vector2.Zero);
        debrisData.Set("PlayerAffected", DTController.PlayerAffected);
        debrisData.Set("PlayerDisturbance", Vector2.Zero);
    }

    private static void DebrisOnUpdate(On.Celeste.Debris.orig_Update orig, Debris debris)
    {
        orig(debris);
        if (Utils.LevelIsNotSafe(out Level level)) return;

        DynamicData debrisData = DynamicData.For(debris);
        bool WindAffected = debrisData.Get<bool>("WindAffected");
        bool PlayerAffected = debrisData.Get<bool>("PlayerAffected");
        if (!PlayerAffected && !WindAffected) return;

        if (WindAffected)
        {
            debrisData.Set("WindDisturbance", level.Wind / 300f);
        }

        if (PlayerAffected)
        {
            if (debris.CollideCheck<Player>())
            {
                Player player = level.Tracker.GetEntity<Player>();
                Vector2 vector = (debris.Position - player.Center).SafeNormalize(player.Speed.Length() * 0.02f);
                Vector2 playerDisturbance = debrisData.Get<Vector2>("PlayerDisturbance");

                if (vector.LengthSquared() > playerDisturbance.LengthSquared())
                {
                    debrisData.Set("PlayerDisturbance", vector);
                }
            }
        }

        bool CeilingAbove = debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X, debris.Position.Y - 2));
        bool WallToLeft = debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X - 2, debris.Position.Y));
        bool WallToRight = debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X + 2, debris.Position.Y));
        bool FloorBelow = debris.CollideCheck<SolidTiles>(new Vector2(debris.Position.X, debris.Position.Y + 2));

        Vector2 WindDisturbance = debrisData.Get<Vector2>("WindDisturbance");
        Vector2 PlayerDisturbance = debrisData.Get<Vector2>("PlayerDisturbance");
        Vector2 TotalDisturbance = PlayerDisturbance + WindDisturbance;

        if (!(TotalDisturbance.X < 0 && WallToLeft) && !(TotalDisturbance.X > 0 && WallToRight))
        {
            debris.Position.X += TotalDisturbance.X;
        }

        if (!(TotalDisturbance.Y < 0 && CeilingAbove) && !(TotalDisturbance.Y > 0 && FloorBelow))
        {
            debris.Position.Y += TotalDisturbance.Y;
        }

        debrisData.Set("PlayerDisturbance", Calc.Approach(PlayerDisturbance, Vector2.Zero, 8f * Engine.DeltaTime));
    }

    private static void DebrisILUpdate(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_01d2: ldarg.0
        // IL_01d3: ldfld class Monocle.Image Celeste.Debris::image
        // IL_01d8: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_White()
        // IL_01dd: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_Gray()

        if (!cursor.TryGotoNextBestFit(MoveType.Before, static instr => instr.MatchLdarg0(),
            static instr => instr.MatchLdfld<Debris>("image"),
            static instr => instr.MatchCall<Color>("get_White"),
            static instr => instr.MatchCall<Color>("get_Gray")))
        {
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!");
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
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!");
            return;
        }

        cursor.MarkLabel(replaceColorLerp);
        cursor.EmitDelegate(ReplaceColorLerp);
    }

    private static bool ShouldReplaceColorLerp() => Utils.CheckEntityExistence(out DebrisTweaksController DTController) && DTController.AlternateFadeout;

    private static void ReplaceColorLerp()
    {
        if (Utils.LevelIsNotSafe(out Level level) || !ShouldReplaceColorLerp()) return;

        foreach (Debris debris in level.Tracker.GetEntitiesTrackIfNeeded<Debris>().Cast<Debris>())
        {
            debris?.image.Color = Color.White * (debris.lifeTimer / 1.5f) * debris.alpha;
        }
    }
}