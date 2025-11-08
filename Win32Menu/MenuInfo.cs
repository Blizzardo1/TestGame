using System.Runtime.InteropServices.Marshalling;

namespace TestGame.Win32Menu;


[CustomMarshaller(typeof(MenuInfo), MarshalMode.Default, typeof(MenuInfoMarshaller))]
public static class MenuInfoMarshaller {
    public static unsafe MenuInfo* ConvertToUnmanaged(MenuInfo managed, out IntPtr ptr) {
        ptr = Marshal.AllocHGlobal(sizeof(MenuInfo));
        *(MenuInfo*)ptr = managed;
        return (MenuInfo*)ptr;
    }

    public static unsafe MenuInfo ConvertToManaged(IntPtr unmanaged) {
        return *(MenuInfo*)unmanaged;
    }

    public static void Free(IntPtr unmanaged) {
        Marshal.FreeHGlobal(unmanaged);
    }
}
/// <summary>
/// Contains information about a menu.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MenuInfo"/> struct with the specified size.
/// </remarks>
/// <param name="size">The size of the structure, in bytes.</param>
[NativeMarshalling(typeof(MenuInfoMarshaller))]
[StructLayout(LayoutKind.Sequential)]
public struct MenuInfo(uint size) {
    /// <summary>
    /// The size of the structure, in bytes. Set this member to sizeof(MENUINFO) before calling the GetMenuInfo or SetMenuInfo function.
    /// </summary>
    public uint cbSize = size;

    /// <summary>
    /// A set of bit flags that specify the information to retrieve or set. This member can be one or more of the following values.
    /// </summary>
    public uint fMask = 0;

    /// <summary>
    /// The menu style. This member can be zero or more of the following values.
    /// </summary>
    public uint dwStyle = 0;

    /// <summary>
    /// The maximum height of the menu in pixels.
    /// </summary>
    public uint cyMax = 0;

    /// <summary>
    /// A handle to the brush that is used to paint the menu's background.
    /// </summary>
    public IntPtr hbrBack = IntPtr.Zero;

    /// <summary>
    /// The menu's Help context identifier.
    /// </summary>
    public uint dwContextHelpID = 0;

    /// <summary>
    /// An application-defined value that identifies the menu or menu item.
    /// </summary>
    public IntPtr dwMenuData = IntPtr.Zero;
}