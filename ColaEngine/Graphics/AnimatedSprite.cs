using System;
using Raylib_cs;

namespace ColaEngine.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            _currentFrame = 0;
            _elapsed = TimeSpan.Zero;

            if (_animation.Frames.Count > 0)
            {
                Region = _animation.Frames[0];
            }
        }
    }
    /// <summary>
    /// Creates a new animated sprite.
    /// </summary>
    public AnimatedSprite() { }

    /// <summary>
    /// Creates a new animated sprite with the specified frames and delay.
    /// </summary>
    /// <param name="animation">The animation for this animated sprite.</param>
    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public void Play(Animation animation)
    {
        Animation = animation;
    }

    public void Update(GameTime gameTime)
    {
        if (_animation == null || _animation.Frames.Count == 0)
        {
            return;
        }

        _elapsed += GameTime.ToTimeSpan(gameTime.DeltaTime);

        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currentFrame = (_currentFrame + 1) % _animation.Frames.Count;
            Region = _animation.Frames[_currentFrame];
        }
    }
}