using ColaEngine;
using Raylib_cs;
using TestGame.Scenes;

namespace TestGame;

public class Game : GameBase
{
    public Game() : base(800, 600, "Test Game")
    {
        
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        
        
        SceneManager.ChangeScene(new MainMenuScene());
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (SceneManager.CurrentScene is not MainMenuScene)
        {
            Raylib.SetExitKey(0);
        }
        else
        {
            Raylib.SetExitKey(KeyboardKey.Escape);
        }

        SceneManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        SceneManager.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        SceneManager.UnloadContent();

        base.UnloadContent();
    }
}