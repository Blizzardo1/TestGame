using System.Runtime.InteropServices;
using TestGame.Win32Menu;

namespace TestGame;

internal partial class NativeMethods {
    private const string User32 = "User32.dll";
    private const string Kernel32 = "Kernel32.dll";
    private const string Gdi32 = "Gdi32.dll";

    [DllImport(Kernel32)]
    public static extern void AllocConsole();

    [DllImport(Kernel32)]
    public static extern void FreeConsole();

    [LibraryImport(Gdi32)]
    public static partial nint CreateSolidBrush(uint color);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    public static partial nint GetMenu(nint hWnd);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="bRevert"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    public static partial nint GetSystemMenu(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uIDEnableItem"></param>
    /// <param name="uEnable"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnableMenuItem(nint hMenu, uint uIDEnableItem, uint uEnable);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DrawMenuBar(nint hWnd);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uPosition"></param>
    /// <param name="uFlags"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RemoveMenu(nint hMenu, uint uPosition, uint uFlags);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uPosition"></param>
    /// <param name="uFlags"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteMenu(nint hMenu, uint uPosition, uint uFlags);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="hMenu"></param>
    /// <returns></returns>
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetMenu(nint hWnd, nint hMenu);

    /// <summary>
    /// Creates a menu. The menu is initially empty, but it can be filled with menu items by using the InsertMenuItem, AppendMenu, and InsertMenu functions.
    /// </summary>
    /// Resources associated with a menu that is assigned to a window are freed automatically. If the menu is not assigned to a window, an application must free system resources associated with the menu before closing. An application frees menu resources by calling the DestroyMenu function.
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the newly created menu.
    /// 
    ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport(User32)]
    public static partial nint CreateMenu();

    /// <summary>
    /// Creates a drop-down menu, submenu, or shortcut menu for the specified menu.
    /// </summary>
    /// <returns></returns>
    [LibraryImport(User32)]
    public static partial nint CreatePopupMenu();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uFlags"></param>
    /// <param name="uIDNewItem"></param>
    /// <param name="lpNewItem"></param>
    /// <returns></returns>
    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AppendMenu(nint hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uPosition"></param>
    /// <param name="uFlags"></param>
    /// <param name="uIDNewItem"></param>
    /// <param name="lpNewItem"></param>
    /// <returns></returns>
    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "InsertMenuA")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool InsertMenu(nint hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uItem"></param>
    /// <param name="fByPosition"></param>
    /// <param name="lpmii"></param>
    /// <returns></returns>
    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "InsertMenuItemA")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InsertMenuItem(nint hMenu, uint uItem, [MarshalAs(UnmanagedType.Bool)] bool fByPosition,
        ref MenuItemInfo lpmii);

    /// <summary>
    /// Sets information about a menu.
    /// </summary>
    /// <param name="hMenu">A handle to the menu for which to set information.</param>
    /// <param name="lpcmi">A pointer to a <see cref="MenuInfo"/> structure.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetMenuInfo(IntPtr hMenu, [In] ref MenuInfo lpcmi);

    [Flags]
    public enum MenuFlags : uint {
        /// <summary>
        /// Indicates that the uPosition parameter gives the identifier of the menu item. The ByCommand flag is the default if neither the ByCommand nor ByPosition flag is specified.
        /// </summary>
        ByCommand = 0x00000000,

        /// <summary>
        /// Indicates that the uPosition parameter gives the zero-based relative position of the new menu item. If uPosition is -1, the new menu item is appended to the end of the menu.
        /// </summary>
        ByPosition = 0x00000400,

        // Values that can be combined with ByCommand or ByPosition:

        /// <summary>
        /// Uses a bitmap as the menu item. The lpNewItem parameter contains a handle to the bitmap.
        /// </summary>
        Bitmap = 0x00000004,

        /// <summary>
        /// Places a check mark next to the menu item. If the application provides check-mark bitmaps (see SetMenuItemBitmaps), this flag displays the check-mark bitmap next to the menu item.
        /// </summary>
        Checked = 0x00000008,

        /// <summary>
        /// Disables the menu item so that it cannot be selected, but does not gray it.
        /// </summary>
        Disabled = 0x00000002,

        /// <summary>
        /// Enables the menu item so that it can be selected and restores it from its grayed state.
        /// </summary>
        Enabled = 0x00000000,

        /// <summary>
        /// Disables the menu item and grays it so it cannot be selected.
        /// </summary>
        Grayed = 0x00000001,

        /// <summary>
        /// Functions the same as the MENUBREAK flag for a menu bar. For a drop-down menu, submenu, or shortcut menu, the new column is separated from the old column by a vertical line.
        /// </summary>
        MenuBarBreak = 0x00000020,

        /// <summary>
        /// Places the item on a new line (for menu bars) or in a new column (for a drop-down menu, submenu, or shortcut menu) without separating columns.
        /// </summary>
        MenuBreak = 0x00000040,

        /// <summary>
        /// Specifies that the item is an owner-drawn item. Before the menu is displayed for the first time, the window that owns the menu receives a WM_MEASUREITEM message to retrieve the width and height of the menu item. The WM_DRAWITEM message is then sent to the window procedure of the owner window whenever the appearance of the menu item must be updated.
        /// </summary>
        OwnerDraw = 0x00000100,

        /// <summary>
        /// Specifies that the menu item opens a drop-down menu or submenu. The uIDNewItem parameter specifies a handle to the drop-down menu or submenu. This flag is used to add a menu name to a menu bar or a menu item that opens a submenu to a drop-down menu, submenu, or shortcut menu.
        /// </summary>
        Popup = 0x00000010,

        /// <summary>
        /// Draws a horizontal dividing line. This flag is used only in a drop-down menu, submenu, or shortcut menu. The line cannot be grayed, disabled, or highlighted. The lpNewItem and uIDNewItem parameters are ignored.
        /// </summary>
        Separator = 0x00000800,

        /// <summary>
        /// Specifies that the menu item is a text string; the lpNewItem parameter is a pointer to the string.
        /// </summary>
        String = 0x00000000,

        /// <summary>
        /// Does not place a check mark next to the menu item (default). If the application supplies check-mark bitmaps (see the SetMenuItemBitmaps function), this flag displays the clear bitmap next to the menu item.
        /// </summary>
        Unchecked = 0x00000000,
    }
}

internal static class MenuFlagsExtensions {
    public static bool HasFlagFast(this NativeMethods.MenuFlags value, NativeMethods.MenuFlags flag) {
        return ( value & flag ) != 0;
    }
}