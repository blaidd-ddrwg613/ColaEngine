using ColaEngine;
using ColaEngine.Scenes;
using Raylib_cs;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{
    public override void Update(GameTime gameTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            SceneManager.ChangeScene(new GameScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Raylib.DrawText("TEST GAME", 280, 180, 40, Color.White);
        Raylib.DrawText("Press ENTER to start", 250, 250, 24, Color.LightGray);
    }
}