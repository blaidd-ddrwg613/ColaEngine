using System;
using Gum;
using Gum.Expressions;
using Gum.Managers;
using Gum.Wireframe;
using Raylib_cs;

namespace ColaEngine;

public abstract class GameBase
{
    public int Width { get; } = 800;
    public int Height { get; } = 600;
    public string Title { get; } = "Test Game";
    public Color ClearColor { get; set; } = Color.Black;

    public bool WindowResizable { get; set; } = true;
    
    public static SceneManager SceneManager { get; private set; }

    public static GumService GumUI => GumService.Default;

    private static bool _exitRequested;
    
    protected GameBase() {}
    
    protected GameBase(int width, int height, string title)
    {
        Width = width;
        Height = height;
        Title = title;
        
        SceneManager = new SceneManager();
    }

    public void Run()
    {
        _exitRequested = false;
        
        InitializeWindow();
        InitializeGum();
        Initialize();
        LoadContent();

        while (!Raylib.WindowShouldClose() && !_exitRequested)
        {
            var gameTime = new GameTime(Raylib.GetFrameTime());
            
            if (Raylib.IsWindowResized())
            {
                OnResize(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            }

            Update(gameTime);
            
            DrawWindow(gameTime);
        }

        UnloadContent();
        Raylib.CloseWindow();
    }

    protected virtual void Initialize() { }
    protected virtual void LoadContent() { }

    protected virtual void Update(GameTime gameTime)
    {
        GumUI.Update(gameTime.DeltaTime);
    }
    protected virtual void Draw(GameTime gameTime) { }
    protected virtual void UnloadContent() { }
    protected virtual void OnResize(int width, int height) { }
    
    public static void RequestExit()
    {
        _exitRequested = true;
    }
    
    private void InitializeWindow()
    {
        if (WindowResizable)
        {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        }
        
        Raylib.InitWindow(Width, Height, Title);
    }

    private void DrawWindow(GameTime gameTime)
    {
        BeginDraw();

        Raylib.ClearBackground(ClearColor);
        GumUI.Draw();
        Draw(gameTime);
        
        EndDraw();
    }

    private void BeginDraw()
    {
        Raylib.BeginDrawing();
    }

    private void EndDraw()
    {
        Raylib.EndDrawing();
    }

    private void InitializeGum()
    {
        GraphicalUiElement.CanvasWidth = Width;
        GraphicalUiElement.CanvasHeight = Height;
        
        GumUI.Initialize("resources/GumProject/TestGameGumProject.gumx");
        // This assumes that your project has at least 1 screen
        if(ObjectFinder.Self.GumProjectSave.Screens.Count == 0)
        {
            throw new Exception(
                "No screen found in the Gum project, " + 
                "did you add a Screen in the Gum tool?");
        }
        
        GumExpressionService.Initialize();
    }
}