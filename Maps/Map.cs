using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using TestGame.GameObjects;
using Newtonsoft.Json;
using TiledCS;
using System.Security.Cryptography.Pkcs;
using TestGame.GameObjects.Textures;
using NLog;

namespace TestGame.Maps {
    internal class Map : GameObject {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        private TiledMap _map;
        private Dictionary< int, TiledTileset > _tilesets;

        private Dictionary< string, nint > _imgPtrs;

        private bool _loaded = false;
        public bool IsLoaded => _loaded;

        public Map(GameContext context, string mapName) {
            RendererPtr = context.RendererPtr;
            _map = new(mapName);
            _imgPtrs = [];
            _tilesets = _map.GetTiledTilesets(Path.Join(Path.GetDirectoryName(mapName), "Maps"));
            Load();
        }

        public void Reload() {
            Unload();
            Load();
        }

        public void Unload() {
            if (!_loaded) {
                _log.Error("Map already unloaded.");
                return;
            }

            _loaded = false;
            foreach (var imgPtr in _imgPtrs.Values) {
                SDL.FreeSurface(imgPtr);
            }
        }

        public void Load() {
            if (_loaded) {
                _log.Error("Map already loaded. Please unload/reload first.");
                return;
            }

            // Find the tileset that contains the tile
            var layers = _map.Layers.Where(l => l.type == TiledLayerType.TileLayer);

            void load_layer(TiledLayer l) {
                for (int y = 0; y < l.height; y++) {
                    for (int x = 0; x < l.width; x++) {
                        var index = ( y * l.width ) + x;
                        var gid = l.data[ index ];
                        var mapTileset = _map.GetTiledMapTileset(gid);
                        var tileset = _tilesets[ mapTileset.firstgid ];

                        if (_imgPtrs.ContainsKey(tileset.Name)) {
                            continue;
                        }

                        nint ptr = Image.Load(tileset.Image.source);
                        if (ptr == nint.Zero) {
                            _log.Error($"Unable to load Image [{tileset.Name}] {tileset.Image.source}");
                            continue;
                        }

                        _imgPtrs.Add(tileset.Name, ptr);
                        _log.Info($"Loaded {tileset.Name} from {tileset.Image.source}");
                    }
                }
            }

            foreach (var layer in layers) {
                load_layer(layer);
            }

            if (_imgPtrs.Count == 0) {
                _log.Error("No images loaded.");
                return;
            }

            _loaded = true;
        }

        public override void Draw() {
            var layers = _map.Layers.Where(l => l.type == TiledLayerType.TileLayer);
            if (_imgPtrs.Count == 0) {
                _log.Error("No images loaded. Cannot Draw() blanks!");
                return;
            }

            foreach (var layer in layers) {
                // _log.Debug($"Drawing layer {layer.name}");
                RenderLayer(layer);
            }
        }

        /// <summary>
        /// Renders a given Layer
        /// </summary>
        /// <param name="layer"></param>
        // TODO: Fix this to actually render the layer to SDL Texture*
        private void RenderLayer(TiledLayer layer) {
            for (int y = 0; y < layer.height; y++) {
                for (int x = 0; x < layer.width; x++) {
                    var index = ( y * layer.width ) + x;
                    var gid = layer.data[ index ];
                    var tileX = ( x * _map.TileWidth );
                    var tileY = ( y * _map.TileHeight );

                    // Skip empty tiles
                    if (gid == 0) {
                        continue;
                    }

                    // Find the tileset that contains the tile
                    var mapTileset = _map.GetTiledMapTileset(gid);
                    var tileset = _tilesets[ mapTileset.firstgid ];
                    var rect = _map.GetSourceRect(mapTileset, tileset, gid);
                    var tile = _map.GetTiledTile(mapTileset, tileset, gid);

                    // Skip non-tiles
                    if (tile is null) {
                        continue;
                    }

                    if (!_imgPtrs.ContainsKey(tileset.Name)) {
                        _log.Error($"Unable to locate Tileset \"{tileset.Name}\"!");
                        continue;
                    }

                    nint img = _imgPtrs[ tileset.Name ];

                    if (img == nint.Zero) {
                        _log.Warn($"No image loaded for tile {tileset.Name} | {tileset.Image.source}");
                        continue;
                    }

                    Rect srcRect = new() {
                        X = rect.x,
                        Y = rect.y,
                        W = rect.width,
                        H = rect.height
                    };

                    Rect dstRect = new() {
                        X = tileX,
                        Y = tileY,
                        W = rect.width,
                        H = rect.height
                    };

                    nint text = SDL.CreateTextureFromSurface(RendererPtr, img);

                    _ = SDL.RenderCopyEx(RendererPtr, text, ref srcRect, ref dstRect, 0, nint.Zero, RendererFlip.None);
                    _ = SDL.RenderFillRect(RendererPtr, ref dstRect);
                    //_log.Info($"Rendered Tile src: {DispRect(srcRect)}; dst: {DispRect(dstRect)}, img: {tileset.Image.source}");
                }
            }
        }

        private string DispRect(Rect rect) {
            return $"X: {rect.X}; Y: {rect.Y}; W: {rect.W}; H: {rect.H}";
        }

        public override void Update(Event e) { }
    }
}