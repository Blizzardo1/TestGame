using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.Config {
    public class MapConfig(string mapPath, string reference, string type) {
        /// <summary>
        /// Path to the map file
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; } = mapPath;

        /// <summary>
        /// A Reference to the given map
        /// </summary>
        /// <example>maps/overworld/dungeon1</example>
        [JsonProperty("reference")]
        public string Reference { get; } = reference;

        /// <summary>
        /// The Type of Map
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = type;
    }
}
