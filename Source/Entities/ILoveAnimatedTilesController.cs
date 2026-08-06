using ComponentList = On.Monocle.ComponentList;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/ILoveAnimatedTilesController")]
[Tracked]
public class ILoveAnimatedTilesController : Entity
{
    private static char? TileId;

    public ILoveAnimatedTilesController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Autotiler.Generate += AutotilerOnGenerate;
        ComponentList.Add_Component += ComponentListOnAdd_Component;
        On.Celeste.AnimatedTiles.Update += AnimatedTilesOnUpdate;

        On.Celeste.Platform.OnShake += PlatformOnOnShake;
        On.Celeste.AnimatedTiles.Render += AnimatedTilesOnRender;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Autotiler.Generate -= AutotilerOnGenerate;
        ComponentList.Add_Component -= ComponentListOnAdd_Component;
        On.Celeste.AnimatedTiles.Update -= AnimatedTilesOnUpdate;

        On.Celeste.Platform.OnShake -= PlatformOnOnShake;
        On.Celeste.AnimatedTiles.Render -= AnimatedTilesOnRender;
    }

    private static Autotiler.Generated AutotilerOnGenerate(On.Celeste.Autotiler.orig_Generate orig,
        Autotiler self,
        VirtualMap<char> mapData,
        int startX,
        int startY,
        int tilesX,
        int tilesY,
        bool forceSolid,
        char forceId,
        Autotiler.Behaviour behaviour)
    {
        TileId = forceId;
        return orig(self, mapData, startX, startY, tilesX, tilesY, forceSolid, forceId, behaviour);
    }

    private static void ComponentListOnAdd_Component(ComponentList.orig_Add_Component orig, Monocle.ComponentList self, Component component)
    {
        orig(self, component);

        if (component is TileGrid)
        {
            self.Entity.AddAnimatedTiles(TileId, out AnimatedTiles animatedTiles);

            if (Utils.SceneIsNotSafe(self.Entity.Scene, out Level level) || animatedTiles is null) return;
            animatedTiles.ClipCamera = level.Camera;
        }
    }

    private static void AnimatedTilesOnUpdate(On.Celeste.AnimatedTiles.orig_Update orig, AnimatedTiles self)
    {
        orig(self);
        if (self.Scene.Tracker.GetEntity<ILoveAnimatedTilesController>() is not null) return;
        self.Entity.Remove(self);
    }

    private static void PlatformOnOnShake(On.Celeste.Platform.orig_OnShake orig, Platform self, Vector2 amount)
    {
        orig(self, amount);

        if (self.Get<AnimatedTiles>() is { } animatedTiles)
        {
            animatedTiles.Position += amount;
        }
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

public static class AnimatedTilesExtensions
{
    public static void AddAnimatedTiles(this Entity Block, char? tileType, out AnimatedTiles animatedTiles)
    {
        DynamicData BlockData = DynamicData.For(Block);

        // If An entity does "Merging", Generate the animated tiles using the Master of the group
        if (BlockData.TryGet("MasterOfGroup", out bool? MasterOfGroup) &&
            BlockData.TryGet("X", out float X) &&
            BlockData.TryGet("Y", out float Y) &&
            BlockData.TryGet("GroupBoundsMin", out Point GroupBoundsMin) &&
            BlockData.TryGet("GroupBoundsMax", out Point GroupBoundsMax) &&
            BlockData.TryGet("Group", out IList Group))
        {
            if (MasterOfGroup is true)
            {
                foreach (FieldInfo field in Block.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (field.FieldType == typeof(char))
                    {
                        tileType = field.GetValue(Block) as char?;
                    }
                }

                if (tileType is null)
                {
                    animatedTiles = null;
                    return;
                }

                Rectangle rectangle = new(GroupBoundsMin.X / 8, GroupBoundsMin.Y / 8, (GroupBoundsMax.X - GroupBoundsMin.X) / 8 + 1, (GroupBoundsMax.Y - GroupBoundsMin.Y) / 8 + 1);
                VirtualMap<char> virtualMap = new(rectangle.Width, rectangle.Height, '0');
                foreach (Entity item in Group)
                {
                    int num = (int)(item.X / 8f) - rectangle.X;
                    int num2 = (int)(item.Y / 8f) - rectangle.Y;
                    int num3 = (int)(item.Width / 8f);
                    int num4 = (int)(item.Height / 8f);
                    for (int i = num; i < num + num3; i++)
                    {
                        for (int j = num2; j < num2 + num4; j++)
                        {
                            virtualMap[i, j] = (char)tileType;
                        }
                    }
                }
                animatedTiles = GFX.FGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour {
                    EdgesExtend = false,
                    EdgesIgnoreOutOfLevel = false,
                    PaddingIgnoreOutOfLevel = false
                }).SpriteOverlay;

                animatedTiles.Position = new Vector2(GroupBoundsMin.X - X, GroupBoundsMin.Y - Y);
                Block.Add(animatedTiles);
            }
            else
            {
                animatedTiles = null;
            }
            return;
        }

        // If not just do it the normal way
        int tilesX = (int)Block.Width / 8;
        int tilesY = (int)Block.Height / 8;
        animatedTiles = tileType != null ? GFX.FGAutotiler.GenerateBox((char)tileType, tilesX, tilesY).SpriteOverlay : null;

        Block.Add(animatedTiles);
    }
}