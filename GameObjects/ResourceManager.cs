using NLog;
using SDL2.TTF;

namespace TestGame.GameObjects
{
    internal static class ResourceManager {
        private static readonly SortedDictionary< string, IGameObject > _resources;

        private static readonly Logger Log;

        private static int _nextUnknownId = 0;

        static ResourceManager()
        {
            Log = LogManager.GetCurrentClassLogger();
            _resources = [];
            Log.Info("Initialized");
        }
        
        public static void Add<T>(string name, T resource) where T : IGameObject
            
        {
            if (_resources.ContainsKey(name))
            {
                Log.Error($"Resource with name {name} already exists");
                return;
            }

            _resources.Add(name, resource);
        }

        public static void Add<T>(T? resource) where T : IGameObject
        {
            if(resource is null) {
                Log.Error($"Unable to load {typeof(T).Name} resource.");
                return;
            }
            
            Add(resource.Name ?? $"resource/unknown{_nextUnknownId++}", resource);
        }

        public static IGameObject Get(string name) {
            return _resources.TryGetValue(name, out IGameObject? resource) ? resource : default!;
        }

        public static T Get<T>(string name)
        {
            if (_resources.TryGetValue(name, out IGameObject? resource)) return (T)resource;
            
            Log.Error($"Resource with name {name} does not exist");
            return default!;

        }

        public static Audio GetAudio(string audioName) {
            if (_resources.Where(x => x.Value is Audio)
                    .FirstOrDefault(x => x.Key == $"audio/{audioName}")
                    .Value is Audio audio) return audio;
            
            Log.Error($"Audio with name {audioName} does not exist");
            return null!;

        }

        public static bool Remove(string name) {
            bool ret = _resources.Remove(name);

            if (ret)
                Log.Info($"Successfully removed resource with the name {name}.");
            else
                Log.Error($"Resource with name {name} does not exist");

            return ret;

        }
    }
}
