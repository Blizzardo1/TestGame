using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.Config {
    public class AudioConfig(string reference, string audioPath) {
        /// <summary>
        /// A Reference to an Audio File
        /// </summary>
        /// <example>audio/bgm/overworld</example>
        [JsonProperty("reference")]
        public string Reference { get; } = reference;

        [JsonProperty("path")]
        public string Path { get; } = audioPath;

        public override string ToString() => Reference;
    }
}