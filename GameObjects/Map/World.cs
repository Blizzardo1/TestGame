using Newtonsoft.Json;

using DotTiled;
using DotTiled.Serialization;
using TestGame.GameObjects.Textures;
using System.Collections.Concurrent;
using TestGame.GameObjects.Characters;
using SharpSDL3.Structs;
using SharpSDL3.Enums;
using SharpSDL3;

namespace TestGame.GameObjects.Map; 

public record TextureData(Textures.Texture Texture, Rect Rect);

public class World(nint rendererPtr, string map) : GameObject {

    private readonly static Log _log = Log.GetCurrentClassLogger(LogCategory.Custom, "Game");

    private FRect sourceRect;
    private FRect destinationRect;

    private Dictionary<uint, TextureData> _textures = [];
    private DotTiled.Map? _map;

    private ConcurrentBag<Entity> _entities = [];
    public List<Entity> Entities => [.. _entities];

    [JsonProperty("name")]
    public string? WorldName { get; set; }

    [JsonProperty("path")]
    public string? WorldPath { get; set; }

    public bool Initialized { get; private set; } = false;

    private void CreateEntities(ObjectLayer objectLayer ) {
        foreach (DotTiled.Object obj in objectLayer.Objects) {
            _log.Debug($"Found object in layer {objectLayer.Name}: class {obj?.Type} {obj?.Name} ({obj?.X}, {obj?.Y})");
            if (obj is null) {
                _log.Error($"Object in layer {objectLayer.Name} is null");
                continue;
            }

            if (!obj.TryGetProperty("EntityType", out IProperty<string> entityType)) {
                _log.Info($"Object in layer {objectLayer} has no EntityType");
                continue;
            }

            Entity e = EntityFactory.CreateEntity(obj?.Name, entityType.Value, RendererPtr);
            e.X = obj!.X;
            e.Y = obj!.Y;
            e.Initialize();

            switch (e) {
                case Player:
                    _entities.Add((Player)e);
                    break;
                case Enemy:
                    _entities.Add((Enemy)e);
                    break;
                case Npc:
                    // Not Implemented yet
                    break;
            }
        }
        _log.Info("Entities Added");
    }

    private  void LoadTilesets(Tileset tileset, string mapPath) {
        if (tileset.Image is null) {
            _log.Error($"Tileset {tileset.Name} has no image");
            return;
        }
        
        nint texture = Sdl.LoadTexture(RendererPtr, Path.Combine(Path.GetDirectoryName(mapPath)!, tileset.Image.Value.Source));
        if (texture == nint.Zero) {
            _log.Error($"Failed to load texture for tileset {tileset.Source}: {Sdl.GetError()}");
            return;
        }

        int x = 0, y = 0;
        for (int c = 0; c < tileset.TileCount; c++) {
            if (x > (tileset.Columns * tileset.TileWidth) - tileset.TileWidth) {
                x = 0;
                y += (int)tileset.TileHeight + (int)tileset.Spacing + (int)tileset.Margin;
            }

            var tileImage = new Rect {
                X = x,
                Y = y,
                W = (int)tileset.TileWidth,
                H = (int)tileset.TileHeight
            };

            uint rId = tileset.FirstGID + (uint)c;

            if (!_textures.ContainsKey(rId)) {
                Textures.Texture text = new TileTexture(rId, RendererPtr, texture, tileImage.W, tileImage.H) {
                    X = tileImage.X,
                    Y = tileImage.Y
                };
                TextureData data = new(text, tileImage);
                _textures.Add(rId, data);
            }
            x += (int)tileset.TileWidth + (int)tileset.Spacing + (int)tileset.Margin;
        }
    }

    private void BuildLayers(TileLayer tileLayer, DotTiled.Map map, string mapPath) {
        uint[] ids = tileLayer.Data.Value.GlobalTileIDs.Value;
        if (ids.Length == 0) {
            _log.Error($"No tiles found in layer {tileLayer.Name}");
            return;
        }

        map?.Tilesets.ForEach(tileset => {
            LoadTilesets(tileset, mapPath);
        });
    }

    private void BuildLayer(DotTiled.Map map, string mapPath, BaseLayer layer) {
        if (layer is ObjectLayer objlayer) {
            switch (objlayer.Name) {
                case "Entities":
                    CreateEntities(objlayer);
                    break;
            }
        }
        _log.Debug($"New Entities: {string.Join(',', _entities!.Select(e => e.Name))}");

        if (layer is TileLayer tileLayer) {
            BuildLayers(tileLayer, map, mapPath);
        }
    }

    private void ConstructMap(DotTiled.Map map, string mapPath) {
        _textures = [];
        map.Layers.ForEach(layer => {
            BuildLayer(map, mapPath, layer);
        });
    }

    private DotTiled.Map LoadMap(string mapPath) {
        string fullPath = mapPath;
        _log.Debug($"Loading {fullPath}");
        Loader loader = Loader.Default();
        DotTiled.Map dmap = loader.LoadMap(fullPath);
        ConstructMap(dmap, mapPath);
        return dmap;
    }

    public void Cleanup() {
        if (_textures is null) {
            return;
        }

        foreach (TextureData texture in _textures!.Values) {
            Sdl.Free(texture.Texture);
        }
        _textures.Clear();
    }

    public override void Initialize() {
        if (Initialized) {
            return;
        }

        _entities = [];
        RendererPtr = rendererPtr;
        _map = LoadMap(map);

        Initialized = true;
    }

    private void RenderLayer(TileLayer layer) {
        int x = 0, y = 0;
        foreach (uint tileId in layer.Data.Value.GlobalTileIDs.Value) {
            if (x >= layer.Width) {
                x = 0;
                y++;
            }

            if (tileId is 0) {
                x++;
                continue;
            }

            if (!_textures!.TryGetValue(tileId, out TextureData? td)) {
                _log.Error($"Texture for tile {tileId} not found");
                continue;
            }

            if (td.Texture is null) {
                _log.Error($"Texture for tile {tileId} is null");
                return;
            }

            // - Should be in the update function, but whatever
            sourceRect = sourceRect with {
                X = td.Rect.X,
                Y = td.Rect.Y,
                W = td.Rect.W,
                H = td.Rect.H
            };

            destinationRect = destinationRect with {
                X = x * td.Rect.W,
                Y = y * td.Rect.H,
                W = td.Rect.W,
                H = td.Rect.H
            };
            // - End Rant

            // Declare a variable of type FPoint to pass as a ref argument
            FPoint center = new() { X = 0, Y = 0 };

            bool res = Sdl.RenderTextureRotated(RendererPtr, td.Texture, ref sourceRect, ref destinationRect,
                0, ref center, FlipMode.None);
            if (!res) {
                _log.Error($"Error rendering tile: {Sdl.GetError()}");
                return;
            }
            x++;
        }
    }

    public override void Draw() {
        if (_map is null) {
            return;
        }

        // Render current map
        foreach (TileLayer layer in _map.Layers.Where(l => l is TileLayer).ToArray().Cast<TileLayer>()) {
            RenderLayer(layer);
        }

        // Render game objects
        foreach (Entity entity in _entities) {
            entity.Draw();
        }

    }

    public override void Update(Event e) {
        // Update current map
        foreach (Entity entity in _entities) {
            entity.Update(e);
        }

        SortByZOrder();
    }

    private void SortByZOrder() {
        if (_entities is null || _entities.IsEmpty) {
            return;
        }

        // Lower Z values are drawn first        
        List<Entity> sorted = [.. _entities.OrderByDescending(e => e.Z + (e.Height / 2))];
        _entities = [.. sorted];
    }
}
