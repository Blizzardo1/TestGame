namespace TestGame.GameObjects {
    [Flags]
    public enum AudioAllow {
        FrequencyChange = 1,
        FormatChange = 2,
        ChannelsChange = 4,
        SamplesChange = 8,
        All = FrequencyChange | FormatChange | ChannelsChange | SamplesChange
    }
}