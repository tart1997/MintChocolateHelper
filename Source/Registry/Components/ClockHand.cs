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

    private readonly EasingUtils.Easer TickEasingFunction;

    public ClockHand(bool alwaysUpdate, bool randomStart, bool backwards,
        int stopNumber, float tickSpeed, float tickDelay, string allowTickFlag, EasingUtils.EasingFunctions easingFunction) : base(true, true)
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

        TickEasingFunction = EasingUtils.GlobalEasingFunction(easingFunction);
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
        if (Utils.LevelIsNotSafe(out Level level)) return;
        Decal decal = (Decal)Entity;

        AllowTick = AllowTickFlag == "" || level.Session.GetFlag(AllowTickFlag);

        if (FinishedCurrentTick)
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
        else
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

                float TimerProgress = TickSpeedTimer / TickSpeed * FunnyErrorHandlerValue;
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
    }
}