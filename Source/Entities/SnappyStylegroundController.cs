using FlaglinesAndSuch;
using VivHelper.Effects;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[UsedImplicitly]
[CustomEntity("MintChocolateHelper/SnappyStylegroundController")]
public class SnappyStylegroundController : Entity
{
    private readonly string SnapTag;

    private static FieldInfo WindPetalsFade;
    private static FieldInfo CustomGodraysFade;
    private static FieldInfo VivCustomRainFade;

    public SnappyStylegroundController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        AddTag(Tags.PauseUpdate);
        SnapTag = data.Attr("SnapTag");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        if (MintChocolateHelperModule.FemtoHelperLoaded)
        {
            WindPetalsFade = typeof(WindPetals).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        if (MintChocolateHelperModule.FlaglinesLoaded)
        {
            CustomGodraysFade = typeof(CustomGodrays).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        if (MintChocolateHelperModule.VivhelperLoaded)
        {
            VivCustomRainFade = typeof(CustomRain).GetField("visibleFade", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();

        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop => backdrop.Tags.Contains(SnapTag)))
        {
            switch (backdrop)
            {
                //Vanilla
                case Godrays godrays:
                    godrays.fade = level.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case Snow snow:
                    snow.visibleFade = level.GetFlag(snow.OnlyIfFlag) ? 1 : 0;
                    break;
                case NorthernLights northernLights:
                    foreach (NorthernLights.Strand strand in northernLights.strands)
                    {
                        strand.Alpha = 1f;
                    }
                    break;
            }

            //Modded
            if (MintChocolateHelperModule.FemtoHelperLoaded && isWindPetals(backdrop))
            {
                setWindPetals(backdrop);
            }

            if (MintChocolateHelperModule.FlaglinesLoaded && isCustomGodrays(backdrop))
            {
                setCustomGodRays(backdrop);
            }

            if (MintChocolateHelperModule.VivhelperLoaded && isVivRain(backdrop))
            {
                setVivRain(backdrop);
            }
        }

        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(SnapTag)))
        {
            switch (backdrop)
            {
                //Vanilla
                case Godrays godrays:
                    godrays.fade = level.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case Snow snow:
                    snow.visibleFade = level.GetFlag(snow.OnlyIfFlag) ? 1 : 0;
                    break;
                case NorthernLights northernLights:
                    foreach (NorthernLights.Strand strand in northernLights.strands)
                    {
                        strand.Alpha = 1f;
                    }
                    break;
            }

            //Modded
            if (MintChocolateHelperModule.FemtoHelperLoaded && isWindPetals(backdrop))
            {
                setWindPetals(backdrop);
            }

            if (MintChocolateHelperModule.FlaglinesLoaded && isCustomGodrays(backdrop))
            {
                setCustomGodRays(backdrop);
            }

            if (MintChocolateHelperModule.VivhelperLoaded && isVivRain(backdrop))
            {
                setVivRain(backdrop);
            }
        }
    }

    private static bool isWindPetals(Backdrop backdrop) => backdrop is WindPetals;
    private static void setWindPetals(Backdrop windPetals) => WindPetalsFade?.SetValue(windPetals, Utils.GetFlag(windPetals.OnlyIfFlag) ? 1 : 0);

    private static bool isCustomGodrays(Backdrop backdrop) => backdrop is CustomGodrays;
    private static void setCustomGodRays(Backdrop godrays) => CustomGodraysFade?.SetValue(godrays, Utils.GetFlag(godrays.OnlyIfFlag) ? 1 : 0);

    private static bool isVivRain(Backdrop backdrop) => backdrop is CustomRain;
    private static void setVivRain(Backdrop vivRain) => VivCustomRainFade?.SetValue(vivRain, Utils.GetFlag(vivRain.OnlyIfFlag) ? 1 : 0);
}