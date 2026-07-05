namespace Celeste.Mod.MintChocolateHelper.Registry.Components;

public class ClockHand : Component
{
    private readonly bool AlwaysUpdate;
    private readonly bool RandomStart;
    private readonly bool Backwards;

    private readonly int StopNumber;
    private int CurrentStop;
    private readonly float RotationAngle;
    private float OriginalRotation;

    private readonly float TickSpeed;
    private readonly float TickDelay;
    private float TickSpeedTimer;
    private float TickDelayTimer;
    private readonly float FunnyErrorHandlerValue;

    private readonly string AllowTickFlag;
    private bool AllowTick;
    private bool FinishedCurrentTick;

    private readonly EasingFunctions.Easer TickEasingFunction;

    public ClockHand(bool alwaysUpdate, bool randomStart, bool backwards,
        int stopNumber, float tickSpeed, float tickDelay, string allowTickFlag, EasingFunctions.EasingFunction easingFunction) : base(true, true)
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

        TickEasingFunction = easingFunction switch {
            EasingFunctions.EasingFunction.Linear => EasingFunctions.Linear,
            EasingFunctions.EasingFunction.BackIn => EasingFunctions.BackIn,
            EasingFunctions.EasingFunction.BackOut => EasingFunctions.BackOut,
            EasingFunctions.EasingFunction.BackInOut => EasingFunctions.BackInOut,
            EasingFunctions.EasingFunction.BigBackIn => EasingFunctions.BigBackIn,
            EasingFunctions.EasingFunction.BigBackOut => EasingFunctions.BigBackOut,
            EasingFunctions.EasingFunction.BigBackInOut => EasingFunctions.BigBackInOut,
            EasingFunctions.EasingFunction.BounceIn => EasingFunctions.BounceIn,
            EasingFunctions.EasingFunction.BounceOut => EasingFunctions.BounceOut,
            EasingFunctions.EasingFunction.BounceInOut => EasingFunctions.BounceInOut,
            EasingFunctions.EasingFunction.CubeIn => EasingFunctions.CubeIn,
            EasingFunctions.EasingFunction.CubeOut => EasingFunctions.CubeOut,
            EasingFunctions.EasingFunction.CubeInOut => EasingFunctions.CubeInOut,
            EasingFunctions.EasingFunction.ElasticIn => EasingFunctions.ElasticIn,
            EasingFunctions.EasingFunction.ElasticOut => EasingFunctions.ElasticOut,
            EasingFunctions.EasingFunction.ElasticInOut => EasingFunctions.ElasticInOut,
            EasingFunctions.EasingFunction.ExpoIn => EasingFunctions.ExpoIn,
            EasingFunctions.EasingFunction.ExpoOut => EasingFunctions.ExpoOut,
            EasingFunctions.EasingFunction.ExpoInOut => EasingFunctions.ExpoInOut,
            EasingFunctions.EasingFunction.QuadIn => EasingFunctions.QuadIn,
            EasingFunctions.EasingFunction.QuadOut => EasingFunctions.QuadOut,
            EasingFunctions.EasingFunction.QuadInOut => EasingFunctions.QuadInOut,
            EasingFunctions.EasingFunction.QuintIn => EasingFunctions.QuintIn,
            EasingFunctions.EasingFunction.QuintOut => EasingFunctions.QuintOut,
            EasingFunctions.EasingFunction.QuintInOut => EasingFunctions.QuintInOut,
            EasingFunctions.EasingFunction.SineIn => EasingFunctions.SineIn,
            EasingFunctions.EasingFunction.SineOut => EasingFunctions.SineOut,
            EasingFunctions.EasingFunction.SineInOut => EasingFunctions.SineInOut,
            _ => EasingFunctions.Linear
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