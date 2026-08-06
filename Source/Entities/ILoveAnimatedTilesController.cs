using EntityList = IL.Monocle.EntityList;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/ILoveAnimatedTilesController")]
[Tracked]
public class ILoveAnimatedTilesController : Entity
{
    public ILoveAnimatedTilesController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Autotiler.Generate += AutotilerOnGenerate;
        EntityList.UpdateLists += EntityListOnUpdateLists;

        On.Celeste.Platform.OnShake += PlatformOnOnShake;
        On.Celeste.AnimatedTiles.Render += AnimatedTilesOnRender;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Autotiler.Generate -= AutotilerOnGenerate;
        EntityList.UpdateLists -= EntityListOnUpdateLists;

        On.Celeste.Platform.OnShake -= PlatformOnOnShake;
        On.Celeste.AnimatedTiles.Render -= AnimatedTilesOnRender;
    }

    private static Autotiler.Generated AutotilerOnGenerate(On.Celeste.Autotiler.orig_Generate orig,
        Autotiler self, VirtualMap<char> mapData,
        int startX, int startY,
        int tilesX, int tilesY,
        bool forceSolid, char forceId,
        Autotiler.Behaviour behaviour)
    {
        Autotiler.Generated generated = orig(self, mapData, startX, startY, tilesX, tilesY, forceSolid, forceId, behaviour);
        DynamicData TileGridData = DynamicData.For(generated.TileGrid);
        TileGridData.Set("AnimatedTiles", generated.SpriteOverlay);

        return generated;
    }

    private static void EntityListOnUpdateLists(ILContext il)
    {
        ILCursor cursor = new(il);

        // IL_01b2: ldloc.s 5
        // IL_01b4: ldarg.0
        // IL_01b5: callvirt instance class Monocle.Scene Monocle.EntityList::get_Scene()
        // IL_01ba: callvirt instance void Monocle.Entity::Awake(class Monocle.Scene)

        int AwokenEntityLoc = -1;
        cursor.GotoNext(MoveType.Before, static instr => instr.MatchCallvirt<Entity>("Awake"));
        cursor.GotoPrev(MoveType.After, instr => instr.MatchLdloc(out AwokenEntityLoc));
        cursor.GotoNext(MoveType.After, static instr => instr.MatchCallvirt<Entity>("Awake"));

        cursor.EmitLdloc(AwokenEntityLoc);
        cursor.EmitDelegate(EntityOnAwake);
    }

    private static void EntityOnAwake(Entity self)
    {
        if (Utils.SceneIsNotSafe(self.Scene, out Level level)) return;
        if (level.Tracker.GetEntity<ILoveAnimatedTilesController>() is null) return;
        if (self.Get<AnimatedTiles>() is not null) return;
        if (self.Get<TileGrid>() is not { } tileGrid) return;

        DynamicData TileGridData = DynamicData.For(tileGrid);
        AnimatedTiles animatedTiles = TileGridData.Get<AnimatedTiles>("AnimatedTiles");
        if (animatedTiles is null) return;

        animatedTiles.Position = tileGrid.Position;
        animatedTiles.ClipCamera = level.Camera;
        self.Add(animatedTiles);
    }

    private static void PlatformOnOnShake(On.Celeste.Platform.orig_OnShake orig, Platform self, Vector2 amount)
    {
        orig(self, amount);
        if (self.Get<AnimatedTiles>() is not { } animatedTiles) return;
        animatedTiles.Position += amount;
    }

    private static void AnimatedTilesOnRender(On.Celeste.AnimatedTiles.orig_Render orig, AnimatedTiles self)
    {
        orig(self);
        if (self.Entity.Get<TileGrid>() is { } tileGrid)
        {
            self.Alpha = tileGrid.Alpha;
        }
        if (self.Entity is IntroCrusher introCrusher)
        {
            self.Position = introCrusher.shake;
        }
    }
}