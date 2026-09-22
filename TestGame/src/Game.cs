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
        dinoAtlas.AddRegion("dino_walk_1", 0, 48, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_2", 0, 48 * 2, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_3", 0, 48 * 3, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_4", 0, 48 * 4, frameSize, frameSize);
        dinoAtlas.AddRegion("dino_walk_5", 0, 48 * 5, frameSize, frameSize);
        List<TextureRegion> walkFrames = dinoAtlas.ReturnRegions();
        
        Animation dinoWalkAnim = new Animation();
        dinoWalkAnim.AddFrames(walkFrames);
        dinoAtlas.AddAnimation("walk", dinoWalkAnim);

        _dino = dinoAtlas.CreateAnimatedSprite("walk");
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        _dino.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _dino.Draw(new Vector2(100, 100));
    }
    
}