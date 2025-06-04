using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using TestGame.GameObjects;

namespace TestGame;

public abstract class Window : Renderer, IRenderer, IDisposable {
    private static readonly Log _log = Log.GetCurrentClassLogger(LogCategory.Video);
    private bool disposedValue;
    #region Events

    public event EventHandler<AudioDeviceEvent>? AudioDeviceAdded;

    public event EventHandler<AudioDeviceEvent>? AudioDeviceFormatChanged;

    public event EventHandler<AudioDeviceEvent>? AudioDeviceRemoved;

    public event EventHandler<CameraDeviceEvent>? CameraDeviceAdded;

    public event EventHandler<CameraDeviceEvent>? CameraDeviceApproved;

    public event EventHandler<CameraDeviceEvent>? CameraDeviceDenied;

    public event EventHandler<CameraDeviceEvent>? CameraDeviceRemoved;

    public event EventHandler<ClipboardEvent>? ClipboardUpdate;

    public event EventHandler<CommonEvent>? DidEnterBackground;

    public event EventHandler<CommonEvent>? DidEnterForeground;

    public event EventHandler<DisplayEvent>? DisplayAdded;

    public event EventHandler<DisplayEvent>? DisplayContentScaleChanged;

    public event EventHandler<DisplayEvent>? DisplayCurrentModeChanged;

    public event EventHandler<DisplayEvent>? DisplayDesktopModeChanged;

    public event EventHandler<DisplayEvent>? DisplayMoved;

    public event EventHandler<DisplayEvent>? DisplayOrientation;

    public event EventHandler<DisplayEvent>? DisplayRemoved;

    public event EventHandler<DropEvent>? DropBegin;

    public event EventHandler<DropEvent>? DropComplete;

    public event EventHandler<DropEvent>? DropFile;

    public event EventHandler<DropEvent>? DropPosition;

    public event EventHandler<DropEvent>? DropText;

    public event EventHandler<TouchFingerEvent>? FingerCanceled;

    public event EventHandler<TouchFingerEvent>? FingerDown;

    public event EventHandler<TouchFingerEvent>? FingerMotion;

    public event EventHandler<TouchFingerEvent>? FingerUp;

    public event EventHandler<CommonEvent>? First;

    public event EventHandler<GamepadDeviceEvent>? GamepadAdded;

    public event EventHandler<GamepadAxisEvent>? GamepadAxisMotion;

    public event EventHandler<GamepadButtonEvent>? GamepadButtonDown;

    public event EventHandler<GamepadButtonEvent>? GamepadButtonUp;

    public event EventHandler<CommonEvent>? GamepadRemapped;

    public event EventHandler<GamepadDeviceEvent>? GamepadRemoved;

    public event EventHandler<GamepadSensorEvent>? GamepadSensorUpdate;

    public event EventHandler<CommonEvent>? GamepadSteamHandleUpdated;

    public event EventHandler<GamepadTouchpadEvent>? GamepadTouchpadDown;

    public event EventHandler<GamepadTouchpadEvent>? GamepadTouchpadMotion;

    public event EventHandler<GamepadTouchpadEvent>? GamepadTouchpadUp;

    public event EventHandler<CommonEvent>? GamepadUpdateComplete;

    public event EventHandler<JoyDeviceEvent>? JoystickAdded;

    public event EventHandler<JoyAxisEvent>? JoystickAxisMotion;

    public event EventHandler<JoyBallEvent>? JoystickBallMotion;

    public event EventHandler<JoyBatteryEvent>? JoystickBatteryUpdated;

    public event EventHandler<JoyButtonEvent>? JoystickButtonDown;

    public event EventHandler<JoyButtonEvent>? JoystickButtonUp;

    public event EventHandler<JoyHatEvent>? JoystickHatMotion;

    public event EventHandler<JoyDeviceEvent>? JoystickRemoved;

    public event EventHandler<CommonEvent>? JoystickUpdateComplete;

    public event EventHandler<KeyboardDeviceEvent>? KeyboardAdded;

    public event EventHandler<KeyboardDeviceEvent>? KeyboardRemoved;

    public event EventHandler<KeyboardEvent>? KeyDown;

    public event EventHandler<CommonEvent>? KeymapChanged;

    public event EventHandler<KeyboardEvent>? KeyUp;

    public event EventHandler<CommonEvent>? Last;

    public event EventHandler<CommonEvent>? LocaleChanged;

    public event EventHandler<CommonEvent>? LowMemory;

    public event EventHandler<MouseDeviceEvent>? MouseAdded;

    public event EventHandler<MouseButtonEvent>? MouseButtonDown;

    public event EventHandler<MouseButtonEvent>? MouseButtonUp;

