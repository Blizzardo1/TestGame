#region Header

// SDL2 >TestGame >MenuItemInfo.cs\n Copyright (C) , 2023\nCreated 25 11, 2023

#endregion

using System.Runtime.InteropServices.Marshalling;

namespace TestGame.Win32Menu;

[CustomMarshaller(typeof(MenuItemInfo), MarshalMode.ManagedToUnmanagedIn, typeof(MenuItemInfoMarshaller))]
internal unsafe static class MenuItemInfoMarshaller {
    public struct Unmanaged {
        public uint CbSize;

        public uint FMask;

        public uint FType;

        public uint FState;

        public uint WId;

        public nint HSubMenu;

        public nint HbmpChecked;

        public nint HbmpUnchecked;

        public nuint DwItemData;

        public nint DwTypeData; // Changed from string to nint to ensure it is unmanaged

        public uint Cch;

        public nint HbmpItem;
    }

    public static Unmanaged ConvertToUnmanaged(MenuItemInfo managed) {
        var unmanaged = new Unmanaged {
            CbSize = (uint)Marshal.SizeOf<Unmanaged>(), // Corrected to use Unmanaged type
            FMask = managed.FMask,
            FType = managed.FType,
            FState = managed.FState,
            WId = managed.WId,
            HSubMenu = managed.HSubMenu,
            HbmpChecked = managed.HbmpChecked,
            HbmpUnchecked = managed.HbmpUnchecked,
            DwItemData = managed.DwItemData,
            DwTypeData = Marshal.StringToHGlobalUni(managed.DwTypeData), // Convert string to unmanaged memory
            Cch = managed.Cch,
            HbmpItem = managed.HbmpItem
        };

        return unmanaged;
    }

    public static void Free(Unmanaged unmanaged) {
        if (unmanaged.DwTypeData != nint.Zero) {
            Marshal.FreeHGlobal(unmanaged.DwTypeData); // Free unmanaged memory for dwTypeData
        }
    }
}

/// <summary>
/// Contains information about a menu item.
/// </summary>
/// <remarks>
/// The MENUITEMINFO structure is used with the GetMenuItemInfo, InsertMenuItem, and SetMenuItemInfo functions.
/// The menu can display items using text, bitmaps, or both.
/// </remarks>
[NativeMarshalling(typeof(MenuItemInfoMarshaller))]
public struct MenuItemInfo {
    /// <summary>
    /// The size of the structure, in bytes. The caller must set this member to sizeof(MENUITEMINFO).
    /// </summary>
    public uint CbSize;

    /// <summary>
    /// Indicates the members to be retrieved or set. This member can be one or more of the following values. <see cref="MenuItemInformationMask"/>
    /// </summary>
    public uint FMask;

    /// <summary>
    /// The menu item type. This member can be one or more of the following values.
    /// 
    /// The MFT_BITMAP, MFT_SEPARATOR, and MFT_STRING values cannot be combined with one another. Set fMask to MIIM_TYPE to use fType.
    /// 
    /// fType is used only if fMask has a value of <see cref="MenuItemInformationMaskFType"/>.
    /// </summary>
    public uint FType;

    /// <summary>
    /// The menu item state. This member can be one or more of these values. Set fMask to <see cref="MenuItemState"/>to use fState.
    /// </summary>
    public uint FState;

    /// <summary>
    /// An application-defined value that identifies the menu item. Set fMask to MIIM_ID to use wID.
    /// </summary>
    public uint WId;

    /// <summary>
    /// A handle to the drop-down menu or submenu associated with the menu item. If the menu item is not an item that opens a drop-down menu or submenu, this member is NULL. Set fMask to MIIM_SUBMENU to use hSubMenu.
    /// </summary>
    public nint HSubMenu;

    /// <summary>
    /// A handle to the bitmap to display next to the item if it is selected. If this member is NULL, a default bitmap is used. If the MFT_RADIOCHECK type value is specified, the default bitmap is a bullet. Otherwise, it is a check mark. Set fMask to MIIM_CHECKMARKS to use hbmpChecked.
    /// </summary>
    public nint HbmpChecked;

    /// <summary>
    /// A handle to the bitmap to display next to the item if it is not selected. If this member is NULL, no bitmap is used. Set fMask to MIIM_CHECKMARKS to use hbmpUnchecked.
    /// </summary>
    public nint HbmpUnchecked;

    /// <summary>
    /// An application-defined value associated with the menu item. Set fMask to MIIM_DATA to use dwItemData.
    /// </summary>
    public nuint DwItemData;

    /// <summary>
    /// The contents of the menu item. The meaning of this member depends on the value of fType and is used only if the MIIM_TYPE flag is set in the fMask member.
    /// 
    /// To retrieve a menu item of type MFT_STRING, first find the size of the string by setting the dwTypeData member of MENUITEMINFO to NULL and then calling GetMenuItemInfo. The value of cch+1 is the size needed. Then allocate a buffer of this size, place the pointer to the buffer in dwTypeData, increment cch, and call GetMenuItemInfo once again to fill the buffer with the string. If the retrieved menu item is of some other type, then GetMenuItemInfo sets the dwTypeData member to a value whose type is specified by the fType member.
    /// 
    /// When using with the SetMenuItemInfo function, this member should contain a value whose type is specified by the fType member.
    /// 
    /// dwTypeData is used only if the MIIM_STRING flag is set in the fMask member
    /// </summary>
    public string DwTypeData;

    /// <summary>
    /// The length of the menu item text, in characters, when information is received about a menu item of the MFT_STRING type. However, cch is used only if the MIIM_TYPE flag is set in the fMask member and is zero otherwise. Also, cch is ignored when the content of a menu item is set by calling SetMenuItemInfo.
    /// 
    /// Note that, before calling GetMenuItemInfo, the application must set cch to the length of the buffer pointed to by the dwTypeData member. If the retrieved menu item is of type MFT_STRING (as indicated by the fType member), then GetMenuItemInfo changes cch to the length of the menu item text. If the retrieved menu item is of some other type, GetMenuItemInfo sets the cch field to zero.
    /// 
    /// The cch member is used when the MIIM_STRING flag is set in the fMask member.
    /// </summary>
    public uint Cch;

    /// <summary>
    /// A handle to the bitmap to be displayed, or it can be one of the values in the following table. It is used when the MIIM_BITMAP flag is set in the fMask member.
    /// </summary>
    public nint HbmpItem;
}