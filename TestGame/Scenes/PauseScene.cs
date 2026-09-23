using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using Raylib_cs;

namespace TestGame.Scenes;

public sealed class PauseScene : Scene
{
    public override void Update(GameTime gameTime)
    {
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            SceneManager.PopScene();
        }

        if (Input.IsActionPressed(InputAction.Back))
        {
            SceneManager.ChangeScene(new MainMenuScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Raylib.DrawRectangle(0, 0, 800, 600, new Color(0, 0, 0, 160));

        Raylib.DrawText("PAUSED", 330, 200, 40, Color.White);
        Raylib.DrawText("ENTER: resume", 305, 270, 22, Color.LightGray);
        Raylib.DrawText("BACKSPACE: main menu", 265, 305, 22, Color.LightGray);
    }
}