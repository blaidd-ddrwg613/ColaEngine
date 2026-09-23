using ColaEngine.Scenes;
using Raylib_cs;

namespace ColaEngine;

public abstract class GameBase
{
    public int Width { get; } = 800;
    public int Height { get; } = 600;
    public string Title { get; } = "Test Game";
    public Color ClearColor { get; set; } = Color.Black;
    public bool ExitOnEscape { get; set; } = true;
    
    public static SceneManager SceneManager { get; private set; }
    
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
        InitializeWindow();
        Initialize();
        LoadContent();

        while (!Raylib.WindowShouldClose())
        {
            var gameTime = new GameTime(Raylib.GetFrameTime());

            Update(gameTime);
            
            DrawWindow(gameTime);
        }

        UnloadContent();
        Raylib.CloseWindow();
    }

    private void InitializeWindow()
    {
        Raylib.InitWindow(Width, Height, Title);
    }

    private void DrawWindow(GameTime gameTime)
    {
        BeginDraw();
        
        Raylib.ClearBackground(ClearColor);
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
    
    protected virtual void Initialize() { }
    protected virtual void LoadContent() { }

    protected virtual void Update(GameTime gameTime)
    { }
    protected virtual void Draw(GameTime gameTime) { }
    protected virtual void UnloadContent() { }
}