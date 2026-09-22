using System.Collections.Generic;
using System.Numerics;
using ColaEngine;
using ColaEngine.Graphics;
using Raylib_cs;

namespace TestGame;

public class Game : GameBase
{
    private AnimatedSprite _dino;
    private Tilemap _tilemap;
        
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
        
        
            
        Animation dinoWalkAnim = dinoAtlas.CreateAnimation({"dino_walk_0","dino_walk_1","dino_walk_2","dino_walk_3","dino_walk_4","dino_walk_5"},)
        dinoAtlas.AddAnimation("walk", dinoWalkAnim);

        _dino = dinoAtlas.CreateAnimatedSprite("walk");1
        _dino.CenterOrigin();

        _tilemap = Tilemap.FromFile("resources/textures/tilemap-definition.xml");
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
        _tilemap.Draw();
        _dino.Draw(new Vector2(100, 100));
    }
    
}