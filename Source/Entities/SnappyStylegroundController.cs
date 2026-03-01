using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

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
                case Godrays godrays:
                    godrays.fade = level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case WindPetals windPetals:
                    MintChocolateHelperModule.WindPetalsFade.SetValue(windPetals, level.Session.GetFlag(windPetals.OnlyIfFlag) ? 1 : 0);
                    break;
            }
        }
        foreach (Backdrop backdrop in level.Foreground.Backdrops.Where(backdrop => backdrop.Tags.Contains(SnapTag)))
        {
            switch (backdrop)
            {
                case Godrays godrays:
                    godrays.fade = level.Session.GetFlag(godrays.OnlyIfFlag) ? 1 : 0;
                    break;
                case WindPetals windPetals:
                    MintChocolateHelperModule.WindPetalsFade.SetValue(windPetals, level.Session.GetFlag(windPetals.OnlyIfFlag) ? 1 : 0);
                    break;
            }
        }
    }
}
