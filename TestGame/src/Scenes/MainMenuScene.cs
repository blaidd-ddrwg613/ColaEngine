using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using Gum;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{

    private Button _button;
    public override void LoadContent()
    {
        base.LoadContent();
        
        var screen = ObjectFinder.Self.GumProjectSave.Screens[0]
            .ToGraphicalUiElement();
        screen.AddToRoot();
        
    }

    public override void Update(GameTime gameTime)
    {
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            SceneManager.ChangeScene(new GameScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {

    }
}