using System.Collections.Generic;

namespace ColaEngine.Graphics;

public class AnimationSet
{
    private readonly Dictionary<string, Animation> _animations = new();

    public void Add(string name, Animation animation)
    {
        _animations.Add(name, animation);
    }

    public Animation Get(string name)
    {
        return _animations[name];
    }
}