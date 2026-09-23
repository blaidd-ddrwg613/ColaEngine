using System.Collections.Generic;
using System.Numerics;
using ColaEngine;
using ColaEngine.Graphics;
using Raylib_cs;

namespace TestGame;

public class Dino
{
    private const int FrameWidth = 48;
    private const int FrameHeight = 48;
    private const int WalkFrameCount = 6;

    private readonly Texture2D _texture;
    private readonly AnimatedSprite _sprite;

    public AnimationSet Animations { get; }

    private bool _isWalking;

    public Vector2 Position { get; set; } = new Vector2(100, 100);
    public float Speed { get; set; } = 100f;

    public Dino()
    {
        Animations = new AnimationSet();
        
        _texture = Raylib.LoadTexture("resources/textures/dino_walk.png");

        TextureRegion fullRegion = new TextureRegion(
            _texture,
            new Rectangle(0, 0, _texture.Width, _texture.Height)
        );

        Tileset tileset = new Tileset(fullRegion, FrameWidth, FrameHeight);

        Animation _idleAnimation = new Animation(
            new List<TextureRegion>
            {
                tileset.GetTile(0)
            },
            TimeSpan.FromMilliseconds(250)
        );

        List<TextureRegion> walkFrames = new();

        for (int i = 0; i < WalkFrameCount - 1; i++)
        {
            walkFrames.Add(tileset.GetTile(i));
        }

        Animation _walkAnimation = new Animation(
            walkFrames,
            TimeSpan.FromMilliseconds(100)
        );
        
        Animations.Add("idle", _idleAnimation);
        Animations.Add("walk", _walkAnimation);

        _sprite = new AnimatedSprite(Animations.Get("idle"));
        _sprite.CenterOrigin();
    }

    public void Update(GameTime gameTime)
    {
        Vector2 direction = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            direction.X -= 1;
        }

        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            direction.X += 1;
        }

        bool shouldWalk = direction != Vector2.Zero;

        if (shouldWalk)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * Speed * gameTime.DeltaTime;

            if (direction.X < 0)
            {
                _sprite.FlipX = true;
            }
            else if (direction.X > 0)
            {
                _sprite.FlipX = false;
            }
        }

        if (shouldWalk != _isWalking)
        {
            _isWalking = shouldWalk;
            _sprite.Play(_isWalking ? Animations.Get("walk") : Animations.Get("idle"));
        }

        _sprite.Update(gameTime);
    }

    public void Draw()
    {
        _sprite.Draw(Position);
    }

    public void Unload()
    {
        Raylib.UnloadTexture(_texture);
    }
}