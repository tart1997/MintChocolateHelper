using System.Linq;
using Celeste.Mod.Entities;
using FlaglinesAndSuch;
using LunaticHelper;
using Microsoft.Xna.Framework;
using Monocle;
using VivHelper.Effects;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/SnappyStylegroundController")]

public class SnappyStylegroundController : Entity
{
    private readonly string SnapTag;
    
    public SnappyStylegroundController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        AddTag(Tags.PauseUpdate);
        SnapTag = data.Attr("SnapTag");
    }
    
    public override void Update()
    {
        if (Scene is not Level level) return;
        
        foreach (Backdrop backdrop in level.Background.Backdrops.Where(backdrop => backdrop.Tags.Contains(SnapTag)))
        {
            switch (backdrop)
            {
                //Vanilla
                case Godrays godrays:
                    godrays.fade = level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case Snow snow:
                    snow.visibleFade = level.Session.GetFlag(snow.OnlyIfFlag) ? 1 : 0;
                    break;
                //Modded
                case WindPetals windPetals:
                    MintChocolateHelperModule.WindPetalsFade.SetValue(windPetals, level.Session.GetFlag(windPetals.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomGodrays godrays:
                    MintChocolateHelperModule.CustomGodraysFade.SetValue(godrays, level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomDust dust:
                    MintChocolateHelperModule.CustomDustFade.SetValue(dust, level.Session.GetFlag(dust.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomRain rain:
                    MintChocolateHelperModule.CustomRainFade.SetValue(rain, level.Session.GetFlag(rain.OnlyIfFlag) ? 1 : 0);
                    break;
            }
        }
        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(SnapTag)))
        {
            switch (backdrop)
            {
                //Vanilla
                case Godrays godrays:
                    godrays.fade = level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case Snow snow:
                    snow.visibleFade = level.Session.GetFlag(snow.OnlyIfFlag) ? 1 : 0;
                    break;
                //Modded
                case WindPetals windPetals:
                    MintChocolateHelperModule.WindPetalsFade.SetValue(windPetals, level.Session.GetFlag(windPetals.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomGodrays godrays:
                    MintChocolateHelperModule.CustomGodraysFade.SetValue(godrays, level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomDust dust:
                    MintChocolateHelperModule.CustomDustFade.SetValue(dust, level.Session.GetFlag(dust.OnlyIfFlag) ? 1 : 0);
                    break;
                case CustomRain rain:
                    MintChocolateHelperModule.CustomRainFade.SetValue(rain, level.Session.GetFlag(rain.OnlyIfFlag) ? 1 : 0);
                    break;
            }
        }
    }
}
