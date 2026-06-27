using System;
using System.Collections;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MintChocolateHelper.Entities;
[CustomEntity("MintChocolateHelper/JesusRefill")]
[Tracked]

public class JesusRefill : Entity
{
    private float respawnTimer;
    private readonly float respawnTime;
    private readonly bool oneUse;
    private readonly bool DisableQuickRespawn;
    private readonly bool UnregisterDeathInStats;
    
    private readonly ParticleType P_Shatter;
    private readonly ParticleType P_Regen;
    private readonly ParticleType P_Glow;
    
    private readonly Sprite sprite;
    private readonly Image outline;
    
    private readonly Wiggler wiggler;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly SineWave sine;
    
    public JesusRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        respawnTime = data.Float("respawnTime", 2.5f);
        oneUse = data.Bool("oneUse");
        DisableQuickRespawn = data.Bool("disableQuickRespawn");
        UnregisterDeathInStats = data.Bool("unregisterDeathInStats");
        
        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));
        
        P_Shatter = new ParticleType(Refill.P_Shatter) {
            Color = Color.Red,
            Color2 = Color.HotPink
        };

        P_Regen = new ParticleType(Refill.P_Regen) {
            Color = Color.Red,
            Color2 = Color.MediumVioletRed
        };

        P_Glow = new ParticleType(Refill.P_Glow){
            Color = Color.IndianRed,
            Color2 = Color.Magenta
        };
        
        Add(outline = new Image(GFX.Game["objects/MintChocolateHelper/Refills/JesusRefill/outline"]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/JesusRefill/idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle");
        sprite.CenterOrigin();
        
        Add(wiggler = Wiggler.Create(1f, 4f, v =>
        {
            sprite.Scale = Vector2.One * (1f + v * 0.2f);
        }));
        
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.8f, 16f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        sine.Randomize();
        
        Depth = -100;
        UpdateY();
    }
    
    public override void Update()
    {
        Level level = SceneAs<Level>();
        
        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn();
            }
        }
        else if (Scene.OnInterval(0.1f) && Collidable)
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }

        UpdateY();

        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
    }
    
    private void UpdateY()
    {
        sprite.Y = bloom.Y = sine.Value * 2f;
    }
    
    public override void Render()
    {
        if (sprite.Visible)
        {
            sprite.DrawOutline();
        }
        base.Render();
    }
    
    private void OnPlayer(Player player)
    {
        if (!MintChocolateHelperModule.Session.HasJesusRefill)
        {
            Audio.Play("event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player)));
            MintChocolateHelperModule.Session.HasJesusRefill = true;
            if (DisableQuickRespawn)
            {
                MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn = true;
            }
            respawnTimer = respawnTime;
        }
    }
    
    private void Respawn()
    {
        if (oneUse) return;
        
        Level level = SceneAs<Level>();
        
        if (!Collidable)
        {
            Collidable = true;
            sprite.Visible = true;
            outline.Visible = false;
            Depth = -100;
            wiggler.Start();
            Audio.Play("event:/game/general/diamond_return", Position);
            level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
        }
    }
    
    private IEnumerator RefillRoutine(Player player)
    {
        Level level = SceneAs<Level>();
        
        Celeste.Freeze(0.05f);
        yield return null;
        level.Shake();
        sprite.Visible = false;
        if (!oneUse)
        {
            outline.Visible = true;
        }
        Depth = 8999;
        yield return 0.05f;
        float num = player.Speed.Angle();
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num - MathF.PI / 2f);
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num + MathF.PI / 2f);
        SlashFx.Burst(Position, num);
    }
    
    internal static void Load()
    {
        On.Celeste.PlayerHair.GetHairColor += JesusRefillHairColor;
        On.Celeste.Player.Update += Resurrection;
    }

    internal static void Unload()
    {
        On.Celeste.PlayerHair.GetHairColor -= JesusRefillHairColor;
        On.Celeste.Player.Update -= Resurrection;
    }
    
    private static Color JesusRefillHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
    {
        return MintChocolateHelperModule.Session.HasJesusRefill ? Color.FromNonPremultiplied(201, 192, 187, 255) : orig(self, index);
    }
    
    private static void Resurrection(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        if (Engine.Scene is Level level)
        {
            JesusRefill jesusRefill = level.Tracker.GetEntity<JesusRefill>();

            if (jesusRefill != null && (Input.DashPressed || Input.CrouchDashPressed) && MintChocolateHelperModule.Session.PlayerIsPsuedoDead && MintChocolateHelperModule.Session.HasJesusRefill)
            {
                self.Add(new Coroutine(jesusRefill.Unkill()));
            }
        }
    }
    
    private IEnumerator Unkill()
    {
        if (Scene is Level level)
        {
            level.Wipe?.Cancel();

            Session session = level.Session;
            Player player = level.Tracker.GetEntity<Player>();

            foreach (PlayerDeadBody playerDeadBody in Scene.Tracker.GetEntitiesTrackIfNeeded<PlayerDeadBody>().Cast<PlayerDeadBody>())
            {
                if (playerDeadBody == null) break;

                playerDeadBody.hair.Entity = player;
                playerDeadBody.sprite.Entity = player;
                playerDeadBody.light.Entity = player;
                playerDeadBody.RemoveSelf();
            }

            if (UnregisterDeathInStats)
            {
                --session.Deaths;
                --session.DeathsInCurrentLevel;
                --SaveData.Instance.TotalDeaths;
                --SaveData.Instance.Areas_Safe[session.Area.ID].Modes[(int)session.Area.Mode].Deaths;
                Stats.Increment(Stat.DEATHS, -1);
                StatsForStadia.Increment(StadiaStat.DEATHS, -1);
            }

            player.Dead = false;
            player.Depth = MintChocolateHelperModule.Session.CDT_Depth;
            player.StateMachine.Locked = false;
            player.StateMachine.State = 0;
            player.Collidable = MintChocolateHelperModule.Session.CDT_Collidable;
            player.Visible = MintChocolateHelperModule.Session.CDT_Visible;
            if (Scene is not null) player.Scene = Scene;
            MintChocolateHelperModule.Session.PlayerIsPsuedoDead = false;
            MintChocolateHelperModule.Session.HasJesusRefill = false;
            MintChocolateHelperModule.Session.JesusRefillDisableQuickRespawn = false;
            player.UseRefill(false);

            //This kinda sucks... I would prefer to just kill whatever rouge tweener that forces me to do this, but I've tried everything I can think of to do so. ¯\_(ツ)_/¯

            yield return null;

            player.Sprite.Scale.X = 1;
        }
        
        if (oneUse)
        {
            RemoveSelf();
        }
    }
}
