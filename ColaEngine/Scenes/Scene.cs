using Gum;

namespace ColaEngine.Scenes;

public abstract class Scene
{
    protected SceneManager SceneManager { get; private set; } = null!;

    protected GumService GumUI => GameBase.GumUI;
    
    internal void SetSceneManager(SceneManager sceneManager)
    {
        SceneManager = sceneManager;
    }

    public virtual void LoadContent() { }
    public virtual void UnloadContent() { }
    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(GameTime gameTime) { }
}