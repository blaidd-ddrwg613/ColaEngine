using ColaEngine;
using ColaEngine.Graphics;

namespace TestGame;

public class Game : GameBase
{
    private Tilemap _tilemap;
    private Dino _dino;
        
    public Game() {}

    protected override void LoadContent()
    {
        base.LoadContent();

        _tilemap = Tilemap.FromFile("resources/textures/example-tilemap-definition.xml");
        _dino = new Dino();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _dino.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _tilemap.Draw();
        _dino.Draw();
    }
    
}