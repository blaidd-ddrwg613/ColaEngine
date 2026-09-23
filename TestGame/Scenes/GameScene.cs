using ColaEngine;
using ColaEngine.Graphics;
using ColaEngine.Scenes;
using Raylib_cs;

namespace TestGame.Scenes;

public sealed class GameScene : Scene
{
    private Tilemap _tilemap = null!;
    private Dino _dino = null!;

    public override void LoadContent()
    {
        _tilemap = Tilemap.FromFile("resources/textures/example-tilemap-definition.xml");
        _dino = new Dino();
    }

    public override void Update(GameTime gameTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.PushScene(new PauseScene());
            return;
        }

        _dino.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        _tilemap.Draw();
        _dino.Draw();
    }

    public override void UnloadContent()
    {
        _dino.Unload();
    }
}