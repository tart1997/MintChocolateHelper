using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.MintChocolateHelper.ModInterop;
using Microsoft.Xna.Framework;
using Monocle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[Tracked]
[CustomEntity("MintChocolateHelper/FusionHeartHalf")]

public class FusionHeartHalf : Entity
{
    private Wiggler ScaleWiggler;
    
    private readonly string spriteColor;
    private readonly float bloomStr;
    private readonly float collisionSpeedX;
    private readonly float collisionSpeedY;
    private readonly float frictionX;
    private readonly float frictionY;
    private readonly bool hasLight;
    private readonly bool rightHalf;
    
    private Sprite sprite;
    private Sprite outline;
    private ParticleType shineParticle;
    private Wiggler moveWiggler;
    private Vector2 moveWiggleDir;
    private BloomPoint bloom;
    private VertexLight light;
    private float timer;
    private bool collected;
    private readonly bool autoPulse;
    private float bounceSfxDelay;
    private float respawnTimer;

    private Vector2 speed;
    
    private static readonly ParticleType P_Regen = new (Seeker.P_Regen);
    
    public FusionHeartHalf(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        spriteColor = data.Attr("color", "9a9ddb");
        bloomStr = data.Float("bloom", 0.75f);
        collisionSpeedX = data.Float("collisionSpeedX", 1f);
        collisionSpeedY = data.Float("collisionSpeedY", 1f);
        frictionX = data.Float("frictionX", 1f);
        frictionY = data.Float("frictionY", 1f);
        hasLight = data.Bool("light", true);
        rightHalf = data.Bool("rightHalf");
        autoPulse = true;
        speed = Vector2.Zero;
        Add(new MirrorReflection());
    }
    
