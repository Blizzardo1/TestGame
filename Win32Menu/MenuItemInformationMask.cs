namespace TestGame.Win32Menu;

[Flags]
public enum MenuItemInformationMask {
    /// <summary>
    /// Retrieves or sets the hbmpItem member.
    /// </summary>
    Bitmap = 0x00000080,

    /// <summary>
    /// Retrieves or sets the hbmpChecked and hbmpUnchecked members.
    /// </summary>
    Checkmarks = 0x00000008,

    /// <summary>
    /// Retrieves or sets the dwItemData member.
    /// </summary>
    Data = 0x00000020,

    /// <summary>
    /// Retrieves or sets the fType member.
    /// </summary>
    FType = 0x00000100,

    /// <summary>
    /// Retrieves or sets the wID member.
    /// </summary>
    Id = 0x00000002,

    /// <summary>
    /// Retrieves or sets the fState member.
    /// </summary>
    State = 0x00000001,

    /// <summary>
    /// Retrieves or sets the dwTypeData member.
    /// </summary>
    String = 0x00000040,

    /// <summary>
    /// Retrieves or sets the hSubMenu member.
    /// </summary>
    Submenu = 0x00000004,

    /// <summary>
    /// Retrieves or sets the fType and dwTypeData members.
    /// </summary>
    /// <remarks> Type is replaced by Bitmap, MenuItemInformationMaskFType, and String.</remarks>
    Type = 0x00000010
}