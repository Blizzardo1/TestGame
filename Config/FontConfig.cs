using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.Config {
    public class FontConfig([JsonProperty("name")] string? name, [JsonProperty("font")] string? font) {
        public string? Name { get; } = name;
        public string? Font { get; } = font;
    }
}