using System.Numerics;
using Raylib_cs;

namespace ColaEngine.Graphics;

public class TextureRegion
{
    public Texture2D Texture { get; set; }
    
    public Rectangle Source { get; }

    public float Width => Source.Width;

    public float Height => Source.Height;

    public TextureRegion()
    { }

    public TextureRegion(Texture2D texture, Rectangle source)
    {
        Texture = texture;
        Source = source;
    }

    public void Draw(Rectangle dst, Vector2 origin, float rotation, Color tint)
    {
        Raylib.DrawTexturePro(Texture, Source, dst, origin, rotation, tint);
    }
}