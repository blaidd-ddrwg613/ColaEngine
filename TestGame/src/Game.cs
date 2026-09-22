using System.Numerics;
using ColaEngine;
using ColaEngine.Graphics;
using Raylib_cs;

namespace TestGame;

public class Game : GameBase
{
    private AnimatedSprite _dino;
        
    public Game() {}

    protected override void LoadContent()
    {
        base.LoadContent();
        var dinoTex = Raylib.LoadTexture("resources/textures/dino_walk.png");
        var dinoAtlas = new TextureAtlas(dinoTex);
        var frameSize = 48;
        
        dinoAtlas.AddRegion("dino_walk_0", 0, 0, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_1", 48, 0, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_2", 48 * 2, 0, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_3", 48 * 3, 0, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_4", 48 * 4, 0, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_5", 48 * 5, 0, frameSize, frameSize);
        List<TextureRegion> walkFrames = dinoAtlas.ReturnRegions();
        
        Animation dinoWalkAnim = new Animation();
        dinoWalkAnim.AddFrames(walkFrames);
        dinoAtlas.AddAnimation("walk", dinoWalkAnim);

        _dino = dinoAtlas.CreateAnimatedSprite("walk");
        _dino.CenterOrigin();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            _dino.FlipX = true;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            _dino.FlipX = false;
        }
        
        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            _dino.FlipY = true;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            _dino.FlipY = false;
        }
        
        _dino.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _dino.Draw(new Vector2(100, 100));
    }
    
}