    public event EventHandler<MouseMotionEvent>? MouseMotion;

    public event EventHandler<MouseDeviceEvent>? MouseRemoved;

    public event EventHandler<MouseWheelEvent>? MouseWheel;

    public event EventHandler<PenAxisEvent>? PenAxis;

    public event EventHandler<PenButtonEvent>? PenButtonDown;

    public event EventHandler<PenButtonEvent>? PenButtonUp;

    public event EventHandler<PenTouchEvent>? PenDown;

    public event EventHandler<PenMotionEvent>? PenMotion;

    public event EventHandler<PenProximityEvent>? PenProximityIn;

    public event EventHandler<PenProximityEvent>? PenProximityOut;

    public event EventHandler<PenTouchEvent>? PenUp;

    public event EventHandler<CommonEvent>? PollSentinel;

    public event EventHandler<UserEvent>? Private0;

    public event EventHandler<UserEvent>? Private1;

    public event EventHandler<UserEvent>? Private2;

    public event EventHandler<UserEvent>? Private3;

    public event EventHandler<QuitEvent>? Quit;
    public event EventHandler<RenderEvent>? RenderDeviceLost;

    public event EventHandler<RenderEvent>? RenderDeviceReset;

    public event EventHandler<RenderEvent>? RenderTargetsReset;

    public event EventHandler<SensorEvent>? SensorUpdate;

    public event EventHandler<CommonEvent>? SystemThemeChanged;

    public event EventHandler<CommonEvent>? Terminating;
    public event EventHandler<TextEditingEvent>? TextEditing;

    public event EventHandler<TextEditingCandidatesEvent>? TextEditingCandidates;

    public event EventHandler<TextInputEvent>? TextInput;

    public event EventHandler<UserEvent>? User;

    public event EventHandler<CommonEvent>? WillEnterBackground;
    public event EventHandler<CommonEvent>? WillEnterForeground;
    public event EventHandler<WindowEvent>? WindowCloseRequested;

    public event EventHandler<WindowEvent>? WindowDestroyed;

    public event EventHandler<WindowEvent>? WindowDisplayChanged;

    public event EventHandler<WindowEvent>? WindowDisplayScaleChanged;

    public event EventHandler<WindowEvent>? WindowEnterFullscreen;

    public event EventHandler<WindowEvent>? WindowExposed;
    public event EventHandler<WindowEvent>? WindowFirst;
    public event EventHandler<WindowEvent>? WindowFocusGained;

    public event EventHandler<WindowEvent>? WindowFocusLost;

    public event EventHandler<WindowEvent>? WindowHdrStateChanged;

    public event EventHandler<WindowEvent>? WindowHidden;

    public event EventHandler<WindowEvent>? WindowHitTest;

    public event EventHandler<WindowEvent>? WindowIccProfChanged;

    public event EventHandler<WindowEvent>? WindowLeaveFullscreen;

    public event EventHandler<WindowEvent>? WindowMaximized;

    public event EventHandler<WindowEvent>? WindowMetalViewResized;

    public event EventHandler<WindowEvent>? WindowMinimized;

    public event EventHandler<WindowEvent>? WindowMouseEnter;

    public event EventHandler<WindowEvent>? WindowMouseLeave;

    public event EventHandler<WindowEvent>? WindowMoved;

    public event EventHandler<WindowEvent>? WindowOccluded;

    public event EventHandler<WindowEvent>? WindowPixelSizeChanged;

    public event EventHandler<WindowEvent>? WindowResized;

    public event EventHandler<WindowEvent>? WindowRestored;

    public event EventHandler<WindowEvent>? WindowSafeAreaChanged;

    public event EventHandler<WindowEvent>? WindowShown;
    #endregion

    protected Window(string title, Point location, Size size, WindowFlags flags) {
        X = location.X;
        Y = location.Y;

        Width = size.Width;
        Height = size.Height;

        WindowPtr = Sdl.CreateWindow(title,
            (int)Width,
            (int)Height,
            flags);

        if (WindowPtr == nint.Zero) {
            _log.Error($"Cannot create Window: {Sdl.GetError()}");
            return;
        }

        _log.Info($"Window Created: 0x{WindowPtr:X8}");
        _log.Info($"Window ID: {Sdl.GetWindowId(WindowPtr):X8}");
        _log.Info($"Window Title: {title}");

        Initialize(Sdl.CreateRenderer(WindowPtr, null));

        if (RendererPtr == nint.Zero) {
            _log.Error($"Cannot create RendererPtr: {Sdl.GetError()}");
        }
    }

