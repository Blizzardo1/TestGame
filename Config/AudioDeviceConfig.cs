using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.Config {
    internal class AudioDeviceConfig(int deviceIndex, string deviceName, string deviceType) {

        /// <summary>
        /// The Index of the gathered Audio Device
        /// </summary>
        [JsonProperty("index")]
        public int DeviceIndex { get; } = deviceIndex;

        /// <summary>
        /// The Name of the audio Device in Unix-friendly Format
        /// </summary>
        [JsonProperty("name")]
        public string DeviceName { get; } = deviceName;

        /// <summary>
        /// The Type of the Audio Device
        /// </summary>
        /// <example>input</example>
        [JsonProperty("type")]
        public string DeviceType { get; } = deviceType;

        public override string ToString() => $"{DeviceType}/{DeviceIndex}/{DeviceName}";
    }
}