    public FusionHeartHalf(Vector2 position, Vector2 halfSpeed, string halfColor, float halfBloom, float halfCSpeedX, float halfCSpeedY, float halfFricX, float halfFrictY, bool halfHasLight, bool halfRightHalf) : base(position)
    {
        spriteColor = halfColor;
        bloomStr = halfBloom;
        collisionSpeedX = halfCSpeedX;
        collisionSpeedY = halfCSpeedY;
        frictionX = halfFricX;
        frictionY = halfFrictY;
        hasLight = halfHasLight;
        rightHalf = halfRightHalf;
        autoPulse = true;
        speed = halfSpeed;
        Add(new MirrorReflection());
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (scene is not Level level) return;
        
        if (!rightHalf)
        {
            Add(sprite = GFX.SpriteBank.Create("FusionHeartLeft"));
            Add(outline = GFX.SpriteBank.Create("FusionHeartLeftOutline"));
        }
        else
        {
            Add(sprite = GFX.SpriteBank.Create("FusionHeartRight"));
            Add(outline = GFX.SpriteBank.Create("FusionHeartRightOutline"));
        }
        
        sprite.SetColor(Calc.HexToColor(spriteColor));
        sprite.Play("idle");
        
        sprite.OnLoop = delegate (string anim) {
            if (!Visible || anim != "spin" || !autoPulse)
                return;

            Audio.Play("event:/game/general/crystalheart_pulse", Position);
            ScaleWiggler.Start();
            ((Level)Scene).Displacement.AddBurst(Position, 0.35f, 8f, 48f, 0.25f);
        };
        
        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));
        
        Add(ScaleWiggler = Wiggler.Create(0.5f, 4f, delegate (float f)
        {
            sprite.Scale = Vector2.One * (1f + f * 0.25f);
        }));
        
        Add(bloom = new BloomPoint(bloomStr, 16f));

        Color value = Calc.HexToColor(spriteColor);
        shineParticle = new ParticleType(HeartGem.P_BlueShine)
        {
            Color = value
        };
        
        value = Color.Lerp(value, Color.White, 0.5f);
        Add(light = new VertexLight(value, hasLight ? 1f : 0f, 32, 64));
        moveWiggler = Wiggler.Create(0.8f, 2f);
        moveWiggler.StartZero = true;
        Add(moveWiggler);
        
    }

    public override void Update()
    {
        
        bounceSfxDelay -= Engine.DeltaTime;
        timer += Engine.DeltaTime;

        if (collected && respawnTimer > 0f)
            respawnTimer -= Engine.DeltaTime;
        if (collected && respawnTimer <= 0f)
        {
            respawnTimer = 0f;
            collected = false;
            Collidable = true;
            Visible = true;
            Audio.Play("event:/game/general/diamond_return", Position);
            bloom.Alpha = bloomStr;
            light.Alpha = (hasLight ? 1f : 0f);
            ScaleWiggler.Start();
        }
        
        base.Update();
        
        sprite.Position = Vector2.UnitY * (float)Math.Sin(timer * 2f) * 2f + moveWiggleDir * moveWiggler.Value * -8f;

        List<Sprite> sprites = [outline];

        foreach (Sprite other in sprites)
        {
            other.Position = sprite.Position;
            other.Scale = sprite.Scale;
            if (other.CurrentAnimationID != sprite.CurrentAnimationID)
            {
                other.Play(sprite.CurrentAnimationID);
            }
            other.SetAnimationFrame(sprite.CurrentAnimationFrame);
        }
        
        if (Visible && Scene.OnInterval(0.1f))
        {
            SceneAs<Level>().Particles.Emit(shineParticle, 1, Center, Vector2.One * 8f);
        }
        
        Position += speed;
        
        speed.X = Calc.Approach(speed.X, 0, 2.65f * frictionX * Engine.DeltaTime);
        speed.Y = Calc.Approach(speed.Y, 0, 2.65f * frictionY * Engine.DeltaTime);

        if (speed is { X: 0f, Y: 0f })
        {
            Position = Position.Round();
        }

        foreach (Spring spring in Scene.Entities.FindAll<Spring>().Where(spring => spring.CollideRect(new Rectangle((int)(Position.X - Width / 2), (int)(Position.Y - Height / 2), (int)Width, (int)Height))))
        {
            spring.BounceAnimate();
            
            bool isCeilingSpring = FrostHelperImports.IsCeilingSpring?.Invoke(spring) ?? false;
            Vector2 getSpringSpeedMultiplier = FrostHelperImports.GetSpringSpeedMultiplier?.Invoke(spring) ?? Vector2.One;
            
            if (isCeilingSpring)
            {
                speed.Y = 2f;
                speed *= getSpringSpeedMultiplier;
            }
            else switch (spring.Orientation)
            {
                case Spring.Orientations.Floor:
                    speed.Y = -2f;
                    speed *= getSpringSpeedMultiplier;
                    break;
                case Spring.Orientations.WallLeft:
                    speed.X = 2.5f;
                    speed *= getSpringSpeedMultiplier;
                    break;
                case Spring.Orientations.WallRight:
                    speed.X = -2.5f;
                    speed *= getSpringSpeedMultiplier;
                    break;
            }
        }

    }

    public void OnPlayer(Player player)
    {
        
        if (Scene is not Level level) return;
        
        Vector2 playerOffset = Center - player.Center;
        
        if (collected || ((Level)Scene).Frozen)
        {
            return;
        }
        if (bounceSfxDelay <= 0f)
        {
            Audio.Play("event:/game/general/crystalheart_bounce", Position);
            bounceSfxDelay = 0.1f;
        }
        int dashes = Math.Max(player.Dashes, 1);

        if (!player.DashAttacking)
        {
            if (player.Speed.Y < 0)
            {
                player.PointBounce(!rightHalf ? CenterRight : CenterLeft);
            }
            else
            {
                player.PointBounce(Center);
            }
            player.StateMachine.state = 0;
        }
        else
        {
            foreach (FusionHeartHalf half in Scene.Tracker.GetEntities<FusionHeartHalf>().Cast<FusionHeartHalf>())
            {
                if (!half.rightHalf)
                {
                    if (half.CollidePoint(Position + Vector2.UnitX * -14) && rightHalf)
                    {
                        Explode();
                        player.dashAttackTimer = 0;
                        return;
                    }
                }
                else
                {
                    if (half.CollidePoint(Position + Vector2.UnitX * 14) && !rightHalf)
                    {
                        Explode();
                        player.dashAttackTimer = 0;
                        return;
                    }
                }

                continue;

                void Explode()
                {
                    bool nearTarget = false;
                    Vector2 targetCenter = 0.5f * (Center + half.Center);
                    
                    half.RemoveSelf();
                    RemoveSelf();

                    FusionHeart fusionHeart = new FusionHeart(targetCenter,"ff4fed",0.75f,true,true);
                    
                    foreach (FusionTarget target in Scene.Tracker.GetEntities<FusionTarget>().Cast<FusionTarget>().Where(target => (0.5f * (Center + half.Center) - target.Center).Length() <= 8))
                    {
                        nearTarget = true;
                        targetCenter = target.Center;
                        fusionHeart.Position = targetCenter;
                        target.RemoveSelf();
                    }
                    
                    Scene.Add(fusionHeart);
                        
                    P_Regen.Color = Calc.HexToColor(fusionHeart.spriteColor);
                    P_Regen.Color2 = Color.White;

                    player.ExplodeLaunch(!nearTarget ? fusionHeart.Center : targetCenter, false, true);

                    fusionHeart.Add(new Coroutine(fusionHeart.FullDashHitColliderDisableTimer()));

                    Audio.Play("event:/new_content/game/10_farewell/puffer_splode", !nearTarget ? fusionHeart.Center : targetCenter);

                    level.Shake();

                    if (!nearTarget)
                    {
                        level.Displacement.AddBurst(fusionHeart.Center, 0.4f, 12f, 36f, 0.5f);
                        level.Displacement.AddBurst(fusionHeart.Center, 0.4f, 24f, 48f, 0.5f);
                        level.Displacement.AddBurst(fusionHeart.Center, 0.4f, 36f, 60f, 0.5f);
                    }
                    else
                    {
                        level.Displacement.AddBurst(targetCenter, 0.4f, 12f, 36f, 0.5f);
                        level.Displacement.AddBurst(targetCenter, 0.4f, 24f, 48f, 0.5f);
                        level.Displacement.AddBurst(targetCenter, 0.4f, 36f, 60f, 0.5f);
                    }

                    if (!nearTarget)
                    {
                        for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
                        {
                            Vector2 position = fusionHeart.Center + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
                            level.Particles.Emit(P_Regen, position, num);
                        }
                    }
                    else
                    {
                        for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
                        {
                            Vector2 position = targetCenter + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
                            level.Particles.Emit(P_Regen, position, num);
                        }
                    }

                }
            }

            if (Math.Abs(playerOffset.Y) > Math.Abs(playerOffset.X))
            {
                if (playerOffset.Y < (Collider.Height/2) - 1f)
                {
                    player.Rebound(-Math.Sign(player.Speed.X));
            
                    speed.Y = -2f * collisionSpeedY;
                    return;
                }
                if (playerOffset.Y > (Collider.Height/2) - 1f)
                {
                    player.Rebound(-Math.Sign(player.Speed.X));
                    
                    Add(new Coroutine(HalfDashHitColliderDisableTimer()));
            
                    speed.Y = 2f * collisionSpeedY;
                    return;
                }
            }
            else
            {
                if (playerOffset.X < (Collider.Width/2) - 1f)
                {
                    player.Rebound(-Math.Sign(player.Speed.X));

                    if (!(player.Speed.Y < 0))
                    {
                        Add(new Coroutine(HalfDashHitColliderDisableTimer()));
                    }
                    else if (player.Speed.Y < 0 && speed.Y < 0)
                    {
                        speed.Y -= 0.7f * collisionSpeedY;
                    }

                    speed.X = -2f * collisionSpeedX;
                    return;
                }
                if (playerOffset.X > (Collider.Width/2) - 1f)
                {
                    player.Rebound(-Math.Sign(player.Speed.X));
                    
                    if (!(player.Speed.Y < 0))
                    {
                        Add(new Coroutine(HalfDashHitColliderDisableTimer()));
                    }
                    else if (player.Speed.Y < 0 && speed.Y < 0)
                    {
                        speed.Y -= 0.7f * collisionSpeedY;
                    }
            
                    speed.X = 2f * collisionSpeedX;
                    return;
                }   
            }
        }

        player.Dashes = dashes;
        moveWiggler.Start();
        ScaleWiggler.Start();
        moveWiggleDir = (Center - player.Center).SafeNormalize(Vector2.UnitY);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
    }
    
    public IEnumerator HalfDashHitColliderDisableTimer()
    {
        if (Scene is not Level level) yield break;
        
        Collidable = false;

        yield return 5 / 60f;
        
        Collidable = true;
    }
}