using System;
using Monocle;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Celeste.Mod.MintChocolateHelper.Registry.Components;

internal class ClockHand : Component
{
    public bool Persistant;
    public bool AlwaysUpdate;
    public bool RandomStart;
    public bool Backwards;

    public int StopNumber;
    
    public float TickSpeed;
    public float Delay;
    public float MicroTimer;
    public float MacroTimer;
    public float CurrentRotation;
    public float CurrentTarget;
    public bool Finished;
    
    public string TickableFlag;
    public bool AllowTick;
    
    public ClockHand(bool persistant, bool alwaysUpdate, bool randomStart, bool backwards, int stopNumber, float tickSpeed, float delay, string tickableFlag) : base(active: true, visible:true)
    {
        Persistant = persistant;
        AlwaysUpdate = alwaysUpdate;
        RandomStart = randomStart;
        Backwards = backwards;
        
        StopNumber = stopNumber;
        
        TickSpeed = tickSpeed;
        Delay = delay;
        
        MicroTimer = TickSpeed;
        MacroTimer = Delay;

        CurrentRotation = 0;
        CurrentTarget = 0;
        Finished = false;
        
        TickableFlag = tickableFlag;
        AllowTick = false;
    }

    public override void EntityAwake()
    {
        Decal decal = (Decal)Entity;
        
        if (Persistant)
        {
            decal.AddTag(Tags.Global);
        }

        if (AlwaysUpdate)
        {
            decal.AddTag(Tags.TransitionUpdate);
            decal.AddTag(Tags.PauseUpdate);
            decal.AddTag(Tags.FrozenUpdate);
        }

        if (RandomStart)
        {
            Random rng = new();
            CurrentRotation = (Calc.Circle / StopNumber) * rng.Next(StopNumber - 1);
        }
        else
        {
            CurrentRotation = decal.Rotation;
        }

        if (!Backwards)
        {
            CurrentTarget = CurrentRotation + (Calc.Circle / StopNumber);
        }
        else
        {
            CurrentTarget = CurrentRotation - (Calc.Circle / StopNumber);
        }

    }

    public override void Update()
    {
        if (Scene is not Level level) return;
        
        Decal decal = (Decal)Entity;

        AllowTick = TickableFlag == "" || level.Session.GetFlag(TickableFlag);

        if (!Finished)
        {
            if (MicroTimer > 0)
            {
                MicroTimer -= (Engine.DeltaTime * 4);

                decal.Rotation = Calc.LerpClamp(CurrentTarget, CurrentRotation,Ease.ExpoOut(MicroTimer));
            }
            else
            {
                if (AllowTick)
                {
                    MicroTimer = TickSpeed;
                    CurrentRotation = CurrentTarget;
                    
                    if (!Backwards)
                    {
                        CurrentTarget = CurrentRotation + (Calc.Circle / StopNumber);
                    }
                    else
                    {
                        CurrentTarget = CurrentRotation - (Calc.Circle / StopNumber);
                    }
                    
                    Finished = true;
                }
            }
        }
        else
        {
            if (MacroTimer > 0)
            {
                MacroTimer -= (Engine.DeltaTime * 4);
            }
            else
            {
                MacroTimer = Delay;
                Finished = false;
            }
        }
    }
}
