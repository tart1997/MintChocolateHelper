using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/AlternateDebrisFade")]

// ReSharper disable once ClassNeverInstantiated.Global
public class AlternateDebrisFade : Entity
{
    public AlternateDebrisFade(EntityData data, Vector2 offset) : base( data.Position + offset)
    {
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!MintChocolateHelperModule.SaveData.AlternateDebrisFadeHookLoaded)
        {
            MintChocolateHelperModule.SaveData.AlternateDebrisFadeHookLoaded = true;
            Load();
        }
    }

    private static void Load()
    {
        On.Celeste.Debris.Update += DebrisOnUpdate;
    }
    
    public static void Unload()
    {
        On.Celeste.Debris.Update -= DebrisOnUpdate;
    }
    
    private static void DebrisOnUpdate(On.Celeste.Debris.orig_Update orig, Debris debris)
    {
        if (Engine.Scene is not Level) orig(debris);

        debris.image.Rotation += Math.Abs(debris.speed.X) * (float)debris.rotateSign * Engine.DeltaTime;
        debris.MoveH(debris.speed.X * Engine.DeltaTime, debris.collideH);
        debris.MoveV(debris.speed.Y * Engine.DeltaTime, debris.collideV);
        if (debris.dreaming)
        {
            debris.speed.X = Calc.Approach(debris.speed.X, 0f, 50f * Engine.DeltaTime);
            debris.speed.Y = Calc.Approach(debris.speed.Y, 6f * debris.dreamSine.Value, 100f * Engine.DeltaTime);
        }
        else
        {
            bool flag = debris.OnGround();
            debris.speed.X = Calc.Approach(debris.speed.X, 0f, (flag ? 50f : 20f) * Engine.DeltaTime);
            if (!flag)
            {
                debris.speed.Y = Calc.Approach(debris.speed.Y, 100f, 400f * Engine.DeltaTime);
            }
        }
        if (debris.lifeTimer > 0f)
        {
            debris.lifeTimer -= Engine.DeltaTime;
        }
        else if (debris.alpha > 0f)
        {
            debris.alpha -= 4f * Engine.DeltaTime;
            if (debris.alpha <= 0f)
            {
                debris.RemoveSelf();
            }
        }
        debris.image.Color = Color.White * (debris.lifeTimer / 1.5f) * debris.alpha;

        if (debris.Scene.Tracker.GetEntities<AlternateDebrisFade>().Count == 0)
        {
            MintChocolateHelperModule.SaveData.AlternateDebrisFadeHookLoaded = false;
            Unload();
        }
    }
}