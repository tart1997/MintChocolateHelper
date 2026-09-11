namespace Celeste.Mod.MintChocolateHelper.MintUtils;

public static class SearchUtils
{
    extension(Level level)
    {
        [CanBeNull]
        [UsedImplicitly]
        internal T GetEntity<T>(bool TrackIfNeeded = false) where T : Entity => TrackIfNeeded ? level.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().FirstOrDefault() : level.Tracker.GetEntity<T>();

        [CanBeNull]
        [UsedImplicitly]
        internal List<T> GetEntities<T>(bool TrackIfNeeded = false) where T : Entity => TrackIfNeeded ? [.. level.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>()] : [.. level.Tracker.GetEntities<T>().Cast<T>()];

        [CanBeNull]
        [UsedImplicitly]
        internal Player GetPlayer() => level.Tracker.GetEntity<Player>();

        [UsedImplicitly]
        internal bool IfAny<T>(bool TrackIfNeeded = false) where T : Entity
        {
            List<T> output = level.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Count != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Count != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out T output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
            return output != null;
        }

        [UsedImplicitly]
        internal bool IfNone<T>(bool TrackIfNeeded = false) where T : Entity
        {
            List<T> output = level.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Count == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Count == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out T output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
            return output == null;
        }
    }

    extension(Scene scene)
    {
        [CanBeNull]
        [UsedImplicitly]
        internal T GetEntity<T>(bool TrackIfNeeded = false) where T : Entity => TrackIfNeeded ? scene.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().FirstOrDefault() : scene.Tracker.GetEntity<T>();

        [CanBeNull]
        [UsedImplicitly]
        internal List<T> GetEntities<T>(bool TrackIfNeeded = false) where T : Entity => TrackIfNeeded ? [.. scene.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>()] : [.. scene.Tracker.GetEntities<T>().Cast<T>()];

        [CanBeNull]
        [UsedImplicitly]
        internal Player GetPlayer() => scene.Tracker.GetEntity<Player>();

        [UsedImplicitly]
        internal bool IfAny<T>(bool TrackIfNeeded = false) where T : Entity
        {
            List<T> output = GetEntities<T>(TrackIfNeeded);
            return output != null && output.Count != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Count != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out T output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
            return output != null;
        }

        [UsedImplicitly]
        internal bool IfNone<T>(bool TrackIfNeeded = false) where T : Entity
        {
            List<T> output = GetEntities<T>(TrackIfNeeded);
            return output == null || output.Count == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Count == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out T output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
            return output == null;
        }
    }

    [CanBeNull]
    [UsedImplicitly]
    internal static T GetEntity<T>(bool TrackIfNeeded = false) where T : Entity
    {
        Level level = Utils.GetLevel();
        return TrackIfNeeded ? level?.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().FirstOrDefault() : level?.Tracker.GetEntity<T>();
    }

    [CanBeNull]
    [UsedImplicitly]
    internal static List<T> GetEntities<T>(bool TrackIfNeeded = false) where T : Entity
    {
        Level level = Utils.GetLevel();
        return TrackIfNeeded ? level?.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().ToList() : level?.Tracker.GetEntities<T>().Cast<T>().ToList();
    }

    [CanBeNull]
    [UsedImplicitly]
    internal static Player GetPlayer()
    {
        Level level = Utils.GetLevel();
        return level?.Tracker.GetEntity<Player>();
    }

    [UsedImplicitly]
    internal static bool IfAny<T>(bool TrackIfNeeded = false) where T : Entity
    {
        List<T> output = GetEntities<T>(TrackIfNeeded);
        return output != null && output.Count != 0;
    }

    [UsedImplicitly]
    internal static bool IfAny<T>([NotNullWhen(true)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded);
        return output != null && output.Count != 0;
    }

    [UsedImplicitly]
    internal static bool IfAny<T>([NotNullWhen(true)] out T output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
        return output != null;
    }

    [UsedImplicitly]
    internal static bool IfNone<T>(bool TrackIfNeeded = false) where T : Entity
    {
        List<T> output = GetEntities<T>(TrackIfNeeded);
        return output == null || output.Count == 0;
    }

    [UsedImplicitly]
    internal static bool IfNone<T>([NotNullWhen(false)] out List<T> output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded);
        return output == null || output.Count == 0;
    }

    [UsedImplicitly]
    internal static bool IfNone<T>([NotNullWhen(false)] out T output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
        return output == null;
    }
}