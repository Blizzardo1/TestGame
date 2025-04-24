using Newtonsoft.Json;

namespace TestGame.Config;

public class AudioDeviceConfig {
    public AudioDeviceConfig(string resource) {
        string[] device = resource.Split('/');
        DeviceType = device[ 0 ];
        if (int.TryParse(device[ 1 ], out int deviceIndex)) {
            DeviceIndex = deviceIndex;
        }

        DeviceName = device[ 2 ];
    }

    [JsonConstructor]
    public AudioDeviceConfig(
        [JsonProperty("index")] int deviceIndex,
        [JsonProperty("name")] string deviceName,
        [JsonProperty("type")] string deviceType) {
        DeviceIndex = deviceIndex;
        DeviceName = deviceName;
        DeviceType = deviceType;
    }

    /// <summary>
    /// The Index of the gathered Audio Device
    /// </summary>
    [JsonProperty("index")]
    public int DeviceIndex { get; }

    /// <summary>
    /// The Name of the audio Device in Unix-friendly Format
    /// </summary>
    [JsonProperty("name")]
    public string? DeviceName { get; }

    /// <summary>
    /// The Type of the Audio Device
    /// </summary>
    /// <example>input</example>
    [JsonProperty("type")]
    public string? DeviceType { get; }

    public override string ToString() => $"{DeviceType}/{DeviceIndex}/{DeviceName}";
}