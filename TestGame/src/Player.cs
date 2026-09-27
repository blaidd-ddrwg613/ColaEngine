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

    public PlayerDirection Direction { get; set; } = PlayerDirection.Front;

    private string? _currentAniamtion;

    private bool _isMoving;

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


        PlayerSprite = new AnimatedSprite(Animations.Get("idle_front"));
        PlayerSprite.CenterOrigin();
    }
    
    public void Update(GameTime gameTime)
    {
        Vector2 direction = Input.GetMovementVector();
        if (direction != Vector2.Zero) Direction = GetDirection(direction);
        var prefix = _isMoving ? "walk" : "idle";
        var facing = Direction switch
        {
            PlayerDirection.Front => "_front",
            PlayerDirection.Back => "_back",
            PlayerDirection.Left => "_side",
            PlayerDirection.Right => "_side",
            // _ => "_front"
        };

        _isMoving = direction != Vector2.Zero;

        PlayerSprite.FlipX = Direction == PlayerDirection.Left;
        SetAnimation($"{prefix}{facing}");

        Position += direction * MovementSpeed * gameTime.DeltaTime;
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
        if (_currentAniamtion == name) return;
        
        PlayerSprite.Play(Animations.Get(name));
        _currentAniamtion = name;
        
        Logger.Debug($"Player Animation Changed: {name}");
    }
}