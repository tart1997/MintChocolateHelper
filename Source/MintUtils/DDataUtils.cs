namespace Celeste.Mod.MintChocolateHelper.MintUtils;

public static class DDataUtils
{
    extension(DynamicData data)
    {
        [UsedImplicitly]
        internal void SetTrue(string name)
        {
            data.Set(name, true);
        }

        [UsedImplicitly]
        internal void SetFalse(string name)
        {
            data.Set(name, false);
        }

        [UsedImplicitly]
        internal bool TryGetTrue(string name) => data.TryGet(name, out bool? boolean) && boolean == true;

        [UsedImplicitly]
        internal bool TryGetFalse(string name) => data.TryGet(name, out bool? boolean) && boolean == false;
    }
}