using System.Numerics;
using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using Raylib_cs;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{
    public override void Update(GameTime gameTime)
    {
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            SceneManager.ChangeScene(new GameScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var posX = Raylib.GetScreenWidth() / 2;
        var posY = Raylib.GetScreenHeight() / 2;

        var titleText = "Test Game";
        var titleFontSize = 40;
        var titleLen = Raylib.MeasureText(titleText, titleFontSize);
        var titlePosVec = new Vector2(posX - (titleLen / 2), 180);

        var startText = "Press ENTER to start";
        var startFontSize = 24;
        var startLen = Raylib.MeasureText(startText, startFontSize);
        var startPosVec = new Vector2(posX -+ (startLen /  2), titlePosVec.Y + 100);
        
        Raylib.DrawText(titleText, (int)titlePosVec.X, (int)titlePosVec.Y, titleFontSize, Color.White);
        Raylib.DrawText(startText, (int)startPosVec.X, (int)startPosVec.Y, startFontSize, Color.LightGray);
    }
}