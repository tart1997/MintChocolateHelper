using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MintChocolateHelper.Entities;

[CustomEntity("MintChocolateHelper/SpeedFlipRefill")]

public class SpeedFlipRefill : Entity
{
	private readonly ParticleType P_Shatter;
	private readonly ParticleType P_Regen;
	private readonly ParticleType P_Glow;

	private readonly Sprite sprite;
	private readonly Sprite flash;
	private readonly Image outline;

	private readonly Wiggler wiggler;
	private readonly BloomPoint bloom;
	private readonly VertexLight light;

	private Level level;

	private readonly SineWave sine;

	private readonly bool oneUse;

	private float respawnTimer;
	private readonly float respawnTime;
	
	public SpeedFlipRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
	{
		oneUse = data.Bool("oneUse");
		respawnTime = data.Float("respawnTime", 2.5f);
		
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
		
		Add(outline = new Image(GFX.Game["objects/MintChocolateHelper/Refills/SpeedFlipRefill/outline"]));
		outline.CenterOrigin();
		outline.Visible = false;
		
		Add(sprite = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/SpeedFlipRefill/idle"));
		sprite.AddLoop("idle", "", 0.1f);
		sprite.Play("idle");
		sprite.CenterOrigin();
		
		Add(flash = new Sprite(GFX.Game, "objects/MintChocolateHelper/Refills/SpeedFlipRefill/flash"));
		flash.Add("flash", "", 0.05f);
		flash.OnFinish = delegate
		{
			flash.Visible = false;
		};
		flash.CenterOrigin();
		
		Add(wiggler = Wiggler.Create(1f, 4f, v =>
		{
			sprite.Scale = flash.Scale = Vector2.One * (1f + v * 0.2f);
		}));
		
		Add(new MirrorReflection());
		Add(bloom = new BloomPoint(0.8f, 16f));
		Add(light = new VertexLight(Color.White, 1f, 16, 48));
		Add(sine = new SineWave(0.6f, 0f));
		
		sine.Randomize();
		
		UpdateY();
		
		Depth = -100;
	}
	
	public override void Added(Scene scene)
	{
		base.Added(scene);
		level = SceneAs<Level>();
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
		else if (Scene.OnInterval(0.1f))
		{
			level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
		}
		
		UpdateY();
		
		light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
		bloom.Alpha = light.Alpha * 0.8f;
		
		if (Scene.OnInterval(2f) && sprite.Visible)
		{
			flash.Play("flash", true);
			flash.Visible = true;
		}
	}
	
	private void Respawn()
	{
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
	
	private void UpdateY()
	{
		flash.Y = sprite.Y = bloom.Y = sine.Value * 2f;
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
		if (player.UseRefill(false))
		{
			Audio.Play("event:/game/general/diamond_touch", Position);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			Collidable = false;
			Add(new Coroutine(RefillRoutine(player)));
			respawnTimer = respawnTime;
		}
	}
	
	private IEnumerator RefillRoutine(Player player)
	{
		Celeste.Freeze(0.05f);
		yield return null;
		level.Shake();
		flash.Visible = false;
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
		if (oneUse)
		{
			RemoveSelf();
		}
	}
}