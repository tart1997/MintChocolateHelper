namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/JesusRefill")]
public class JesusRefill : Entity
{
    private float respawnTimer;
    private readonly float respawnTime;
    private readonly bool oneUse;
    internal readonly bool DisableQuickRespawn;
    internal readonly bool DontRegisterDeathInStats;
    internal readonly bool KeepFollowers;
    internal readonly bool TeleportToRefill;
    internal readonly bool StoreSpeed;
    internal readonly bool Redirectable;

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
        DontRegisterDeathInStats = data.Bool("dontRegisterDeathInStats");
        KeepFollowers = data.Bool("keepFollowers");
        TeleportToRefill = data.Bool("teleportToRefill");
        StoreSpeed = data.Bool("storeSpeed");
        Redirectable = data.Bool("redirectable");

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        P_Shatter = new ParticleType(Refill.P_Shatter) {
            Color = Color.DarkRed,
            Color2 = Color.White
        };

        P_Regen = new ParticleType(Refill.P_Regen) {
            Color = Color.IndianRed,
            Color2 = Color.White
        };

        P_Glow = new ParticleType(Refill.P_Glow) {
            Color = Color.SaddleBrown,
            Color2 = Color.SandyBrown
        };

        Add(outline = new Image(GFX.Game["objects/MintChocolateHelper/Refills/JesusRefill/outline"]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/JesusRefill/idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle");
        sprite.CenterOrigin();

        Add(wiggler = Wiggler.Create(1f, 4f, v => {
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
            SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
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
            MintChocolateHelperModule.Session.LastJesusRefill = this;
            respawnTimer = respawnTime;
        }
    }

    private void Respawn()
    {
        if (oneUse || Collidable) return;

        Collidable = true;
        sprite.Visible = true;
        outline.Visible = false;
        Depth = -100;
        wiggler.Start();
        Audio.Play("event:/game/general/diamond_return", Position);
        SceneAs<Level>().ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
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

    [OnLoad]
    internal static void Load()
    {
        On.Celeste.Player.Die += JesusRefillRefillDie;
        On.Celeste.Player.Update += Resurrection;
    }

    [OnUnload]
    internal static void Unload()
    {
        On.Celeste.Player.Die -= JesusRefillRefillDie;
        On.Celeste.Player.Update -= Resurrection;
    }
    
    internal static void ResetRefill()
    {
        MintChocolateHelperModule.Session.PlayerIsPseudoDead = false;
        MintChocolateHelperModule.Session.LastJesusRefill = null;
        MintChocolateHelperModule.Session.StoredSpeed = null;
    }

    private static PlayerDeadBody JesusRefillRefillDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (!MintChocolateHelperModule.Session.HasJesusRefill && SearchUtils.IfNone<CancelDeathTrigger>())
        {
            ResetRefill();
        }

        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    private static void Resurrection(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        
        JesusRefill jesusRefill = MintChocolateHelperModule.Session.LastJesusRefill;
        // if (jesusRefill != null && MintChocolateHelperModule.Session.JesusRefillBufferedTeleport)
        // {
        //     Level level = self.SceneAs<Level>();
        //     
        //     if (!MintChocolateHelperModule.Session.PseudoDeadDontRegisterDeathInStats)
        //     {
        //         level.Session.Deaths++;
        //         level.Session.DeathsInCurrentLevel++;
        //         SaveData.Instance.AddDeath(level.Session.Area);
        //     }
        //
        //     if (!MintChocolateHelperModule.Session.PseudoDeadKeepFollowers)
        //     {
        //         self.Leader.LoseFollowers();
        //     }
        //     
        //     self.Drop();
        //     self.LastBooster?.PlayerDied();
        //     level.InCutscene = false;
        //     level.Shake();
        //     Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
        //     Audio.Play("event:/char/madeline/predeath", self.Position);
        //
        //     level.Wipe?.Cancel();
        //     self.Position = jesusRefill.Center + new Vector2(0, (int)(self.Height / 2));
        //     MintChocolateHelperModule.Session.LastJesusRefill = null;
        //     self.UseRefill(false);
        //     MintChocolateHelperModule.Session.JesusRefillBufferedTeleport = false;
        //     if (jesusRefill.oneUse) jesusRefill.RemoveSelf();
        //
        //     return;
        // }

        if (jesusRefill != null && (Input.DashPressed || Input.CrouchDashPressed) && MintChocolateHelperModule.Session.PlayerIsPseudoDead && MintChocolateHelperModule.Session.HasJesusRefill)
        {
            self.Add(new Coroutine(jesusRefill.Unkill(self)));
        }
    }

    private IEnumerator Unkill(Player player)
    {
        Level level = player.SceneAs<Level>();
        level.Wipe?.Cancel();

        bool backupDashNeeded = false;
        bool crouched = false;
        
        if (Input.CrouchDash.bufferCounter > 0 || Input.Dash.bufferCounter > 0)
        {
            backupDashNeeded = true;
        }
        
        if (player.Ducking || Input.CrouchDash.bufferCounter > 0)
        {
            crouched = true;
        }
        
        if (TeleportToRefill) player.Position = Center + new Vector2(0, (int)(player.Height / 2));
        PseudoDeath.BasicUnkill(player, SearchUtils.GetEntity<PlayerDeadBody>(true));
        MintChocolateHelperModule.Session.LastJesusRefill = null;
        player.UseRefill(false);

        //This part of the code sucks... but I've tried literally everything else I can think of to fix the related bugs this area is fixing so ¯\_(ツ)_/¯

        yield return null;
        
        player.Sprite.Scale.X = 1;
        if (backupDashNeeded) player.StateMachine.State = player.StartDash();
        if (crouched) player.Ducking = true;

        level.Wipe?.Cancel();
        if (oneUse) RemoveSelf();
    }
}