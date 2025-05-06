namespace TestGame.GameObjects;

internal enum AudioFormat {
    Unsigned8 = 0x0008,
    Signed8 = 0x8008,
    Unsigned16LeastSignificantBit = 0x0010,
    Signed16LeastSignificantBit = 0x8010,
    Unsigned16MostSignificantBit = 0x1010,
    Signed16MostSignedBit = 0x9010,
    Signed32LeastSignificantBit = 0x8020,
    Signed32MostSignificantBit = 0x9020,
    Float32LeastSignificantBit = 0x8120,
    Float32MostSignificantBit = 0x9120,
}