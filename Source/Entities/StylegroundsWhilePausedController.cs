namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/StylegroundsWhilePaused", "MintChocolateHelper/StylegroundsWhilePausedController")]
[Tracked]
public class StylegroundsWhilePausedController : Entity
{
    private readonly string updateTag;

    public StylegroundsWhilePausedController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        updateTag = data.Attr("updateTag");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (Utils.SceneIsNotSafe(scene, out Level level)) return;

        foreach (Backdrop dummy in level.Background.Backdrops.Where(backdrop => backdrop.Tags.Contains(updateTag)))
        {
            Tag |= Tags.PauseUpdate;
        }
        foreach (Backdrop dummy in level.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(updateTag)))
        {
            Tag |= Tags.PauseUpdate;
        }
    }

    public override void Update()
    {
        if (Utils.LevelIsNotSafe(out Level level)) return;

        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop => Scene.Paused && backdrop.Tags.Contains(updateTag)))
        {
            backdrop.Update(Scene);
        }
        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => Scene.Paused && backdrop.Tags.Contains(updateTag)))
        {
            backdrop.Update(Scene);
        }
    }

    [OnLoad]
    internal static void Load()
    {
        IL.Celeste.Level.Update += LevelOnUpdate;
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.Level.Update -= LevelOnUpdate;
    }

    private static void LevelOnUpdate(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_002f: call float32 Monocle.Engine::get_RawDeltaTime()
        // IL_0034: sub
        // IL_0035: stfld float32 Celeste.Level::unpauseTimer

        if (!cursor.TryGotoNextBestFit(MoveType.After, static instr => instr.MatchCall<Engine>("get_RawDeltaTime"),
            static instr => instr.MatchSub(),
            static instr => instr.MatchStfld<Level>("unpauseTimer")))
        {
            Logger.Info("debug", $"IL hook application on method {il.Method.FullName} failed: Dumb Fuck!");
            return;
        }

        cursor.EmitDelegate(UpdateBackdrops);
    }

    private static void UpdateBackdrops()
    {
        if (Utils.LevelIsNotSafe(out Level level) || !Utils.CheckEntityExistence(out StylegroundsWhilePausedController SWPController)) return;

        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop =>
            backdrop.Tags.Contains(SWPController.updateTag)))
        {
            backdrop.Update(level);
        }

        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop =>
            backdrop.Tags.Contains(SWPController.updateTag)))
        {
            backdrop.Update(level);
        }
    }
}