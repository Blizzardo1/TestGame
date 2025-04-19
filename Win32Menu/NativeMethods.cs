using System.Runtime.InteropServices;
using TestGame.Win32Menu;

namespace TestGame;

internal partial class NativeMethods {
    #if WINDOWS
    private const string User32 = "User32.dll";
    private const string Kernel32 = "Kernel32.dll";
    private const string Gdi32 = "Gdi32.dll";

    [DllImport(Kernel32, EntryPoint="AllocConsole")]
    private static extern void INTERNAL_AllocConsole();

    [DllImport(Kernel32, EntryPoint="FreeConsole")]
    private static extern void INTERNAL_FreeConsole();

    [LibraryImport(Gdi32, EntryPoint="CreateSolidBrush")]
    private static partial nint INTERNAL_CreateSolidBrush(uint color);

    [LibraryImport(User32, EntryPoint="GetMenu")]
    public static partial nint INTERNAL_GetMenu(nint hWnd);
    
    [LibraryImport(User32, EntryPoint="DeleteMenu")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool INTERNAL_DeleteMenu(nint hMenu, uint uPosition, uint uFlags);

    [LibraryImport(User32, EntryPoint="SetMenu")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool INTERNAL_SetMenu(nint hWnd, nint hMenu);

    [LibraryImport(User32, EntryPoint="CreateMenu")]
    public static partial nint INTERNAL_CreateMenu();

    [LibraryImport(User32, EntryPoint="CreatePopupMenu")]
    public static partial nint INTERNAL_CreatePopupMenu();

    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16, EntryPoint="AppendMenu")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool INTERNAL_AppendMenu(nint hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "InsertMenuA")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool INTERNAL_InsertMenu(nint hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem);

    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "InsertMenuItemA")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool INTERNAL_InsertMenuItem(nint hMenu, uint uItem, [MarshalAs(UnmanagedType.Bool)] bool fByPosition,
        ref MenuItemInfo lpmii);

    [DllImport(User32, SetLastError = true, EntryPoint="SetMenuInfo")]
    public static extern bool INTERNAL_SetMenuInfo(IntPtr hMenu, [In] ref MenuInfo lpcmi);
    #endif

    /// <summary>
    /// 
    /// </summary>
    public static void AllocConsole() {
        #if WINDOWS
        INTERNAL_AllocConsole();
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    public static void FreeConsole() {
        #if WINDOWS
        INTERNAL_FreeConsole();
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void CreateBrush(uint color) {
        #if WINDOWS
        INTERNAL_CreateSolidBrush(color);
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static nint GetMenu(nint hWnd){
        #if WINDOWS
        return INTERNAL_GetMenu(hWnd);
        #else
        return 0;
        #endif
    }

/// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uPosition"></param>
    /// <param name="uFlags"></param>
    /// <returns></returns>
    public static bool DeleteMenu(nint hMenu, uint uPosition, uint uFlags) {
        #if WINDOWS
        return INTERNAL_DeleteMenu(hMenu, uPosition, uFlags);
        #else
        return false;
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="hMenu"></param>
    /// <returns></returns>
    public static bool SetMenu(nint hWnd, nint hMenu) {
        #if WINDOWS
        return INTERNAL_SetMenu(hWnd, hMenu);
        #else
        return false;
        #endif
    }

    /// <summary>
    /// Creates a menu. The menu is initially empty, but it can be filled with menu items by using the InsertMenuItem, AppendMenu, and InsertMenu functions.
    /// </summary>
    /// Resources associated with a menu that is assigned to a window are freed automatically. If the menu is not assigned to a window, an application must free system resources associated with the menu before closing. An application frees menu resources by calling the DestroyMenu function.
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the newly created menu.
    /// 
    ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    public static nint CreateMenu() {
        #if WINDOWS
        return INTERNAL_CreateMenu();
        #else
        return nint.Zero;
        #endif
    }



    public static nint CreatePopupMenu() {
        #if WINDOWS
        return INTERNAL_CreatePopupMenu();
        #else
        return nint.Zero;
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uFlags"></param>
    /// <param name="uIDNewItem"></param>
    /// <param name="lpNewItem"></param>
    /// <returns></returns>
    public static bool AppendMenu(nint hMenu, uint uFlags, uint uIDNewItem, string lpNewItem) {
        #if WINDOWS
        return INTERNAL_AppendMenu(hMenu, uFlags, uIDNewItem, lpNewItem);
        #else
        return false;
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uPosition"></param>
    /// <param name="uFlags"></param>
    /// <param name="uIDNewItem"></param>
    /// <param name="lpNewItem"></param>
    /// <returns></returns>
    public static bool InsertMenu(nint hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem) {
        #if WINDOWS
        return INTERNAL_InsertMenu(hMenu, uPosition, uFlags, uIDNewItem, lpNewItem);
        #else
        return false;
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hMenu"></param>
    /// <param name="uItem"></param>
    /// <param name="fByPosition"></param>
    /// <param name="lpmii"></param>
    /// <returns></returns>
    public static bool InsertMenuItem(nint hMenu, uint uItem, bool fByPosition, ref MenuItemInfo lpmii) {
        #if WINDOWS
        return INTERNAL_InsertMenuItem(hMenu, uItem, fByPosition, ref lpmii);
        #else
        return false;
        #endif
    }

    /// <summary>
    /// Sets information about a menu.
    /// </summary>
    /// <param name="hMenu">A handle to the menu for which to set information.</param>
    /// <param name="lpcmi">A pointer to a <see cref="MenuInfo"/> structure.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    public static bool SetMenuInfo(nint hMenu, ref MenuInfo lpcmi) {
        #if WINDOWS
        return INTERNAL_SetMenuInfo(hMenu, ref lpcmi);
        #else
        return false;
        #endif
    }



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