    ~Window() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public Event CurrentEvent { get; protected set; }
    /// <inheritdoc />
    public float Height { get; protected set; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public float Width { get; protected set; }

    /// <inheritdoc />
    public float X { get; protected set; }

    /// <inheritdoc />
    public float Y { get; protected set; }

    /// <inheritdoc />
    public float Z { get; } = float.MaxValue;

    protected nint WindowPtr { get; }

    /// <summary>
    /// A Pointer to the Window
    /// </summary>
    /// <returns>A <see cref="nint"/> Pointer to the Window</returns>
    public nint GetWindow() => WindowPtr;

    /// <inheritdoc />
    public abstract void Draw();
    public void OnAudioDeviceAdded(AudioDeviceEvent @event) {
        AudioDeviceAdded?.Invoke(this, @event);
    }

    public void OnAudioDeviceFormatChanged(AudioDeviceEvent @event) {
        AudioDeviceFormatChanged?.Invoke(this, @event);
    }

    public void OnAudioDeviceRemoved(AudioDeviceEvent @event) {
        AudioDeviceRemoved?.Invoke(this, @event);
    }

    public void OnCameraDeviceAdded(CameraDeviceEvent @event) {
        CameraDeviceAdded?.Invoke(this, @event);
    }

    public void OnCameraDeviceApproved(CameraDeviceEvent @event) {
        CameraDeviceApproved?.Invoke(this, @event);
    }

    public void OnCameraDeviceDenied(CameraDeviceEvent @event) {
        CameraDeviceDenied?.Invoke(this, @event);
    }

    public void OnCameraDeviceRemoved(CameraDeviceEvent @event) {
        CameraDeviceRemoved?.Invoke(this, @event);
    }

    public void OnClipboardUpdate(ClipboardEvent @event) {
        ClipboardUpdate?.Invoke(this, @event);
    }

    public void OnDidEnterBackground(CommonEvent @event) {
        DidEnterBackground?.Invoke(this, @event);
    }

    public void OnDidEnterForeground(CommonEvent @event) {
        DidEnterForeground?.Invoke(this, @event);
    }

    public void OnDisplayAdded(DisplayEvent @event) {
        DisplayAdded?.Invoke(this, @event);
    }

    public void OnDisplayContentScaleChanged(DisplayEvent @event) {
        DisplayContentScaleChanged?.Invoke(this, @event);
    }

    public void OnDisplayCurrentModeChanged(DisplayEvent @event) {
        DisplayCurrentModeChanged?.Invoke(this, @event);
    }

    public void OnDisplayDesktopModeChanged(DisplayEvent @event) {
        DisplayDesktopModeChanged?.Invoke(this, @event);
    }

    public void OnDisplayMoved(DisplayEvent @event) {
        DisplayMoved?.Invoke(this, @event);
    }

    public void OnDisplayOrientation(DisplayEvent @event) {
        DisplayOrientation?.Invoke(this, @event);
    }

    public void OnDisplayRemoved(DisplayEvent @event) {
        DisplayRemoved?.Invoke(this, @event);
    }

    public void OnDropBegin(DropEvent @event) {
        DropBegin?.Invoke(this, @event);
    }

    public void OnDropComplete(DropEvent @event) {
        DropComplete?.Invoke(this, @event);
    }

    public void OnDropFile(DropEvent @event) {
        DropFile?.Invoke(this, @event);
    }

    public void OnDropPosition(DropEvent @event) {
        DropPosition?.Invoke(this, @event);
    }

    public void OnDropText(DropEvent @event) {
        DropText?.Invoke(this, @event);
    }

    public void OnFingerCanceled(TouchFingerEvent @event) {
        FingerCanceled?.Invoke(this, @event);
    }

    public void OnFingerDown(TouchFingerEvent @event) {
        FingerDown?.Invoke(this, @event);
    }

    public void OnFingerMotion(TouchFingerEvent @event) {
        FingerMotion?.Invoke(this, @event);
    }

    public void OnFingerUp(TouchFingerEvent @event) {
        FingerUp?.Invoke(this, @event);
    }

    public void OnFirst(CommonEvent @event) {
        First?.Invoke(this, @event);
    }

    public void OnGamepadAdded(GamepadDeviceEvent @event) {
        GamepadAdded?.Invoke(this, @event);
    }

    public void OnGamepadAxisMotion(GamepadAxisEvent @event) {
        GamepadAxisMotion?.Invoke(this, @event);
    }

    public void OnGamepadButtonDown(GamepadButtonEvent @event) {
        GamepadButtonDown?.Invoke(this, @event);
    }

    public void OnGamepadButtonUp(GamepadButtonEvent @event) {
        GamepadButtonUp?.Invoke(this, @event);
    }

    public void OnGamepadRemapped(CommonEvent @event) {
        GamepadRemapped?.Invoke(this, @event);
    }

    public void OnGamepadRemoved(GamepadDeviceEvent @event) {
        GamepadRemoved?.Invoke(this, @event);
    }

    public void OnGamepadSensorUpdate(GamepadSensorEvent @event) {
        GamepadSensorUpdate?.Invoke(this, @event);
    }

    public void OnGamepadSteamHandleUpdated(CommonEvent @event) {
        GamepadSteamHandleUpdated?.Invoke(this, @event);
    }

    public void OnGamepadTouchpadDown(GamepadTouchpadEvent @event) {
        GamepadTouchpadDown?.Invoke(this, @event);
    }

    public void OnGamepadTouchpadMotion(GamepadTouchpadEvent @event) {
        GamepadTouchpadMotion?.Invoke(this, @event);
    }

    public void OnGamepadTouchpadUp(GamepadTouchpadEvent @event) {
        GamepadTouchpadUp?.Invoke(this, @event);
    }

    public void OnGamepadUpdateComplete(CommonEvent @event) {
        GamepadUpdateComplete?.Invoke(this, @event);
    }

    public void OnJoystickAdded(JoyDeviceEvent @event) {
        JoystickAdded?.Invoke(this, @event);
    }

    public void OnJoystickAxisMotion(JoyAxisEvent @event) {
        JoystickAxisMotion?.Invoke(this, @event);
    }

    public void OnJoystickBallMotion(JoyBallEvent @event) {
        JoystickBallMotion?.Invoke(this, @event);
    }

    public void OnJoystickBatteryUpdated(JoyBatteryEvent @event) {
        JoystickBatteryUpdated?.Invoke(this, @event);
    }

    public void OnJoystickButtonDown(JoyButtonEvent @event) {
        JoystickButtonDown?.Invoke(this, @event);
    }

    public void OnJoystickButtonUp(JoyButtonEvent @event) {
        JoystickButtonUp?.Invoke(this, @event);
    }

    public void OnJoystickHatMotion(JoyHatEvent @event) {
        JoystickHatMotion?.Invoke(this, @event);
    }

    public void OnJoystickRemoved(JoyDeviceEvent @event) {
        JoystickRemoved?.Invoke(this, @event);
    }

    public void OnJoystickUpdateComplete(CommonEvent @event) {
        JoystickUpdateComplete?.Invoke(this, @event);
    }

    public void OnKeyboardAdded(KeyboardDeviceEvent @event) {
        KeyboardAdded?.Invoke(this, @event);
    }

    public void OnKeyboardRemoved(KeyboardDeviceEvent @event) {
        KeyboardRemoved?.Invoke(this, @event);
    }

    public void OnKeyDown(KeyboardEvent @event) {
        KeyDown?.Invoke(this, @event);
    }

    public void OnKeymapChanged(CommonEvent @event) {
        KeymapChanged?.Invoke(this, @event);
    }

    public void OnKeyUp(KeyboardEvent @event) {
        KeyUp?.Invoke(this, @event);
    }

    public void OnLast(CommonEvent @event) {
        Last?.Invoke(this, @event);
    }

    public void OnLocaleChanged(CommonEvent @event) {
        LocaleChanged?.Invoke(this, @event);
    }

    public void OnLowMemory(CommonEvent @event) {
        LowMemory?.Invoke(this, @event);
    }

    public void OnMouseAdded(MouseDeviceEvent @event) {
        MouseAdded?.Invoke(this, @event);
    }

    public void OnMouseButtonDown(MouseButtonEvent @event) {
        MouseButtonDown?.Invoke(this, @event);
    }

    public void OnMouseButtonUp(MouseButtonEvent @event) {
        MouseButtonUp?.Invoke(this, @event);
    }

    public void OnMouseMotion(MouseMotionEvent @event) {
        MouseMotion?.Invoke(this, @event);
    }

    public void OnMouseRemoved(MouseDeviceEvent @event) {
        MouseRemoved?.Invoke(this, @event);
    }

    public void OnMouseWheel(MouseWheelEvent @event) {
        MouseWheel?.Invoke(this, @event);
    }

    public void OnPenAxis(PenAxisEvent @event) {
        PenAxis?.Invoke(this, @event);
    }

    public void OnPenButtonDown(PenButtonEvent @event) {
        PenButtonDown?.Invoke(this, @event);
    }

    public void OnPenButtonUp(PenButtonEvent @event) {
        PenButtonUp?.Invoke(this, @event);
    }

    public void OnPenDown(PenTouchEvent @event) {
        PenDown?.Invoke(this, @event);
    }

    public void OnPenMotion(PenMotionEvent @event) {
        PenMotion?.Invoke(this, @event);
    }

    public void OnPenProximityIn(PenProximityEvent @event) {
        PenProximityIn?.Invoke(this, @event);
    }

    public void OnPenProximityOut(PenProximityEvent @event) {
        PenProximityOut?.Invoke(this, @event);
    }

    public void OnPenUp(PenTouchEvent @event) {
        PenUp?.Invoke(this, @event);
    }

    public void OnPollSentinel(CommonEvent @event) {
        PollSentinel?.Invoke(this, @event);
    }

    public void OnPrivate0(UserEvent @event) {
        Private0?.Invoke(this, @event);
    }

    public void OnPrivate1(UserEvent @event) {
        Private1?.Invoke(this, @event);
    }

    public void OnPrivate2(UserEvent @event) {
        Private2?.Invoke(this, @event);
    }

    public void OnPrivate3(UserEvent @event) {
        Private3?.Invoke(this, @event);
    }

    public void OnQuit(QuitEvent @event) {
        Quit?.Invoke(this, @event);
    }

    public void OnRenderDeviceLost(RenderEvent @event) {
        RenderDeviceLost?.Invoke(this, @event);
    }

    public void OnRenderDeviceReset(RenderEvent @event) {
        RenderDeviceReset?.Invoke(this, @event);
    }

    public void OnRenderTargetsReset(RenderEvent @event) {
        RenderTargetsReset?.Invoke(this, @event);
    }

    public void OnSensorUpdate(SensorEvent @event) {
        SensorUpdate?.Invoke(this, @event);
    }

    public void OnSystemThemeChanged(CommonEvent @event) {
        SystemThemeChanged?.Invoke(this, @event);
    }

    public void OnTerminating(CommonEvent @event) {
        Terminating?.Invoke(this, @event);
    }

    public void OnTextEditing(TextEditingEvent @event) {
        TextEditing?.Invoke(this, @event);
    }

    public void OnTextEditingCandidates(TextEditingCandidatesEvent @event) {
        TextEditingCandidates?.Invoke(this, @event);
    }

    public void OnTextInput(TextInputEvent @event) {
        TextInput?.Invoke(this, @event);
    }

    public void OnUser(UserEvent @event) {
        User?.Invoke(this, @event);
    }

    public void OnWillEnterBackground(CommonEvent @event) {
        WillEnterBackground?.Invoke(this, @event);
    }

    public void OnWillEnterForeground(CommonEvent @event) {
        WillEnterForeground?.Invoke(this, @event);
    }

    public void OnWindowCloseRequested(WindowEvent @event) {
        WindowCloseRequested?.Invoke(this, @event);
    }

    public void OnWindowDestroyed(WindowEvent @event) {
        WindowDestroyed?.Invoke(this, @event);
    }

    public void OnWindowDisplayChanged(WindowEvent @event) {
        WindowDisplayChanged?.Invoke(this, @event);
    }

    public void OnWindowDisplayScaleChanged(WindowEvent @event) {
        WindowDisplayScaleChanged?.Invoke(this, @event);
    }

    public void OnWindowEnterFullscreen(WindowEvent @event) {
        WindowEnterFullscreen?.Invoke(this, @event);
    }

    public void OnWindowExposed(WindowEvent @event) {
        WindowExposed?.Invoke(this, @event);
    }

    public void OnWindowFirst(WindowEvent @event) {
        WindowFirst?.Invoke(this, @event);
    }
    public void OnWindowFocusGained(WindowEvent @event) {
        WindowFocusGained?.Invoke(this, @event);
    }

    public void OnWindowFocusLost(WindowEvent @event) {
        WindowFocusLost?.Invoke(this, @event);
    }

    public void OnWindowHdrStateChanged(WindowEvent @event) {
        WindowHdrStateChanged?.Invoke(this, @event);
    }

    public void OnWindowHidden(WindowEvent @event) {
        WindowHidden?.Invoke(this, @event);
    }

    public void OnWindowHitTest(WindowEvent @event) {
        WindowHitTest?.Invoke(this, @event);
    }

    public void OnWindowIccProfChanged(WindowEvent @event) {
        WindowIccProfChanged?.Invoke(this, @event);
    }

    public void OnWindowLeaveFullscreen(WindowEvent @event) {
        WindowLeaveFullscreen?.Invoke(this, @event);
    }

    public void OnWindowMaximized(WindowEvent @event) {
        WindowMaximized?.Invoke(this, @event);
    }

    public void OnWindowMetalViewResized(WindowEvent @event) {
        WindowMetalViewResized?.Invoke(this, @event);
    }

    public void OnWindowMinimized(WindowEvent @event) {
        WindowMinimized?.Invoke(this, @event);
    }

    public void OnWindowMouseEnter(WindowEvent @event) {
        WindowMouseEnter?.Invoke(this, @event);
    }

    public void OnWindowMouseLeave(WindowEvent @event) {
        WindowMouseLeave?.Invoke(this, @event);
    }

    public void OnWindowMoved(WindowEvent @event) {
        WindowMoved?.Invoke(this, @event);
    }

    public void OnWindowOccluded(WindowEvent @event) {
        WindowOccluded?.Invoke(this, @event);
    }

    public void OnWindowPixelSizeChanged(WindowEvent @event) {
        WindowPixelSizeChanged?.Invoke(this, @event);
    }

    public void OnWindowResized(WindowEvent @event) {
        WindowResized?.Invoke(this, @event);
    }

    public void OnWindowRestored(WindowEvent @event) {
        WindowRestored?.Invoke(this, @event);
    }

    public void OnWindowSafeAreaChanged(WindowEvent @event) {
        WindowSafeAreaChanged?.Invoke(this, @event);
    }

    public void OnWindowShown(WindowEvent @event) {
        WindowShown?.Invoke(this, @event);
    }

    /// <inheritdoc />
    public virtual void Update(Event e) {
        CurrentEvent = e;
        HandleEvents(e);
    }

    public void UpdatePosition(nint window) {
        Sdl.GetWindowPosition(window, out int x, out int y);
        X = x;
        Y = y;
    }

    public void UpdateSize(nint window) {
        Sdl.GetWindowSize(window, out int w, out int h);
        Width = w;
        Height = h;
    }

    #region Event Handler

    private void HandleEvents(Event e) {
        switch (e) {
            case { Type: EventType.First }:
                OnFirst(e.Common);
                break;
            case { Type: EventType.Quit }:
                OnQuit(e.Quit);
                break;
            case { Type: EventType.Terminating }:
                OnTerminating(e.Common);
                break;
            case { Type: EventType.LowMemory }:
                OnLowMemory(e.Common);
                break;
            case { Type: EventType.WillEnterBackground }:
                OnWillEnterBackground(e.Common);
                break;
            case { Type: EventType.DidEnterBackground }:
                OnDidEnterBackground(e.Common);
                break;
            case { Type: EventType.WillEnterForeground }:
                OnWillEnterForeground(e.Common);
                break;
            case { Type: EventType.DidEnterForeground }:
                OnDidEnterForeground(e.Common);
                break;
            case { Type: EventType.DisplayAdded }:
                OnDisplayAdded(e.Display);
                break;
            case { Type: EventType.WindowFirst }:
                OnWindowFirst(e.Window);
                break;
            case { Type: EventType.KeyDown }:
                OnKeyDown(e.Key);
                break;
            case { Type: EventType.KeyUp }:
                OnKeyUp(e.Key);
                break;
            case { Type: EventType.TextEditing }:
                OnTextEditing(e.Edit);
                break;
            case { Type: EventType.TextInput }:
                OnTextInput(e.Text);
                break;
            case { Type: EventType.KeymapChanged }:
                OnKeymapChanged(e.Common);
                break;
            case { Type: EventType.MouseMotion }:
                OnMouseMotion(e.Motion);
                break;
            case { Type: EventType.MouseButtonDown }:
                OnMouseButtonDown(e.Button);
                break;
            case { Type: EventType.MouseButtonUp }:
                OnMouseButtonUp(e.Button);
                break;
            case { Type: EventType.MouseWheel }:
                OnMouseWheel(e.Wheel);
                break;
            case { Type: EventType.JoystickAxisMotion }:
                OnJoystickAxisMotion(e.JAxis);
                break;
            case { Type: EventType.JoystickBallMotion }:
                OnJoystickBallMotion(e.JBall);
                break;
            case { Type: EventType.JoystickHatMotion }:
                OnJoystickHatMotion(e.JHat);
                break;
            case { Type: EventType.JoystickButtonDown }:
                OnJoystickButtonDown(e.JButton);
                break;
            case { Type: EventType.JoystickButtonUp }:
                OnJoystickButtonUp(e.JButton);
                break;
            case { Type: EventType.JoystickAdded }:
                OnJoystickAdded(e.JDevice);
                break;
            case { Type: EventType.JoystickRemoved }:
                OnJoystickRemoved(e.JDevice);
                break;
            case { Type: EventType.JoystickBatteryUpdated }:
                OnJoystickBatteryUpdated(e.JBattery);
                break;
            case { Type: EventType.GamepadAxisMotion }:
                OnGamepadAxisMotion(e.GAxis);
                break;
            case { Type: EventType.GamepadButtonDown }:
                OnGamepadButtonDown(e.GButton);
                break;
            case { Type: EventType.GamepadButtonUp }:
                OnGamepadButtonUp(e.GButton);
                break;
            case { Type: EventType.GamepadAdded }:
                OnGamepadAdded(e.GDevice);
                break;
            case { Type: EventType.GamepadRemoved }:
                OnGamepadRemoved(e.GDevice);
                break;
            case { Type: EventType.GamepadRemapped }:
                OnGamepadRemapped(e.Common);
                break;
            case { Type: EventType.FingerDown }:
                OnFingerDown(e.TFinger);
                break;
            case { Type: EventType.FingerUp }:
                OnFingerUp(e.TFinger);
                break;
            case { Type: EventType.FingerMotion }:
                OnFingerMotion(e.TFinger);
                break;
            case { Type: EventType.ClipboardUpdate }:
                OnClipboardUpdate(e.Clipboard);
                break;
            case { Type: EventType.DropFile }:
                OnDropFile(e.Drop);
                break;
            case { Type: EventType.DropText }:
                OnDropText(e.Drop);
                break;
            case { Type: EventType.DropBegin }:
                OnDropBegin(e.Drop);
                break;
            case { Type: EventType.DropComplete }:
                OnDropComplete(e.Drop);
                break;
            case { Type: EventType.AudioDeviceAdded }:
                OnAudioDeviceAdded(e.ADevice);
                break;
            case { Type: EventType.AudioDeviceRemoved }:
                OnAudioDeviceRemoved(e.ADevice);
                break;
            case { Type: EventType.SensorUpdate }:
                OnSensorUpdate(e.Sensor);
                break;
            case { Type: EventType.RenderTargetsReset }:
                OnRenderTargetsReset(e.Render);
                break;
            case { Type: EventType.RenderDeviceReset }:
                OnRenderDeviceReset(e.Render);
                break;
            case { Type: EventType.User }:
                OnUser(e.User);
                break;
            case { Type: EventType.Last }:
                OnLast(e.Common);
                break;
            case { Type: EventType.PollSentinel}: // SDL_POLLSENTINEL
                OnLast(e.Common);
                break;
            case { Type: EventType.AudioDeviceFormatChanged }:
                    OnAudioDeviceFormatChanged(e.ADevice);
                break;
            case { Type: EventType.CameraDeviceAdded }:
                    OnCameraDeviceAdded(e.CDevice);
                break;
            case { Type: EventType.CameraDeviceApproved }: 
                    OnCameraDeviceApproved(e.CDevice);
                break;
            case { Type: EventType.CameraDeviceDenied }:
                OnCameraDeviceDenied(e.CDevice);
                break;
            case { Type: EventType.CameraDeviceRemoved }:
                    OnCameraDeviceRemoved(e.CDevice);
                break;
            case { Type: EventType.DisplayContentScaleChanged }:
                    OnDisplayContentScaleChanged(e.Display);
                break;
            case { Type: EventType.DisplayCurrentModeChanged }:
                    OnDisplayCurrentModeChanged(e.Display);
                break;
            case { Type: EventType.DisplayDesktopModeChanged }:
                    OnDisplayDesktopModeChanged(e.Display);
                break;
            case { Type: EventType.DisplayMoved }:
                    OnDisplayMoved(e.Display);
                break;
            case { Type: EventType.DisplayOrientation }:
                OnDisplayOrientation(e.Display);
                break;
            case { Type: EventType.DisplayRemoved }:
                OnDisplayRemoved(e.Display);
                break;
            case { Type: EventType.DropPosition }:
                OnDropPosition(e.Drop);
                break;
            case { Type: EventType.FingerCanceled }:
                OnFingerCanceled(e.TFinger);
                break;
            case { Type: EventType.GamepadSensorUpdate }:
                OnGamepadSensorUpdate(e.GSensor);
                break;
            case { Type: EventType.GamepadSteamHandleUpdated }:
                OnGamepadSteamHandleUpdated(e.Common);
                break;
            case { Type: EventType.GamepadTouchpadDown }:
                OnGamepadTouchpadDown(e.GTouchpad);
                break;
            case { Type: EventType.GamepadTouchpadMotion }:
                OnGamepadTouchpadMotion(e.GTouchpad);
                break;
            case { Type: EventType.GamepadTouchpadUp }:
                OnGamepadTouchpadUp(e.GTouchpad);
                break;
            case { Type: EventType.GamepadUpdateComplete }:
                OnGamepadUpdateComplete(e.Common);
                break;
            case { Type: EventType.JoystickUpdateComplete }:
                OnJoystickUpdateComplete(e.Common);
                break;
            case { Type: EventType.KeyboardAdded }:
                OnKeyboardAdded(e.KDevice);
                break;
            case { Type: EventType.KeyboardRemoved }:
                OnKeyboardRemoved(e.KDevice);
                break;
            case { Type: EventType.LocaleChanged }:
                OnLocaleChanged(e.Common);
                break;
            case { Type: EventType.MouseAdded }:
                OnMouseAdded(e.MDevice);
                break;
            case { Type: EventType.MouseRemoved }:
                OnMouseRemoved(e.MDevice);
                break;
            case { Type: EventType.PenAxis }:
                OnPenAxis(e.PAxis);
                break;
            case { Type: EventType.PenButtonDown }:
                OnPenButtonDown(e.PButton);
                break;
            case { Type: EventType.PenButtonUp }:
                OnPenButtonUp(e.PButton);
                break;
            case { Type: EventType.PenDown }:
                OnPenDown(e.PTouch);
                break;
            case { Type: EventType.PenMotion }:
                OnPenMotion(e.PMotion);
                break;
            case { Type: EventType.PenProximityIn }:
                OnPenProximityIn(e.PProximity);
                break;
            case { Type: EventType.PenProximityOut }:
                OnPenProximityOut(e.PProximity);
                break;
            case { Type: EventType.PenUp }:
                OnPenUp(e.PTouch);
                break;
            case { Type: EventType.Private0 }:
                OnPrivate0(e.User);
                break;
            case { Type: EventType.Private1 }:
                OnPrivate1(e.User);
                break;
            case { Type: EventType.Private2 }:
                OnPrivate2(e.User);
                break;
            case { Type: EventType.Private3 }:
                OnPrivate3(e.User);
                break;
            case { Type: EventType.RenderDeviceLost }:
                OnRenderDeviceLost(e.Render);
                break;
            case { Type: EventType.SystemThemeChanged }:
                OnSystemThemeChanged(e.Common);
                break;
            case { Type: EventType.TextEditingCandidates }:
                OnTextEditingCandidates(e.EditCandidates);
                break;
            case { Type: EventType.WindowCloseRequested }:
                OnWindowCloseRequested(e.Window);
                break;
            case { Type: EventType.WindowDestroyed }:
                OnWindowDestroyed(e.Window);
                break;
            case { Type: EventType.WindowDisplayChanged }:
                OnWindowDisplayChanged(e.Window);
                break;
            case { Type: EventType.WindowDisplayScaleChanged }:
                OnWindowDisplayScaleChanged(e.Window);
                break;
            case { Type: EventType.WindowEnterFullscreen }:
                OnWindowEnterFullscreen(e.Window);
                break;
            case { Type: EventType.WindowExposed }:
                OnWindowExposed(e.Window);
                break;
            case { Type: EventType.WindowFocusGained }:
                OnWindowFocusGained(e.Window);
                break;
            case { Type: EventType.WindowFocusLost }:
                OnWindowFocusLost(e.Window);
                break;
            case { Type: EventType.WindowHdrStateChanged }:
                OnWindowHdrStateChanged(e.Window);
                break;
            case { Type: EventType.WindowHidden }:
                OnWindowHidden(e.Window);
                break;
            case { Type: EventType.WindowHitTest }:
                OnWindowHitTest(e.Window);
                break;
            case { Type: EventType.WindowLeaveFullscreen }:
                OnWindowLeaveFullscreen(e.Window);
                break;
            case { Type: EventType.WindowMaximized }:
                OnWindowMaximized(e.Window);
                break;
            case { Type: EventType.WindowMetalViewResized }:
                OnWindowMetalViewResized(e.Window);
                break;
            case { Type: EventType.WindowMinimized }:
                OnWindowMinimized(e.Window);
                break;
            case { Type: EventType.WindowMouseEnter }:
                OnWindowMouseEnter(e.Window);
                break;
            case { Type: EventType.WindowMouseLeave }:
                OnWindowMouseLeave(e.Window);
                break;
            case { Type: EventType.WindowMoved }:
                OnWindowMoved(e.Window);
                break;
            case { Type: EventType.WindowOccluded }:
                OnWindowOccluded(e.Window);
                break;
            case { Type: EventType.WindowPixelSizeChanged }:
                OnWindowPixelSizeChanged(e.Window);
                break;
            case { Type: EventType.WindowResized }:
                OnWindowResized(e.Window);
                break;
            case { Type: EventType.WindowRestored }:
                OnWindowRestored(e.Window);
                break;
            case { Type: EventType.WindowSafeAreaChanged }:
                OnWindowSafeAreaChanged(e.Window);
                break;
            default:
                _log.Error($"Unhandled event type: {e.Type}");
                break;
        }
    }

    protected virtual void Dispose(bool disposing) {
        if (disposedValue) {
            return;
        }
        if (disposing) {
            // No managed objects to dispose
        }

        Sdl.DestroyWindow(WindowPtr);
        disposedValue = true;
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}