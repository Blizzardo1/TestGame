namespace TestGame.Win32Menu;

[Flags]
public enum MenuItemState {
    /// <summary>
    /// Checks the menu item. For more information about selected menu items, see the Checked member.
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
    /// Highlights the menu item.
    /// </summary>
    Highlight = 0x00000080,

    /// <summary>
    /// Unchecks the menu item. For more information about clear menu items, see the Checked member.
    /// </summary>
    Unchecked = 0x10000000,
}

public static class MenuItemStateExtensions
{
    public static bool HasFlagFast(this MenuItemState value, MenuItemState flag)
    {
        return (value & flag) != 0;
    }
}