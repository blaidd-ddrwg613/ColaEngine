using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Raylib_cs;

namespace ColaEngine.Graphics;

/// <summary>
/// A loaded LDtk project and its level data. Dispose the project to release textures loaded for drawing.
/// </summary>
public sealed class LdtkProject : IDisposable
{
    private readonly Dictionary<int, Texture2D> _textures = new();
    private readonly Dictionary<int, LdtkTilesetDefinition> _tilesetsByUid = new();
    private bool _disposed;

    public string ProjectPath { get; }
    public string Identifier { get; }
    public string JsonVersion { get; }
    public string WorldLayout { get; }
    public int WorldGridWidth { get; }
    public int WorldGridHeight { get; }
    public int DefaultGridSize { get; }
    public IReadOnlyList<LdtkLevel> Levels { get; }
    public IReadOnlyList<LdtkWorld> Worlds { get; }
    public IReadOnlyList<LdtkTilesetDefinition> Tilesets { get; }
    public IReadOnlyList<LdtkLayerDefinition> LayerDefinitions { get; }

    private LdtkProject(
        string projectPath,
        string identifier,
        string jsonVersion,
        string worldLayout,
        int worldGridWidth,
        int worldGridHeight,
        int defaultGridSize,
        List<LdtkLevel> levels,
        List<LdtkWorld> worlds,
        List<LdtkTilesetDefinition> tilesets,
        List<LdtkLayerDefinition> layerDefinitions)
    {
        ProjectPath = projectPath;
        Identifier = identifier;
        JsonVersion = jsonVersion;
        WorldLayout = worldLayout;
        WorldGridWidth = worldGridWidth;
        WorldGridHeight = worldGridHeight;
        DefaultGridSize = defaultGridSize;
        Levels = levels.AsReadOnly();
        Worlds = worlds.AsReadOnly();
        Tilesets = tilesets.AsReadOnly();
        LayerDefinitions = layerDefinitions.AsReadOnly();

        foreach (LdtkTilesetDefinition tileset in tilesets)
        {
            _tilesetsByUid.Add(tileset.Uid, tileset);
        }
    }

    /// <summary>
    /// Loads an LDtk project (.ldtk) and any external level files it references.
    /// </summary>
    public static LdtkProject Load(string projectPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        string fullPath = Path.GetFullPath(projectPath);

        using FileStream projectStream = File.OpenRead(fullPath);
        using JsonDocument document = JsonDocument.Parse(projectStream);
        JsonElement root = document.RootElement;
        string projectDirectory = Path.GetDirectoryName(fullPath)!;
        JsonElement definitions = GetProperty(root, "defs");

        List<LdtkTilesetDefinition> tilesets = ParseTilesets(definitions);
        List<LdtkLayerDefinition> layerDefinitions = ParseLayerDefinitions(definitions);
        Dictionary<int, LdtkTilesetDefinition> tilesetsByUid = new();
        foreach (LdtkTilesetDefinition tileset in tilesets)
        {
            tilesetsByUid.Add(tileset.Uid, tileset);
        }

        List<LdtkLevel> levels = new();
        Dictionary<string, LdtkLevel> levelsByIid = new(StringComparer.Ordinal);
        JsonElement levelArray = GetArray(root, "levels");
        foreach (JsonElement levelData in levelArray.EnumerateArray())
        {
            LdtkLevel level = ParseLevel(levelData, projectDirectory, tilesetsByUid);
            levels.Add(level);
            levelsByIid.Add(level.Iid, level);
        }

        string projectLayout = GetNullableString(root, "worldLayout") ?? "MultiWorlds";
        ApplyLinearLayout(levels, projectLayout);
        List<LdtkWorld> worlds = ParseWorlds(
            root,
            projectDirectory,
            tilesetsByUid,
            levels,
            levelsByIid);

        return new LdtkProject(
            fullPath,
            GetString(root, "iid"),
            GetString(root, "jsonVersion"),
            GetNullableString(root, "worldLayout") ?? "MultiWorlds",
            GetNullableInt(root, "worldGridWidth") ?? 0,
            GetNullableInt(root, "worldGridHeight") ?? 0,
            GetInt(root, "defaultGridSize"),
            levels,
            worlds,
            tilesets,
            layerDefinitions);
    }

