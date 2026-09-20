namespace Celeste.Mod.MintChocolateHelper.Triggers;

[UsedImplicitly]
[CustomEntity("MintChocolateHelper/WaitUntilTrueTrigger")]
public class WaitUntilTrueTrigger : Trigger
{
    private readonly Vector2[] nodes;
    private readonly string Flag;
    private readonly float Delay;
    private readonly bool OneUse;

    private List<Trigger> triggers;
    private bool Activated;
    private bool Activating;
    private bool Deactivating;

    private readonly object FlagExpression;
    private readonly bool IsValidExpression;

    public WaitUntilTrueTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        nodes = data.NodesOffset(offset);
        Flag = data.Attr("flag");
        Delay = data.Float("delay");
        OneUse = data.Bool("oneUse");
        if (FrostHelperImports.SafeTryCreateSessionExpression(Flag, out FlagExpression)) IsValidExpression = true;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Add(new Coroutine(WaitUntilTrue()));
        if (Activated && OneUse) RemoveSelf();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        TryDeactivate(player);
    }

    public override void Update()
    {
        base.Update();
        if (Scene.GetEntity<Player>() is { } && Activated && OneUse) RemoveSelf();
    }

    private void TryActivate(Player player)
    {
        if (Activating || (Activated && !Deactivating)) return;

        if (Delay > 0f)
        {
            Activating = true;
            Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => {
                Activating = false;
                ActivateTriggers(player);
            }, Delay, true));
        }
        else
        {
            ActivateTriggers(player);
        }
    }

    private void TryDeactivate(Player player)
    {
        if (Deactivating || (!Activated && !Activating)) return;

        if (Delay > 0f)
        {
            Deactivating = true;
            Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => {
                Deactivating = false;
                DeactivateTriggers(player);
            }, Delay, true));
        }
        else
        {
            DeactivateTriggers(player);
        }
    }

    private void CleanTriggers() => triggers.RemoveAll(trigger => trigger.Scene == null);

    private void ActivateTriggers(Player player)
    {
        DeactivateTriggers(player);
        CleanTriggers();
        Activated = true;

        foreach (Trigger trigger in triggers.Where(trigger => trigger != null))
        {
            if (trigger.PlayerIsInside) trigger.OnLeave(player);
            trigger.OnEnter(player);
        }
    }

    private void DeactivateTriggers(Player player)
    {
        CleanTriggers();
        Activated = false;

        foreach (Trigger trigger in triggers.Where(trigger => trigger.PlayerIsInside))
        {
            trigger.OnLeave(player);
        }
    }

    private List<Trigger> GetTriggers(Scene scene)
    {
        List<Trigger> localTriggers = [];

        foreach (Vector2 node in nodes)
        {
            Dictionary<Trigger, bool> wasCollidable = new();

            foreach (Trigger trig in scene.GetEntities<Trigger>()!)
            {
                wasCollidable.Add(trig, trig.Collidable);
                trig.Collidable = true;
            }

            Trigger trigger = scene.CollideFirst<Trigger>(node);

            foreach (Trigger trig in scene.GetEntities<Trigger>()!)
            {
                trig.Collidable = wasCollidable[trig];
            }

            trigger ??= scene.Tracker.GetNearestEntity<Trigger>(node);

            if (trigger != this && trigger != null)
            {
                localTriggers.Add(trigger);
                trigger.Collidable = false;
            }
        }

        return localTriggers;
    }

    private IEnumerator WaitUntilTrue()
    {
        Level level = SceneAs<Level>();

        while (IsValidExpression ? !FrostHelperImports.SafeGetBoolSessionExpressionValue(FlagExpression, level.Session) : !level.Session.GetFlag(Flag))
        {
            yield return null;
        }

        triggers = GetTriggers(Scene);
        TryActivate(Scene.GetEntity<Player>());
    }
}