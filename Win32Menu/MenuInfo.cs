using System.Runtime.InteropServices;

namespace TestGame.Win32Menu;

/// <summary>
/// Contains information about a menu.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MenuInfo {
    /// <summary>
    /// The size of the structure, in bytes. Set this member to sizeof(MENUINFO) before calling the GetMenuInfo or SetMenuInfo function.
    /// </summary>
    public uint cbSize;

    /// <summary>
    /// A set of bit flags that specify the information to retrieve or set. This member can be one or more of the following values.
    /// </summary>
    public uint fMask;

    /// <summary>
    /// The menu style. This member can be zero or more of the following values.
    /// </summary>
    public uint dwStyle;

    /// <summary>
    /// The maximum height of the menu in pixels.
    /// </summary>
    public uint cyMax;

    /// <summary>
    /// A handle to the brush that is used to paint the menu's background.
    /// </summary>
    public IntPtr hbrBack;

    /// <summary>
    /// The menu's Help context identifier.
    /// </summary>
    public uint dwContextHelpID;

    /// <summary>
    /// An application-defined value that identifies the menu or menu item.
    /// </summary>
    public IntPtr dwMenuData;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuInfo"/> struct with the specified size.
    /// </summary>
    /// <param name="size">The size of the structure, in bytes.</param>
    public MenuInfo(uint size) {
        cbSize = size;
        fMask = 0;
        dwStyle = 0;
        cyMax = 0;
        hbrBack = IntPtr.Zero;
        dwContextHelpID = 0;
        dwMenuData = IntPtr.Zero;
    }
}