    /// <summary>
    /// Draws every level at its LDtk world position, with an optional world-space offset.
    /// </summary>
    public void Draw(Vector2 offset = default)
    {
        ThrowIfDisposed();
        foreach (LdtkLevel level in Levels)
        {
            DrawLevel(level, offset);
        }
    }

    /// <summary>
    /// Draws one level at its LDtk world position, with an optional world-space offset.
    /// </summary>
    public void DrawLevel(string identifier, Vector2 offset = default)
    {
        ThrowIfDisposed();
        foreach (LdtkLevel level in Levels)
        {
            if (string.Equals(level.Identifier, identifier, StringComparison.Ordinal))
            {
                DrawLevel(level, offset);
                return;
            }
        }

        throw new KeyNotFoundException($"LDtk project '{Identifier}' has no level named '{identifier}'.");
    }

    /// <summary>
    /// Draws every level belonging to a multi-world project world.
    /// </summary>
    public void DrawWorld(string worldIid, Vector2 offset = default)
    {
        ThrowIfDisposed();
        foreach (LdtkWorld world in Worlds)
        {
            if (string.Equals(world.Iid, worldIid, StringComparison.Ordinal))
            {
                foreach (LdtkLevel level in world.Levels)
                {
                    DrawLevel(level, offset);
                }

                return;
            }
        }

        throw new KeyNotFoundException($"LDtk project '{Identifier}' has no world with IID '{worldIid}'.");
    }

    private void DrawLevel(LdtkLevel level, Vector2 offset)
    {
        Vector2 levelOrigin = level.WorldPosition + offset;
        for (int layerIndex = level.Layers.Count - 1; layerIndex >= 0; layerIndex--)
        {
            LdtkLayer layer = level.Layers[layerIndex];
            if (!layer.Visible)
            {
                continue;
            }

            foreach (LdtkTile tile in layer.Tiles)
            {
                DrawTile(
                    tile.TilesetDefinitionUid,
                    tile.SourcePosition,
                    tile.SourceWidth,
                    tile.SourceHeight,
                    layer.GridSize,
                    layer.GridSize,
                    levelOrigin + layer.PixelOffset + tile.Position,
                    layer.Opacity * tile.Alpha,
                    tile.FlipX,
                    tile.FlipY);
            }

            foreach (LdtkEntity entity in layer.Entities)
            {
                if (entity.Tile is not null)
                {
                    DrawTile(
                        entity.Tile.TilesetDefinitionUid,
                        entity.Tile.SourcePosition,
                        entity.Tile.SourceWidth,
                        entity.Tile.SourceHeight,
                        entity.Width,
                        entity.Height,
                        levelOrigin + layer.PixelOffset +
                        entity.Position - entity.Pivot * new Vector2(entity.Width, entity.Height),
                        layer.Opacity,
                        false,
                        false);
                }
            }
        }
    }

    private void DrawTile(
        int tilesetUid,
        Vector2 sourcePosition,
        int sourceWidth,
        int sourceHeight,
        int destinationWidth,
        int destinationHeight,
        Vector2 position,
        float opacity,
        bool flipX,
        bool flipY)
    {
        Texture2D texture = GetTexture(tilesetUid);
        float alpha = Math.Clamp(opacity, 0f, 1f);
        byte alphaByte = (byte)Math.Round(alpha * 255f);
        Rectangle source = new(
            sourcePosition.X,
            sourcePosition.Y,
            flipX ? -sourceWidth : sourceWidth,
            flipY ? -sourceHeight : sourceHeight);
        Rectangle destination = new(position.X, position.Y, destinationWidth, destinationHeight);
        Raylib.DrawTexturePro(
            texture,
            source,
            destination,
            Vector2.Zero,
            0f,
            new Color((byte)255, (byte)255, (byte)255, alphaByte));
    }

