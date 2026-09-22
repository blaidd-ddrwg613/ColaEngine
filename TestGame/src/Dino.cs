using ColaEngine;
using ColaEngine.Graphics;

namespace TestGame;

public class Dino
{
    private AnimatedSprite _sprite;

    private bool _isMoving = false;

    public Dictionary<string, Animation> Animations;

    public Dino(AnimatedSprite sprite)
    {
       _sprite = sprite;
       Animations = new Dictionary<string, Animation>();
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(GameTime gameTime)
    {
        
    }
}