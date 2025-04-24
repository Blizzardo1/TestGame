namespace TestGame.Win32Menu;

public enum HBitmapTable {
    /// <summary>
    /// A bitmap that is drawn by the window that owns the menu. The application must process the WM_MEASUREITEM and WM_DRAWITEM messages. 
    /// </summary>
    Callback = -1,

    /// <summary>
    /// Close button for the menu bar. 
    /// </summary>
    MenuBarClose = 5,

    /// <summary>
    /// Disabled close button for the menu bar. 
    /// </summary>
    MenuBarCloseDisabled = 6,

    /// <summary>
    /// Minimize button for the menu bar. 
    /// </summary>
    MenuBarMinimize = 3,

    /// <summary>
    /// Disabled minimize button for the menu bar.
    /// </summary>
    MenuBarMinimizeDisabled = 7,

    /// <summary>
    /// Restore button for the menu bar. 
    /// </summary>
    MenuBarRestore = 2,

    /// <summary>
    /// Close button for the submenu. 
    /// </summary>
    Popup = 8,

    /// <summary>
    /// Maximize button for the submenu. 
    /// </summary>
    PopupMaximize = 10,

    /// <summary>
    /// Minimize button for the submenu. 
    /// </summary>
    PopupMinimize = 11,

    /// <summary>
    /// Restore button for the submenu. 
    /// </summary>
    PopupRestore = 9,

    /// <summary>
    /// Windows icon or the icon of the window specified in dwItemData. 
    /// </summary>
    System = 1
}