using System.Numerics;
using ColaEngine;
using ColaEngine.Graphics;
using ColaEngine.Input;
using Raylib_cs;

namespace TestGame;

public class Player
{
    public AnimatedSprite PlayerSprite { get; set; }

    public Vector2 Position { get; set; } = Vector2.Zero;
    
    public AnimationSet Animations { get; set; }

    public float MovementSpeed { get; set; } = 100f;

    private bool _isMoving;

    private int _tileWidth = 32;
    private int _tileHeight = 32;

    public Player()
    {
        Animations = new AnimationSet();
        
        var texture = Raylib.LoadTexture("resources/textures/player/Idle.png");
        var fullRegion = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));

        var tileSet = new Tileset(fullRegion, _tileWidth, _tileHeight);

        BuildIdleAnims(tileSet);
       

        PlayerSprite = new AnimatedSprite(Animations.Get("idle_front"));
        PlayerSprite.CenterOrigin();
    }
    
    public void Update(GameTime gameTime)
    {
        Vector2 direction = Input.GetMovementVector();

        bool shouldWalk = direction != Vector2.Zero;

        if (shouldWalk)
        {
            Position += direction * MovementSpeed * gameTime.DeltaTime;

            if (direction.X < 0)
            {
                PlayerSprite.Play(Animations.Get("idle_side"));
                PlayerSprite.FlipX = true;
            }
            if (direction.X > 0)
            {
                PlayerSprite.Play(Animations.Get("idle_side"));
                PlayerSprite.FlipX = false;
            }
            if (direction.Y < 0)
            {
                PlayerSprite.Play(Animations.Get("idle_back"));
            }

            if (direction.Y > 0)
            {
                PlayerSprite.Play(Animations.Get("idle_front"));
            }
        }

        PlayerSprite.Update(gameTime);
    }

    public void Draw()
    {
        PlayerSprite.Draw(Position);
    }

    private void BuildIdleAnims(Tileset tileSet)
    {
        var delay = 250;
        // Construct Each anim Animation from the Sprite Sheet
        var anim = new Animation(new List<TextureRegion>
        {
            tileSet.GetTile(0),
            tileSet.GetTile(1),
            tileSet.GetTile(2),
            tileSet.GetTile(3)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("idle_front", anim);

        anim = new Animation(new List<TextureRegion>
        {
            tileSet.GetTile(4),
            tileSet.GetTile(5),
            tileSet.GetTile(6),
            tileSet.GetTile(7)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("idle_back", anim);

        anim = new Animation(new List<TextureRegion>
        {
            tileSet.GetTile(8),
            tileSet.GetTile(9),
            tileSet.GetTile(10),
            tileSet.GetTile(11)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("idle_side", anim);
    }
}