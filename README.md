# TestGame

## Loading LDtk maps

`LdtkProject` loads LDtk project JSON, including embedded or external level files.
It exposes worlds, levels, tile layers, IntGrid cell values, entities, and custom
field values. Tileset image paths are resolved relative to the `.ldtk` file.
Raylib textures are loaded lazily when a tile is drawn and released when the
project is disposed, so dispose the project before closing the Raylib window.

```csharp
using ColaEngine.Graphics;
using Raylib_cs;

// Call after Raylib.InitWindow().
LdtkProject map = LdtkProject.Load("resources/ldtk/Maps.ldtk");

// Inside the normal Raylib drawing pass.
map.Draw(); // Draw all levels at their LDtk world positions.
// Or: map.DrawLevel("Level_0");

// Read authored game data.
LdtkLevel level = map.Levels[0];
foreach (LdtkLayer layer in level.Layers)
{
    if (layer.Type == "IntGrid")
    {
        int cell = layer.GetIntGridValue(0, 0);
    }

    foreach (LdtkEntity entity in layer.Entities)
    {
        if (entity.TryGetField("Health", out var health))
        {
            int value = health.GetInt32();
        }
    }
}

// Call from your scene's UnloadContent(), before Raylib.CloseWindow().
map.Dispose();
```

The loader preserves LDtk layer data for gameplay code and draws tile/auto-tile
layers and entity tiles with layer offsets, visibility, opacity, and flip flags.
Call `DrawWorld(worldIid)` to render a specific world in a multi-world project.
