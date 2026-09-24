using ColaEngine;
using ColaEngine.Input;
using ColaEngine.Scenes;
using TestGame.Screens;

namespace TestGame.Scenes;

public sealed class MainMenuScene : Scene
{
    private MainScreenRuntime _menuScreen = null!;
    private bool _isStartingGame;

    public override void LoadContent()
    {
        base.LoadContent();

        _menuScreen = new MainScreenRuntime();
        _menuScreen.AddToRoot();

        _menuScreen.ButtonStartGame.Click += OnStartGameClicked;
        _menuScreen.ButtonCloseGame.Click += OnCloseGameClicked;
    }

    public override void Update(GameTime gameTime)
    {
        if (Input.IsActionPressed(InputAction.Confirm))
        {
            StartGame();
        }
    }

    public override void UnloadContent()
    {
        if (_menuScreen != null)
        {
            _menuScreen.ButtonStartGame.Click -= OnStartGameClicked;
            _menuScreen.ButtonCloseGame.Click -= OnCloseGameClicked;
            _menuScreen.RemoveFromRoot();
        }

        base.UnloadContent();
    }

    private void OnStartGameClicked(object? sender, EventArgs args)
    {
        StartGame();
    }

    private void OnCloseGameClicked(object? sender, EventArgs args)
    {
        GameBase.RequestExit();
    }

    private void StartGame()
    {
        if (_isStartingGame)
        {
            return;
        }

        _isStartingGame = true;
        SceneManager.ChangeScene(new GameScene());
    }
}