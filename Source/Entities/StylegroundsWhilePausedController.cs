namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[ConditionalEntity]
[CustomEntity("MintChocolateHelper/StylegroundsWhilePaused", "MintChocolateHelper/StylegroundsWhilePausedController")]
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
        Level level = scene.AsLevel();

        foreach (Backdrop dummy in level?.Background.Backdrops.Where(backdrop => backdrop.Tags.Contains(updateTag))!)
        {
            Tag |= Tags.PauseUpdate;
        }
        foreach (Backdrop dummy in level?.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(updateTag))!)
        {
            Tag |= Tags.PauseUpdate;
        }
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();

        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop => Scene.Paused && backdrop.Tags.Contains(updateTag)))
        {
            backdrop.Update(Scene);
        }
        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => Scene.Paused && backdrop.Tags.Contains(updateTag)))
        {
            backdrop.Update(Scene);
        }
    }

    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogVerbose($"Loading {nameof(StylegroundsWhilePausedController)} Hooks...");
        IL.Celeste.Level.Update += LevelOnUpdate;
    }

    [OnUnload]
    internal static void Unload() => IL.Celeste.Level.Update -= LevelOnUpdate;

    private static void LevelOnUpdate(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_002f: call float32 Monocle.Engine::get_RawDeltaTime()
        IL_0034: sub
        IL_0035: stfld float32 Celeste.Level::unpauseTimer
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchCall<Engine>("get_RawDeltaTime"),
                static instr => instr.MatchSub(),
                static instr => instr.MatchStfld<Level>("unpauseTimer")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitDelegate(UpdateBackdrops);
    }

    private static void UpdateBackdrops()
    {
        if (SearchUtils.IfNone(out StylegroundsWhilePausedController SWPController)) return;
        Level level = SWPController.SceneAs<Level>();

        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop => backdrop.Tags.Contains(SWPController.updateTag)))
        {
            backdrop.Update(level);
        }

        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(SWPController.updateTag)))
        {
            backdrop.Update(level);
        }
    }
}