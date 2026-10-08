using System.Numerics;
using ColaEngine;
using ColaEngine.Graphics;
using ColaEngine.Input;
using ColaEngine.utils;
using Raylib_cs;

namespace TestGame;

public enum PlayerDirection
{
    Front,
    Back,
    Left,
    Right
}

public class Player
{
    public AnimatedSprite PlayerSprite { get; set; }

    public Vector2 Position { get; set; } = Vector2.Zero;
    
    public AnimationSet Animations { get; set; }

    public float MovementSpeed { get; set; } = 100f;

    public float SprintModifier { get; set; } = 1.5f;

    public PlayerDirection Direction { get; set; } = PlayerDirection.Front;

    private string? _currentAnimation;

    private bool _isSprinting;

    private int _tileWidth = 32;
    private int _tileHeight = 32;

    public Player()
    {
        Animations = new AnimationSet();
        
        var texture = ResourceManager.LoadTexture("textures/player/Idle.png");
        var region = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
        var tileSet = new Tileset(region, _tileWidth, _tileHeight);
        BuildIdleAnims(tileSet);

        texture = ResourceManager.LoadTexture("textures/player/Walk.png");
        region = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
        tileSet = new Tileset(region, _tileWidth, _tileHeight);
        BuildWalkAnims(tileSet);

        texture = ResourceManager.LoadTexture("textures/player/Run.png");
        region = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
        tileSet = new Tileset(region, _tileWidth, _tileHeight);
        BuildRunAnims(tileSet);


        PlayerSprite = new AnimatedSprite(Animations.Get("idle_front"));
        PlayerSprite.CenterOrigin();
    }

    public void Update(GameTime gameTime)
    {
        var prefix = "idle";
        
        Vector2 direction = Input.GetMovementVector();
        
        _isSprinting = Input.IsActionDown(InputAction.Sprint);
        
        if (direction != Vector2.Zero)
        {
            Direction = GetDirection(direction);
            prefix = _isSprinting ? "run" : "walk";
        } else
        {
            prefix = "idle";
        }
        
        var facing = Direction switch
        {
            PlayerDirection.Front => "_front",
            PlayerDirection.Back => "_back",
            PlayerDirection.Left => "_side",
            PlayerDirection.Right => "_side"
        };

        PlayerSprite.FlipX = Direction == PlayerDirection.Left;
        SetAnimation($"{prefix}{facing}");

        var movementSpeed = _isSprinting ? MovementSpeed * SprintModifier : MovementSpeed;

        Position += direction * movementSpeed * gameTime.DeltaTime;
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

    private void BuildWalkAnims(Tileset tileset)
    {
        var delay = 100;

        var anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(0),
            tileset.GetTile(1),
            tileset.GetTile(2),
            tileset.GetTile(3),
            tileset.GetTile(4),
            tileset.GetTile(5)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("walk_front", anim);

        anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(6),
            tileset.GetTile(7),
            tileset.GetTile(8),
            tileset.GetTile(9),
            tileset.GetTile(10),
            tileset.GetTile(11)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("walk_back", anim);

        anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(12),
            tileset.GetTile(13),
            tileset.GetTile(14),
            tileset.GetTile(15),
            tileset.GetTile(16),
            tileset.GetTile(17)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("walk_side", anim);
    }

    private void BuildRunAnims(Tileset tileset)
    {
        var delay = 100;
        var anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(0),
            tileset.GetTile(1),
            tileset.GetTile(2),
            tileset.GetTile(3),
            tileset.GetTile(4),
            tileset.GetTile(5),
            tileset.GetTile(6),
            tileset.GetTile(7)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("run_front", anim);
        
        anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(8),
            tileset.GetTile(9),
            tileset.GetTile(10),
            tileset.GetTile(11),
            tileset.GetTile(12),
            tileset.GetTile(13),
            tileset.GetTile(14),
            tileset.GetTile(15)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("run_back", anim);
        
        anim = new Animation(new List<TextureRegion>
        {
            tileset.GetTile(16),
            tileset.GetTile(17),
            tileset.GetTile(18),
            tileset.GetTile(19),
            tileset.GetTile(20),
            tileset.GetTile(21),
            tileset.GetTile(22),
            tileset.GetTile(23)
        }, TimeSpan.FromMilliseconds(delay));
        Animations.Add("run_side", anim);
    }

    private PlayerDirection GetDirection(Vector2 movementVector)
    {
        var direction = Direction;
        
        if (movementVector.Y > 0) direction = PlayerDirection.Front;
        if (movementVector.Y < 0) direction = PlayerDirection.Back;
        if (movementVector.X > 0) direction = PlayerDirection.Right;
        if (movementVector.X < 0) direction = PlayerDirection.Left;
        
        return direction;
    }

    private void SetAnimation(string name)
    {
        if (_currentAnimation == name) return;
        
        PlayerSprite.Play(Animations.Get(name));
        _currentAnimation = name;
    }
}