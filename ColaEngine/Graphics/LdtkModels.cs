using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;

namespace ColaEngine.Graphics;

public sealed class LdtkWorld
{
    public string Identifier { get; internal set; } = string.Empty;
    public string Iid { get; internal set; } = string.Empty;
    public string Layout { get; internal set; } = string.Empty;
    public int GridWidth { get; internal set; }
    public int GridHeight { get; internal set; }
    public IReadOnlyList<LdtkLevel> Levels { get; internal set; } = Array.Empty<LdtkLevel>();
}

public sealed class LdtkLevel
{
    public string Identifier { get; internal set; } = string.Empty;
    public string Iid { get; internal set; } = string.Empty;
    public int Uid { get; internal set; }
    public Vector2 WorldPosition { get; internal set; }
    public int WorldDepth { get; internal set; }
    public int PixelWidth { get; internal set; }
    public int PixelHeight { get; internal set; }
    public string? BackgroundColor { get; internal set; }
    public IReadOnlyList<LdtkLayer> Layers { get; internal set; } = Array.Empty<LdtkLayer>();

    public IReadOnlyDictionary<string, JsonElement> Fields { get; internal set; } =
        new Dictionary<string, JsonElement>(StringComparer.Ordinal);
}

public sealed class LdtkLayer
{
    public string Identifier { get; internal set; } = string.Empty;
    public string Iid { get; internal set; } = string.Empty;
    public int LayerDefinitionUid { get; internal set; }
    public string Type { get; internal set; } = string.Empty;
    public string LevelIdentifier { get; internal set; } = string.Empty;
    public int GridWidth { get; internal set; }
    public int GridHeight { get; internal set; }
    public int GridSize { get; internal set; }
    public Vector2 PixelOffset { get; internal set; }
    public float Opacity { get; internal set; } = 1f;
    public bool Visible { get; internal set; } = true;
    public int? TilesetDefinitionUid { get; internal set; }
    public IReadOnlyList<int> IntGridValues { get; internal set; } = Array.Empty<int>();
    public IReadOnlyList<LdtkTile> Tiles { get; internal set; } = Array.Empty<LdtkTile>();
    public IReadOnlyList<LdtkEntity> Entities { get; internal set; } = Array.Empty<LdtkEntity>();

    public int GetIntGridValue(int column, int row)
    {
        if ((uint)column >= (uint)GridWidth)
        {
            throw new ArgumentOutOfRangeException(nameof(column));
        }

        if ((uint)row >= (uint)GridHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(row));
        }

        int index = row * GridWidth + column;
        if (index >= IntGridValues.Count)
        {
            throw new InvalidOperationException(
                $"Layer '{Identifier}' does not contain IntGrid data for ({column}, {row}).");
        }

        return IntGridValues[index];
    }
}

public sealed class LdtkTile
{
    public Vector2 Position { get; internal set; }
    public Vector2 SourcePosition { get; internal set; }
    public int SourceWidth { get; internal set; }
    public int SourceHeight { get; internal set; }
    public int TileId { get; internal set; }
    public int TilesetDefinitionUid { get; internal set; }
    public bool FlipX { get; internal set; }
    public bool FlipY { get; internal set; }
    public float Alpha { get; internal set; } = 1f;
}

public sealed class LdtkEntity
{
    public string Identifier { get; internal set; } = string.Empty;
    public string Iid { get; internal set; } = string.Empty;
    public int DefinitionUid { get; internal set; }
    public Vector2 Position { get; internal set; }
    public Vector2 Pivot { get; internal set; }
    public int Width { get; internal set; }
    public int Height { get; internal set; }
    public IReadOnlyList<string> Tags { get; internal set; } = Array.Empty<string>();

    public IReadOnlyDictionary<string, JsonElement> Fields { get; internal set; } =
        new Dictionary<string, JsonElement>(StringComparer.Ordinal);

    public LdtkEntityTile? Tile { get; internal set; }

    public bool TryGetField(string identifier, out JsonElement value)
    {
        return Fields.TryGetValue(identifier, out value);
    }

    public T? GetField<T>(string identifier)
    {
        if (!Fields.TryGetValue(identifier, out JsonElement value))
        {
            throw new KeyNotFoundException(
                $"Entity '{Identifier}' does not have a field named '{identifier}'.");
        }

        return value.Deserialize<T>();
    }
}

public sealed class LdtkEntityTile
{
    public int TilesetDefinitionUid { get; internal set; }
    public Vector2 SourcePosition { get; internal set; }
    public int SourceWidth { get; internal set; }
    public int SourceHeight { get; internal set; }
}

public sealed class LdtkTilesetDefinition
{
    public int Uid { get; internal set; }
    public string Identifier { get; internal set; } = string.Empty;
    public string? RelativePath { get; internal set; }
    public int TileGridSize { get; internal set; }
    public int PixelWidth { get; internal set; }
    public int PixelHeight { get; internal set; }
    public int Spacing { get; internal set; }
    public int Padding { get; internal set; }
}

public sealed class LdtkLayerDefinition
{
    public int Uid { get; internal set; }
    public string Identifier { get; internal set; } = string.Empty;
    public string Type { get; internal set; } = string.Empty;
    public int GridSize { get; internal set; }

    public IReadOnlyList<LdtkIntGridValueDefinition> IntGridValues { get; internal set; } =
        Array.Empty<LdtkIntGridValueDefinition>();
}

public sealed class LdtkIntGridValueDefinition
{
    public int Value { get; internal set; }
    public string? Identifier { get; internal set; }
    public string Color { get; internal set; } = string.Empty;
}