using System.Numerics;
using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using ColaEngine.UI;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{
    private UISystem _system = UiManager.System;
        
    public override void LoadContent()
    {
        base.LoadContent();
        
        var panel = new Panel(_system);
        panel.Anchor = Anchor.Center;
        panel.Size.SetPixels(400, 400);
        UiManager.System.Root.AddChild(panel);

        var paragraph = new Paragraph(_system);
        paragraph.Text = "Hello World!";
        panel.AddChild(paragraph);
    }

    public override void Update(GameTime gameTime)
    {
        _system.Update(gameTime.DeltaTime);
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            SceneManager.ChangeScene(new GameScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _system.Draw();
    }
}