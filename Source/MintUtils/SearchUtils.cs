namespace Celeste.Mod.MintChocolateHelper.MintUtils;

[SuppressMessage("Usage", "CL0015:Tracker used on untracked type")]
public static class SearchUtils
{
    extension(Level level)
    {
        [CanBeNull]
        [UsedImplicitly]
        internal T GetEntity<T>(bool TrackIfNeeded = false) where T : Entity => TrackIfNeeded ? level.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().FirstOrDefault() : level.Tracker.GetEntity<T>();

        [CanBeNull]
        [UsedImplicitly]
        internal T[] GetEntities<T>(bool TrackIfNeeded = false) where T : Entity =>
            TrackIfNeeded ? [.. level.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>()] : [.. level.Tracker.GetEntities<T>().Cast<T>()];

        [CanBeNull]
        [UsedImplicitly]
        internal Player GetPlayer() => level.Tracker.GetEntity<Player>();

        [UsedImplicitly]
        internal bool IfAny<T>(bool TrackIfNeeded = false) where T : Entity
        {
            T[] output = level.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Length != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out T[] output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Length != 0;
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
            T[] output = level.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Length == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out T[] output, bool TrackIfNeeded = false) where T : Entity
        {
            output = level.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Length == 0;
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
        internal T[] GetEntities<T>(bool TrackIfNeeded = false) where T : Entity =>
            TrackIfNeeded ? [.. scene.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>()] : [.. scene.Tracker.GetEntities<T>().Cast<T>()];

        [CanBeNull]
        [UsedImplicitly]
        internal Player GetPlayer() => scene.Tracker.GetEntity<Player>();

        [UsedImplicitly]
        internal bool IfAny<T>(bool TrackIfNeeded = false) where T : Entity
        {
            T[] output = GetEntities<T>(TrackIfNeeded);
            return output != null && output.Length != 0;
        }

        [UsedImplicitly]
        internal bool IfAny<T>([NotNullWhen(true)] out T[] output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded);
            return output != null && output.Length != 0;
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
            T[] output = GetEntities<T>(TrackIfNeeded);
            return output == null || output.Length == 0;
        }

        [UsedImplicitly]
        internal bool IfNone<T>([NotNullWhen(false)] out T[] output, bool TrackIfNeeded = false) where T : Entity
        {
            output = scene.GetEntities<T>(TrackIfNeeded);
            return output == null || output.Length == 0;
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
    internal static T[] GetEntities<T>(bool TrackIfNeeded = false) where T : Entity
    {
        Level level = Utils.GetLevel();
        return TrackIfNeeded ? level?.Tracker.GetEntitiesTrackIfNeeded<T>().Cast<T>().ToArray() : level?.Tracker.GetEntities<T>().Cast<T>().ToArray();
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
        T[] output = GetEntities<T>(TrackIfNeeded);
        return output != null && output.Length != 0;
    }

    [UsedImplicitly]
    internal static bool IfAny<T>([NotNullWhen(true)] out T[] output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded);
        return output != null && output.Length != 0;
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
        T[] output = GetEntities<T>(TrackIfNeeded);
        return output == null || output.Length == 0;
    }

    [UsedImplicitly]
    internal static bool IfNone<T>([NotNullWhen(false)] out T[] output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded);
        return output == null || output.Length == 0;
    }

    [UsedImplicitly]
    internal static bool IfNone<T>([NotNullWhen(false)] out T output, bool TrackIfNeeded = false) where T : Entity
    {
        output = GetEntities<T>(TrackIfNeeded)?.FirstOrDefault();
        return output == null;
    }
}