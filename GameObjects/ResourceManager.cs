using NLog;
using SDL2.TTF;

namespace TestGame.GameObjects {
    /// <summary>
    /// The global Resource Pool
    /// </summary>
    internal static class ResourceManager {
        private static readonly SortedDictionary< string, IGameObject > Resources;

        private static readonly Logger Log;

        private static int _nextUnknownId = 0;

        /// <summary>
        /// A static constructor initializing the Resource Pool
        /// </summary>
        static ResourceManager() {
            Log = LogManager.GetCurrentClassLogger();
            Resources = [];
            Log.Info("Initialized");
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> resource to the Resource Pool
        /// </summary>
        /// <typeparam name="T">An <see cref="IGameObject"/></typeparam>
        /// <param name="name">The name of the resource</param>
        /// <param name="resource">A <typeparamref name="T"/> containing the actual resource</param>
        public static void Add< T >(string name, T resource) where T : IGameObject {
            if (Resources.ContainsKey(name)) {
                Log.Error($"Resource with name {name} already exists");
                return;
            }

            Resources.Add(name, resource);
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> resource to the Resource Pool
        /// </summary>
        /// <typeparam name="T">An <see cref="IGameObject"/></typeparam>
        /// <param name="resource">A <typeparamref name="T"/> containing the actual resource</param>
        public static void Add< T >(T? resource) where T : IGameObject {
            if (resource is null) {
                Log.Error($"Unable to load {typeof(T).Name} resource.");
                return;
            }

            Add(resource.Name ?? $"resource/unknown{_nextUnknownId++}", resource);
        }

        /// <summary>
        /// Gets an <see cref="IGameObject"/> from the Resource Pool
        /// </summary>
        /// <param name="name">The name referring the <see cref="IGameObject"/>. Format is resourceType/name</param>
        /// <returns>If the resource exists, a non-null <see cref="IGameObject"/></returns>
        public static IGameObject? Get(string name) {
            return Resources.GetValueOrDefault(name);
        }

        /// <summary>
        /// Gets a <typeparamref name="T"/> from the Resource Pool
        /// </summary>
        /// <typeparam name="T">An <see cref="IGameObject"/></typeparam>
        /// <param name="name">The name referring the <see cref="IGameObject"/>. Format is resourceType/name</param>
        /// <returns>If the resource exists, a non-null <typeparamref name="T"/></returns>
        public static T? Get< T >(string name) where T : IGameObject {
            if (Resources.TryGetValue(name, out IGameObject? resource)) return (T)resource;

            Log.Error($"Resource with name {name} does not exist");
            return default;
        }

        /// <summary>
        /// Gets an <see cref="Audio"/> Game Object
        /// </summary>
        /// <param name="audioName">The name of the <see cref="Audio"/> to obtain.</param>
        /// <remarks>The <paramref name="audioName"/> should only contain the name of the audio reference as the resourceType is already specified</remarks>
        /// <returns>If the resource exists, a non-null <see cref="Audio"/> Game Object will be returned</returns>
        public static Audio? GetAudio(string audioName) {
            if (Resources.Where(x => x.Value is Audio)
                    .FirstOrDefault(x => x.Key == $"audio/{audioName}")
                    .Value is Audio audio) return audio;

            Log.Error($"Audio with name {audioName} does not exist");
            return null;
        }

        /// <summary>
        /// Removes an <see cref="IGameObject"/> from the Resource Pool.
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <remarks><paramref name="name"/> must follow resourceType/name format</remarks>
        /// <returns>True if the resource was successfully removed, else False</returns>
        public static bool Remove(string name) {
            bool ret = Resources.Remove(name);

            if (ret)
                Log.Info($"Successfully removed resource with the name {name}.");
            else
                Log.Error($"Resource with name {name} does not exist");

            return ret;
        }
    }
}