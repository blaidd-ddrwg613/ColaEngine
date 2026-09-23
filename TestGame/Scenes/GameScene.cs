using System.Numerics;
using ColaEngine;
using ColaEngine.Camera;
using ColaEngine.Graphics;
using ColaEngine.Input;
using ColaEngine.Scenes;
using Raylib_cs;

namespace TestGame.Scenes;

public sealed class GameScene : Scene
{
    private Tilemap _tilemap = null!;
    private Dino _dino = null!;
    private GameCamera _camera = null!;

    public override void LoadContent()
    {
        _tilemap = Tilemap.FromFile("resources/textures/example-tilemap-definition.xml");
        _dino = new Dino();
        _dino.Sprite.Scale = new Vector2(2, 2);
        
        _camera = new GameCamera(800, 600);
    }

    public override void Update(GameTime gameTime)
    {
        if (Raylib.IsWindowResized())
        {
            _camera.SetViewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }

        if (Input.IsActionPressed(InputAction.Pause))
        {
            SceneManager.PushScene(new PauseScene());
            return;
        }

        _dino.Update(gameTime);
        _camera.Follow(_dino.Position);
    }

    public override void Draw(GameTime gameTime)
    {
        _camera.BeginMode();
        
        _tilemap.Draw();
        _dino.Draw();
        
        _camera.EndMode();
    }

    public override void UnloadContent()
    {
        _dino.Unload();
    }
}