    private Texture2D GetTexture(int tilesetUid)
    {
        if (_textures.TryGetValue(tilesetUid, out Texture2D texture))
        {
            return texture;
        }

        if (!_tilesetsByUid.TryGetValue(tilesetUid, out LdtkTilesetDefinition? tileset))
        {
            throw new InvalidDataException($"LDtk references missing tileset UID {tilesetUid}.");
        }

        if (string.IsNullOrWhiteSpace(tileset.RelativePath))
        {
            throw new InvalidDataException(
                $"LDtk tileset '{tileset.Identifier}' (UID {tilesetUid}) has no image path.");
        }

        string texturePath = Path.IsPathRooted(tileset.RelativePath)
            ? tileset.RelativePath
            : Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(ProjectPath)!,
                tileset.RelativePath
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar)));
        texture = Raylib.LoadTexture(texturePath);
        if (texture.Id == 0)
        {
            throw new IOException(
                $"Raylib could not load tileset image '{texturePath}' (UID {tilesetUid}).");
        }

        _textures.Add(tilesetUid, texture);
        return texture;
    }

    private static List<LdtkTilesetDefinition> ParseTilesets(JsonElement definitions)
    {
        List<LdtkTilesetDefinition> tilesets = new();
        JsonElement tilesetArray = GetArray(definitions, "tilesets");
        foreach (JsonElement item in tilesetArray.EnumerateArray())
        {
            tilesets.Add(new LdtkTilesetDefinition
            {
                Uid = GetInt(item, "uid"),
                Identifier = GetString(item, "identifier"),
                RelativePath = GetNullableString(item, "relPath"),
                TileGridSize = GetInt(item, "tileGridSize"),
                PixelWidth = GetInt(item, "pxWid"),
                PixelHeight = GetInt(item, "pxHei"),
                Spacing = GetInt(item, "spacing"),
                Padding = GetInt(item, "padding")
            });
        }

        return tilesets;
    }

    private static List<LdtkLayerDefinition> ParseLayerDefinitions(JsonElement definitions)
    {
        List<LdtkLayerDefinition> layerDefinitions = new();
        JsonElement layerArray = GetArray(definitions, "layers");
        foreach (JsonElement item in layerArray.EnumerateArray())
        {
            List<LdtkIntGridValueDefinition> values = new();
            JsonElement valueArray = GetArray(item, "intGridValues");
            foreach (JsonElement value in valueArray.EnumerateArray())
            {
                values.Add(new LdtkIntGridValueDefinition
                {
                    Value = GetInt(value, "value"),
                    Identifier = GetNullableString(value, "identifier"),
                    Color = GetString(value, "color")
                });
            }

            layerDefinitions.Add(new LdtkLayerDefinition
            {
                Uid = GetInt(item, "uid"),
                Identifier = GetString(item, "identifier"),
                Type = GetString(item, "type"),
                GridSize = GetInt(item, "gridSize"),
                IntGridValues = values.AsReadOnly()
            });
        }

        return layerDefinitions;
    }

    private static LdtkLevel ParseLevel(
        JsonElement data,
        string projectDirectory,
        IReadOnlyDictionary<int, LdtkTilesetDefinition> tilesetsByUid)
    {
        JsonElement layerData = GetProperty(data, "layerInstances");
        string? externalPath = GetNullableString(data, "externalRelPath");
        if (externalPath is not null)
        {
            string resolvedPath = Path.GetFullPath(Path.Combine(
                projectDirectory,
                externalPath
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar)));
            using FileStream externalStream = File.OpenRead(resolvedPath);
            using JsonDocument externalDocument = JsonDocument.Parse(externalStream);
            layerData = GetProperty(externalDocument.RootElement, "layerInstances");
            if (layerData.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException(
                    $"External LDtk level '{resolvedPath}' does not contain layerInstances.");
            }

            return ParseLevelWithLayers(data, layerData, tilesetsByUid);
        }

        if (layerData.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException(
                $"LDtk level '{GetString(data, "identifier")}' has no layerInstances or external level file.");
        }

        return ParseLevelWithLayers(data, layerData, tilesetsByUid);
    }

    private static LdtkLevel ParseLevelWithLayers(
        JsonElement data,
        JsonElement layerData,
        IReadOnlyDictionary<int, LdtkTilesetDefinition> tilesetsByUid)
    {
        List<LdtkLayer> layers = new();
        if (layerData.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement layer in layerData.EnumerateArray())
            {
                layers.Add(ParseLayer(layer, GetString(data, "identifier"), tilesetsByUid));
            }
        }

        return new LdtkLevel
        {
            Identifier = GetString(data, "identifier"),
            Iid = GetString(data, "iid"),
            Uid = GetInt(data, "uid"),
            WorldPosition = new Vector2(GetInt(data, "worldX"), GetInt(data, "worldY")),
            WorldDepth = GetInt(data, "worldDepth"),
            PixelWidth = GetInt(data, "pxWid"),
            PixelHeight = GetInt(data, "pxHei"),
            BackgroundColor = GetNullableString(data, "bgColor") ?? GetString(data, "__bgColor"),
            Layers = layers.AsReadOnly(),
            Fields = ParseFields(GetProperty(data, "fieldInstances"))
        };
    }

    private static LdtkLayer ParseLayer(
        JsonElement data,
        string levelIdentifier,
        IReadOnlyDictionary<int, LdtkTilesetDefinition> tilesetsByUid)
    {
        int? layerTilesetUid = GetNullableInt(data, "overrideTilesetUid")
                               ?? GetNullableInt(data, "__tilesetDefUid");
        int gridSize = GetInt(data, "__gridSize");
        List<int> intGridValues = new();
        JsonElement intGrid = GetArray(data, "intGridCsv");
        foreach (JsonElement value in intGrid.EnumerateArray())
        {
            intGridValues.Add(value.GetInt32());
        }

        if (GetString(data, "__type") == "IntGrid" &&
            intGridValues.Count != GetInt(data, "__cWid") * GetInt(data, "__cHei"))
        {
            throw new InvalidDataException(
                $"LDtk layer '{GetString(data, "__identifier")}' has an IntGrid size that does not match its dimensions.");
        }

        List<LdtkTile> tiles = new();
        AddTiles(GetArray(data, "autoLayerTiles"), layerTilesetUid, tilesetsByUid, gridSize, tiles);
        AddTiles(GetArray(data, "gridTiles"), layerTilesetUid, tilesetsByUid, gridSize, tiles);

        List<LdtkEntity> entities = new();
        JsonElement entityData = GetArray(data, "entityInstances");
        foreach (JsonElement entity in entityData.EnumerateArray())
        {
            entities.Add(ParseEntity(entity));
        }

        return new LdtkLayer
        {
            Identifier = GetString(data, "__identifier"),
            Iid = GetString(data, "iid"),
            LayerDefinitionUid = GetInt(data, "layerDefUid"),
            Type = GetString(data, "__type"),
            LevelIdentifier = levelIdentifier,
            GridWidth = GetInt(data, "__cWid"),
            GridHeight = GetInt(data, "__cHei"),
            GridSize = gridSize,
            PixelOffset = new Vector2(
                GetInt(data, "__pxTotalOffsetX"),
                GetInt(data, "__pxTotalOffsetY")),
            Opacity = GetFloat(data, "__opacity", 1f),
            Visible = GetProperty(data, "visible").ValueKind != JsonValueKind.False,
            TilesetDefinitionUid = layerTilesetUid,
            IntGridValues = intGridValues.AsReadOnly(),
            Tiles = tiles.AsReadOnly(),
            Entities = entities.AsReadOnly()
        };
    }

    private static void AddTiles(
        JsonElement tileArray,
        int? layerTilesetUid,
        IReadOnlyDictionary<int, LdtkTilesetDefinition> tilesetsByUid,
        int gridSize,
        List<LdtkTile> tiles)
    {
        if (tileArray.GetArrayLength() > 0 && gridSize <= 0)
        {
            throw new InvalidDataException("LDtk tile layers must have a positive grid size.");
        }

        foreach (JsonElement data in tileArray.EnumerateArray())
        {
            int tilesetUid = GetNullableInt(data, "tilesetUid") ?? layerTilesetUid
                ?? throw new InvalidDataException("LDtk tile has no tileset definition UID.");
            if (layerTilesetUid.HasValue && tilesetUid != layerTilesetUid.Value)
            {
                throw new InvalidDataException(
                    $"LDtk tile references tileset UID {tilesetUid}, but its layer uses UID {layerTilesetUid}.");
            }

            int[] position = GetIntArray(data, "px", 2);
            int[] source = GetIntArray(data, "src", 2);
            int flip = GetInt(data, "f");
            if (!tilesetsByUid.TryGetValue(tilesetUid, out LdtkTilesetDefinition? tileset))
            {
                throw new InvalidDataException($"LDtk tile references missing tileset UID {tilesetUid}.");
            }

            tiles.Add(new LdtkTile
            {
                Position = new Vector2(position[0], position[1]),
                SourcePosition = new Vector2(source[0], source[1]),
                SourceWidth = tileset.TileGridSize,
                SourceHeight = tileset.TileGridSize,
                TileId = GetInt(data, "t"),
                TilesetDefinitionUid = tilesetUid,
                FlipX = (flip & 1) != 0,
                FlipY = (flip & 2) != 0,
                Alpha = GetFloat(data, "a", 1f)
            });
        }
    }

    private static LdtkEntity ParseEntity(JsonElement data)
    {
        List<string> tags = new();
        JsonElement tagsData = GetProperty(data, "__tags");
        if (tagsData.ValueKind != JsonValueKind.Array)
        {
            tagsData = GetArray(data, "tags");
        }

        foreach (JsonElement tag in tagsData.EnumerateArray())
        {
            tags.Add(tag.GetString() ?? string.Empty);
        }

        LdtkEntityTile? tile = ParseEntityTile(GetProperty(data, "__tile"));
        int[] position = GetIntArray(data, "px", 2);
        string pivotProperty = GetProperty(data, "__pivot").ValueKind == JsonValueKind.Array
            ? "__pivot"
            : "pivot";
        Vector2 pivot = GetVector2(data, pivotProperty);

        return new LdtkEntity
        {
            Identifier = GetString(data, "__identifier"),
            Iid = GetString(data, "iid"),
            DefinitionUid = GetInt(data, "defUid"),
            Position = new Vector2(position[0], position[1]),
            Pivot = pivot,
            Width = GetInt(data, "width"),
            Height = GetInt(data, "height"),
            Tags = tags.AsReadOnly(),
            Fields = ParseFields(GetProperty(data, "fieldInstances")),
            Tile = tile
        };
    }

    private static LdtkEntityTile? ParseEntityTile(JsonElement data)
    {
        if (data.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        int x = GetInt(data, "x");
        int y = GetInt(data, "y");
        int width = GetInt(data, "w");
        int height = GetInt(data, "h");
        return new LdtkEntityTile
        {
            TilesetDefinitionUid = GetInt(data, "tilesetUid"),
            SourcePosition = new Vector2(x, y),
            SourceWidth = width,
            SourceHeight = height
        };
    }

    private static Dictionary<string, JsonElement> ParseFields(JsonElement fieldArray)
    {
        if (fieldArray.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("Expected LDtk JSON array property 'fieldInstances'.");
        }

        Dictionary<string, JsonElement> fields = new(StringComparer.Ordinal);
        foreach (JsonElement field in fieldArray.EnumerateArray())
        {
            fields[GetString(field, "__identifier")] = GetProperty(field, "__value").Clone();
        }

        return fields;
    }

    private static List<LdtkWorld> ParseWorlds(
        JsonElement root,
        string projectDirectory,
        IReadOnlyDictionary<int, LdtkTilesetDefinition> tilesetsByUid,
        List<LdtkLevel> allLevels,
        Dictionary<string, LdtkLevel> levelsByIid)
    {
        List<LdtkWorld> worlds = new();
        JsonElement worldArray = GetArray(root, "worlds");
        foreach (JsonElement data in worldArray.EnumerateArray())
        {
            List<LdtkLevel> levels = new();
            JsonElement worldLevels = GetArray(data, "levels");
            foreach (JsonElement levelReference in worldLevels.EnumerateArray())
            {
                string iid = GetString(levelReference, "iid");
                if (!levelsByIid.TryGetValue(iid, out LdtkLevel? level))
                {
                    if (GetProperty(levelReference, "identifier").ValueKind != JsonValueKind.String)
                    {
                        throw new InvalidDataException(
                            $"LDtk world references missing level IID '{iid}'.");
                    }

                    level = ParseLevel(levelReference, projectDirectory, tilesetsByUid);
                    levelsByIid.Add(level.Iid, level);
                    allLevels.Add(level);
                }

                levels.Add(level);
            }

            string layout = GetNullableString(data, "worldLayout") ?? "Free";
            ApplyLinearLayout(levels, layout);
            worlds.Add(new LdtkWorld
            {
                Identifier = GetString(data, "identifier"),
                Iid = GetString(data, "iid"),
                Layout = layout,
                GridWidth = GetInt(data, "worldGridWidth"),
                GridHeight = GetInt(data, "worldGridHeight"),
                Levels = levels.AsReadOnly()
            });
        }

        return worlds;
    }

    private static void ApplyLinearLayout(List<LdtkLevel> levels, string layout)
    {
        float x = 0f;
        float y = 0f;
        foreach (LdtkLevel level in levels)
        {
            if (layout == "LinearHorizontal")
            {
                level.WorldPosition = new Vector2(x, 0f);
                x += level.PixelWidth;
            }
            else if (layout == "LinearVertical")
            {
                level.WorldPosition = new Vector2(0f, y);
                y += level.PixelHeight;
            }
        }
    }

    private static JsonElement GetProperty(JsonElement element, string name)
    {
        return element.ValueKind == JsonValueKind.Object &&
               element.TryGetProperty(name, out JsonElement property)
            ? property
            : default;
    }

    private static JsonElement GetArray(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Expected LDtk JSON array property '{name}'.");
        }

        return value;
    }

    private static string GetString(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        if (value.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException($"Expected LDtk JSON string property '{name}'.");
        }

        return value.GetString()!;
    }

    private static string? GetNullableString(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        return value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? null
            : value.GetString();
    }

    private static int GetInt(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        if (value.ValueKind != JsonValueKind.Number)
        {
            throw new InvalidDataException($"Expected LDtk JSON integer property '{name}'.");
        }

        return value.GetInt32();
    }

    private static int? GetNullableInt(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        return value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? null
            : value.GetInt32();
    }

    private static float GetFloat(JsonElement element, string name, float defaultValue)
    {
        JsonElement value = GetProperty(element, name);
        return value.ValueKind == JsonValueKind.Number
            ? value.GetSingle()
            : defaultValue;
    }

    private static int[] GetIntArray(JsonElement element, string name, int expectedLength)
    {
        JsonElement value = GetProperty(element, name);
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Expected LDtk JSON array property '{name}'.");
        }

        int[] result = new int[expectedLength];
        int index = 0;
        foreach (JsonElement item in value.EnumerateArray())
        {
            if (index == expectedLength)
            {
                throw new InvalidDataException(
                    $"LDtk JSON array property '{name}' has more than {expectedLength} values.");
            }

            result[index++] = item.GetInt32();
        }

        if (index != expectedLength)
        {
            throw new InvalidDataException(
                $"LDtk JSON array property '{name}' must have {expectedLength} values.");
        }

        return result;
    }

    private static Vector2 GetVector2(JsonElement element, string name)
    {
        JsonElement value = GetProperty(element, name);
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Expected LDtk JSON array property '{name}'.");
        }

        System.Text.Json.JsonElement.ArrayEnumerator values = value.EnumerateArray();
        if (!values.MoveNext())
        {
            throw new InvalidDataException($"LDtk JSON array property '{name}' must have two values.");
        }

        float x = values.Current.GetSingle();
        if (!values.MoveNext())
        {
            throw new InvalidDataException($"LDtk JSON array property '{name}' must have two values.");
        }

        float y = values.Current.GetSingle();
        if (values.MoveNext())
        {
            throw new InvalidDataException($"LDtk JSON array property '{name}' has more than two values.");
        }

        return new Vector2(x, y);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (Texture2D texture in _textures.Values)
        {
            Raylib.UnloadTexture(texture);
        }

        _textures.Clear();
        _disposed = true;
    }
}