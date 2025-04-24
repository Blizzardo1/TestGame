namespace TestGame.Win32Menu;

[Flags]
public enum MenuItemState {
    /// <summary>
    /// Checks the menu item. For more information about selected menu items, see the hbmpChecked member.
    /// </summary>
    Checked = 0x00000008,

    /// <summary>
    /// Specifies that the menu item is the default. A menu can contain only one default menu item, which is displayed in bold.
    /// </summary>
    Default = 0x00001000,

    /// <summary>
    /// Disables the menu item and grays it so that it cannot be selected. This is equivalent to GRAYED.
    /// </summary>
    Disabled = 0x00000003,

    /// <summary>
    /// Enables the menu item so that it can be selected. This is the default state.
    /// </summary>
    Enabled = 0x00000000,

    /// <summary>
    /// Disables the menu item and grays it so that it cannot be selected. This is equivalent to DISABLED.
    /// </summary>
    Grayed = 0x00000003,

    /// <summary>
    /// Highlights the menu item.
    /// </summary>
    Hilite = 0x00000080,

    /// <summary>
    /// Unchecks the menu item. For more information about clear menu items, see the hbmpChecked member.
    /// </summary>
    Unchecked = 0x00000000,

    /// <summary>
    /// Removes the highlight from the menu item. This is the default state.
    /// </summary>
    Unhilite = 0x00000000,
}