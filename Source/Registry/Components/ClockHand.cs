using System;
using Microsoft.Xna.Framework;
using Monocle;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Celeste.Mod.MintChocolateHelper.Registry.Components;

internal class ClockHand : Component
{
    public bool AlwaysUpdate;
    public bool RandomStart;
    public bool Backwards;
    
    public int StopNumber;
    public int CurrentStop;
    public float RotationAngle;
    public float OriginalRotation;
    
    public float TickSpeed;
    public float TickDelay;
    public float TickSpeedTimer;
    public float TickDelayTimer;
    public float FunnyErrorHandlerValue;
    
    public string AllowTickFlag;
    public bool AllowTick;
    public bool FinishedCurrentTick;
    
    public delegate float TickEaser(float t);
    public TickEaser TickEasingFunction;
    
    public static float TickLinear(float t) => Ease.Linear(t);
    public static float TickBackIn(float t) => Ease.BackIn(t);
    public static float TickBackOut(float t) => Ease.BackOut(t);
    public static float TickBackInOut(float t) => Ease.BackInOut(t);
    public static float TickBigBackIn(float t) => Ease.BigBackIn(t);
    public static float TickBigBackOut(float t) => Ease.BigBackOut(t);
    public static float TickBigBackInOut(float t) => Ease.BigBackInOut(t);
    public static float TickBounceIn(float t) => Ease.BounceIn(t);
    public static float TickBounceOut(float t) => Ease.BounceOut(t);
    public static float TickBounceInOut(float t) => Ease.BounceInOut(t);
    public static float TickCubeIn(float t) => Ease.CubeIn(t);
    public static float TickCubeOut(float t) => Ease.CubeOut(t);
    public static float TickCubeInOut(float t) => Ease.CubeInOut(t);
    public static float TickElasticIn(float t) => Ease.ElasticIn(t);
    public static float TickElasticOut(float t) => Ease.ElasticOut(t);
    public static float TickElasticInOut(float t) => Ease.ElasticInOut(t);
    public static float TickExpoIn(float t) => Ease.ExpoIn(t);
    public static float TickExpoOut(float t) => Ease.ExpoOut(t);
    public static float TickExpoInOut(float t) => Ease.ExpoInOut(t);
    public static float TickQuadIn(float t) => Ease.QuadIn(t);
    public static float TickQuadOut(float t) => Ease.QuadOut(t);
    public static float TickQuadInOut(float t) => Ease.QuadInOut(t);
    public static float TickQuintIn(float t) => Ease.QuintIn(t);
    public static float TickQuintOut(float t) => Ease.QuintOut(t);
    public static float TickQuintInOut(float t) => Ease.QuintInOut(t);
    public static float TickSineIn(float t) => Ease.SineIn(t);
    public static float TickSineOut(float t) => Ease.SineOut(t);
    public static float TickSineInOut(float t) => Ease.SineInOut(t);
        
        
    public ClockHand(bool alwaysUpdate, bool randomStart, bool backwards,
        int stopNumber, float tickSpeed, float tickDelay, string allowTickFlag, string easingFunctionString) : base(active: true, visible:true)
    {
        AlwaysUpdate = alwaysUpdate;
        RandomStart = randomStart;
        Backwards = backwards;
        
        StopNumber = stopNumber;
        CurrentStop = 0;
        RotationAngle = MathHelper.ToRadians(360f / StopNumber);
        OriginalRotation = 0f;
        
        TickSpeed = tickSpeed;
        TickDelay = tickDelay;
        TickSpeedTimer = 0f;
        TickDelayTimer = 0f;
        FunnyErrorHandlerValue = Calc.LerpClamp(1.5f, 1.03f, Calc.Clamp(Ease.ExpoOut((TickSpeed - 0.05f) / 0.95f), 0, 1));
        AllowTickFlag = allowTickFlag;
        AllowTick = false;
        FinishedCurrentTick = false;

        TickEasingFunction = easingFunctionString switch {
            "Linear" => TickLinear,
            "BackIn" => TickBackIn,
            "BackOut" => TickBackOut,
            "BackInOut" => TickBackInOut,
            "BigBackIn" => TickBigBackIn,
            "BigBackOut" => TickBigBackOut,
            "BigBackInOut" => TickBigBackInOut,
            "BounceIn" => TickBounceIn,
            "BounceOut" => TickBounceOut,
            "BounceInOut" => TickBounceInOut,
            "CubeIn" => TickCubeIn,
            "CubeOut" => TickCubeOut,
            "CubeInOut" => TickCubeInOut,
            "ElasticIn" => TickElasticIn,
            "ElasticOut" => TickElasticOut,
            "ElasticInOut" => TickElasticInOut,
            "ExpoIn" => TickExpoIn,
            "ExpoOut" => TickExpoOut,
            "ExpoInOut" => TickExpoInOut,
            "QuadIn" => TickQuadIn,
            "QuadOut" => TickQuadOut,
            "QuadInOut" => TickQuadInOut,
            "QuintIn" => TickQuintIn,
            "QuintOut" => TickQuintOut,
            "QuintInOut" => TickQuintInOut,
            "SineIn" => TickSineIn,
            "SineOut" => TickSineOut,
            "SineInOut" => TickSineInOut,
            _ => TickLinear
        };
    }

    public override void EntityAwake()
    {
        Decal decal = (Decal)Entity;
        OriginalRotation = decal.Rotation;
        
        if (AlwaysUpdate)
        {
            decal.AddTag(Tags.TransitionUpdate);
            decal.AddTag(Tags.PauseUpdate);
            decal.AddTag(Tags.FrozenUpdate);
        }

        if (RandomStart)
        {
            Random rng = new();
            CurrentStop = rng.Next(StopNumber);
            decal.Rotation = RotationAngle * CurrentStop;
        }
    }

    public override void Update()
    {
        if (Scene is not Level level) return;
        Decal decal = (Decal)Entity;
    
        AllowTick = AllowTickFlag == "" || level.Session.GetFlag(AllowTickFlag);
    
        if (!FinishedCurrentTick)
        {
            if (TickSpeedTimer < TickSpeed)
            {
                float CurrentStopAngle = RotationAngle * CurrentStop;
                float NextStopAngle;
                
                if (Backwards)
                {
                    NextStopAngle = RotationAngle * (CurrentStop - 1);
                }
                else
                {
                    NextStopAngle = RotationAngle * (CurrentStop + 1);
                }

                float TimerProgress = (TickSpeedTimer / TickSpeed) * FunnyErrorHandlerValue;
                decal.Rotation = Calc.LerpClamp(CurrentStopAngle, NextStopAngle, TickEasingFunction(TimerProgress)) + OriginalRotation;
                
                TickSpeedTimer += Engine.DeltaTime;
            }
            else
            {
                if (AllowTick)
                {
                    if (Backwards)
                    {
                        CurrentStop--;
                    }
                    else
                    {
                        CurrentStop++;
                    }
                    
                    CurrentStop %= StopNumber;
                    
                    TickSpeedTimer -= TickSpeed;
                    FinishedCurrentTick = true;
                }
            }
        }
        else
        {
            if (TickDelayTimer < TickDelay)
            {
                TickDelayTimer += Engine.DeltaTime;
            }
            else
            {
                TickDelayTimer -= TickDelay;
                FinishedCurrentTick = false;
            }
        }
    }
}
