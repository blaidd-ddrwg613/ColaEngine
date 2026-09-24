using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using Gum;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using Raylib_cs;
using TestGame.Screens;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{
    private MainScreenRuntime _menueScreen;
    
    public override void LoadContent()
    {
        base.LoadContent();

        _menueScreen = new MainScreenRuntime();
        _menueScreen.AddToRoot();
    }

    public override void Update(GameTime gameTime)
    {
        // TODO : Why Does this take 30 - 60 sec to change over and spam the console with loading the new scenes resource (atlas and dino)
        _menueScreen.ButtonStartGame.Click += (sender, args) =>
        {
            _menueScreen.RemoveFromRoot();
            SceneManager.ChangeScene(new GameScene());
        };
        // TODO This Will close the window but not close out the games resources it will just hang.
        _menueScreen.ButtonCloseGame.Click += (sender, args) => Raylib.CloseWindow();
        
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            SceneManager.ChangeScene(new GameScene());
        }
    }
}