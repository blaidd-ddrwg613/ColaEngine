using System.Collections.Generic;
using System.IO;
using Raylib_cs;

namespace ColaEngine;

public class ResourceManager
{
    private static readonly Dictionary<string, Texture2D> _textures = new();

    private static readonly Dictionary<string, Font> _fonts = new();

    private static string _resourceFolderPath = "resources/";
    
    public static Texture2D LoadTexture(string path)
    {
        if (!_textures.TryGetValue(path, out var texture))
        {
            texture = Raylib.LoadTexture(Path.Combine(_resourceFolderPath, path));
            _textures[path] = texture;
        }
        return texture;
    }

    public static Font LoadFont(string path)
    {
        if (!_fonts.TryGetValue(path, out var font))
        {
            font = Raylib.LoadFont(Path.Combine(_resourceFolderPath, path));
            _fonts[path] = font;
        }

        return font;
    }
    

    public static void UnloadAll()
    {
        foreach (var texture in _textures.Values)
            Raylib.UnloadTexture(texture);
        _textures.Clear();
    }

    public static void SetResourcePath(string path)
    {
        _resourceFolderPath = path;
    }
}