using SDL2;
using TestGame.GameObjects;

namespace TestGame;

public abstract class Window : Renderer, IRenderer {
    private static readonly Logger? _log = Logger.GetCurrentClassLogger(LogCategory.Video);
    #region Events

    public event EventHandler? AppDidEnterBackground;
    public event EventHandler? AppDidEnterForeground;
    public event EventHandler? AppLowMemory;
    public event EventHandler? AppTerminating;
    public event EventHandler? AppWillEnterBackground;
    public event EventHandler? AppWillEnterForeground;
    public event EventHandler< AudioDeviceEvent >? AudioDeviceAdded;
    public event EventHandler< AudioDeviceEvent >? AudioDeviceRemoved;
    public event EventHandler? ClipboardUpdate;
    public event EventHandler< ControllerAxisEvent >? ControllerAxisMotion;
    public event EventHandler< ControllerButtonEvent >? ControllerButtonDown;
    public event EventHandler< ControllerButtonEvent >? ControllerButtonUp;
    public event EventHandler< ControllerDeviceEvent >? ControllerDeviceAdded;
    public event EventHandler< ControllerDeviceEvent >? ControllerDeviceRemoved;
    public event EventHandler< ControllerDeviceEvent >? ControllerDeviceRemapped;
    public event EventHandler< DisplayEvent >? DisplayEvents;
    public event EventHandler< DollarGestureEvent >? DollarGesture;
    public event EventHandler< DollarGestureEvent >? DollarRecord;
    public event EventHandler< DropEvent >? DropBegin;
    public event EventHandler< DropEvent >? DropComplete;
    public event EventHandler< DropEvent >? DropFile;
    public event EventHandler< DropEvent >? DropText;
    public event EventHandler< TouchFingerEvent >? FingerDown;
    public event EventHandler< TouchFingerEvent >? FingerMotion;
    public event EventHandler< TouchFingerEvent >? FingerUp;
    public event EventHandler? FirstEvent;
    public event EventHandler? LastEvent;
    public event EventHandler< JoyAxisEvent >? JoyAxisMotion;
    public event EventHandler< JoyBallEvent >? JoyBallMotion;
    public event EventHandler< JoyButtonEvent >? JoyButtonDown;
    public event EventHandler< JoyButtonEvent >? JoyButtonUp;
    public event EventHandler< JoyDeviceEvent >? JoyDeviceAdded;
    public event EventHandler< JoyDeviceEvent >? JoyDeviceRemoved;
    public event EventHandler< JoyHatEvent >? JoyHatMotion;
    //public event EventHandler< JoyBatteryEvent >? JoyBatteryUpdated;
    public event EventHandler< KeyboardEvent >? KeymapChanged;
    public event EventHandler< KeyboardEvent >? KeyDown;
    public event EventHandler< KeyboardEvent >? KeyUp;
    public event EventHandler< MouseButtonEvent >? MouseDown;
    public event EventHandler< MouseButtonEvent >? MouseUp;
    public event EventHandler< MouseMotionEvent >? MouseMove;
    public event EventHandler< MouseWheelEvent >? MouseWheel;
    public event EventHandler< MultiGestureEvent >? MultiGesture;
    public event EventHandler< QuitEvent >? Quit;
    public event EventHandler? RenderDeviceReset;
    public event EventHandler? RenderTargetsReset;
    public event EventHandler< SensorEvent >? SensorUpdate;
    public event EventHandler< SysWMEvent >? SysWMEvent;
    public event EventHandler< TextEditingEvent >? TextEditing;
    public event EventHandler< TextInputEvent >? TextInput;
    public event EventHandler< UserEvent >? UserEvent;
    public event EventHandler< WindowEvent >? WindowEvent;
    public event EventHandler? PollSentinel;

    #endregion

    protected nint WindowPtr { get; }


    ~Window() {
        SDL.DestroyWindow(WindowPtr);
    }

    protected Window(string title, Point location, Size size, WindowFlags flags) {
        int x, y;
        X = x = location.X;
        Y = y = location.Y;

        Width = size.Width;
        Height = size.Height;

        WindowPtr = SDL.CreateWindow(title,
            x == 0x7FFFFFFF ? SDL.WINDOWPOS_CENTERED : x,
            y == 0x7FFFFFFF ? SDL.WINDOWPOS_CENTERED : y,
            Width,
            Height,
            flags);

        if (WindowPtr == nint.Zero) {
            _log?.Error($"Cannot create Window: {SDL.GetError()}");
        }

        Initialize(SDL.CreateRenderer(WindowPtr, -1, RendererFlags.Accelerated | RendererFlags.TargetTexture));

        if (RendererPtr == nint.Zero) {
            _log?.Error($"Cannot create RendererPtr: {SDL.GetError()}");
        }
    }

    /// <inheritdoc />
    public float X { get; protected set; }

    /// <inheritdoc />
    public float Y { get; protected set; }

    /// <inheritdoc />
    public float Z { get; } = float.MaxValue;

    /// <inheritdoc />
    public int Width { get; protected set; }

    /// <inheritdoc />
    public int Height { get; protected set; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract void Draw();

    public Event CurrentEvent { get; protected set; }

    /// <inheritdoc />
    public virtual void Update(Event e) {
        CurrentEvent = e;
        HandleEvents(e);
    }

    public void UpdatePosition(nint window) {
        SDL.GetWindowPosition(window, out int x, out int y);
        X = x;
        Y = y;
    }

    public void UpdateSize(nint window) {
        SDL.GetWindowSize(window, out int w, out int h);
        Width = w;
        Height = h;
    }

    #region Event Handler

    private void HandleEvents(Event e) {
        switch (e) {
            case { Type: EventType.FirstEvent }:
                OnFirstEvent(e);
                break;
            case { Type: EventType.Quit }:
                OnQuit(e.Quit);
                break;
            case { Type: EventType.AppTerminating }:
                OnAppTerminating(e);
                break;
            case { Type: EventType.AppLowMemory }:
                OnAppLowMemory(e);
                break;
            case { Type: EventType.AppWillEnterBackground }:
                OnAppWillEnterBackground(e);
                break;
            case { Type: EventType.AppDidEnterBackground }:
                OnAppDidEnterBackground(e);
                break;
            case { Type: EventType.AppWillEnterForeground }:
                OnAppWillEnterForeground(e);
                break;
            case { Type: EventType.AppDidEnterForeground }:
                OnAppDidEnterForeground(e);
                break;
            case { Type: EventType.DisplayEvent }:
                OnDisplayEvent(e.Display);
                break;
            case { Type: EventType.WindowEvent }:
                OnWindowEvent(e.Window);
                break;
            case { Type: EventType.SyswmEvent }:
                OnSysWmEvent(e.Syswm);
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
                OnKeymapChanged(e.Key);
                break;
            case { Type: EventType.MouseMotion }:
                OnMouseMove(e.Motion);
                break;
            case { Type: EventType.MouseButtonDown }:
                OnMouseDown(e.Button);
                break;
            case { Type: EventType.MouseButtonUp }:
                OnMouseUp(e.Button);
                break;
            case { Type: EventType.MouseWheel }:
                OnMouseWheel(e.Wheel);
                break;
            case { Type: EventType.JoyAxisMotion }:
                OnJoyAxisMotion(e.JAxis);
                break;
            case { Type: EventType.JoyBallMotion }:
                OnJoyBallMotion(e.JBall);
                break;
            case { Type: EventType.JoyHatMotion }:
                OnJoyHatMotion(e.JHat);
                break;
            case { Type: EventType.JoyButtonDown }:
                OnJoyButtonDown(e.JButton);
                break;
            case { Type: EventType.JoyButtonUp }:
                OnJoyButtonUp(e.JButton);
                break;
            case { Type: EventType.JoyDeviceAdded }:
                OnJoyDeviceAdded(e.JDevice);
                break;
            case { Type: EventType.JoyDeviceRemoved }:
                OnJoyDeviceRemoved(e.JDevice);
                break;
            case { Type: (EventType)0x607 }: // SDL_JOYBATTERYUPDATED

                break;
            case { Type: EventType.ControllerAxisMotion }:
                OnControllerAxisMotion(e.CAxis);
                break;
            case { Type: EventType.ControllerButtonDown }:
                OnControllerButtonDown(e.CButton);
                break;
            case { Type: EventType.ControllerButtonUp }:
                OnControllerButtonUp(e.CButton);
                break;
            case { Type: EventType.ControllerDeviceAdded }:
                OnControllerDeviceAdded(e.CDevice);
                break;
            case { Type: EventType.ControllerDeviceRemoved }:
                OnControllerDeviceRemoved(e.CDevice);
                break;
            case { Type: EventType.ControllerDeviceRemapped }:
                OnControllerDeviceRemapped(e.CDevice);
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
            case { Type: EventType.DollarGesture }:
                OnDollarGesture(e.DGesture);
                break;
            case { Type: EventType.DollarRecord }:
                OnDollarRecord(e.DGesture);
                break;
            case { Type: EventType.MultiGesture }:
                OnMultiGesture(e.MGesture);
                break;
            case { Type: EventType.ClipboardUpdate }:
                OnClipboardUpdate(e);
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
                OnRenderTargetsReset(e);
                break;
            case { Type: EventType.RenderDeviceReset }:
                OnRenderDeviceReset(e);
                break;
            case { Type: EventType.UserEvent }:
                OnUserEvent(e.User);
                break;
            case { Type: EventType.LastEvent }:
                OnLastEvent(e);
                break;
            case { Type: (EventType)0x7F00 }: // SDL_POLLSENTINEL
                OnPollSentinel(e);
                break;
            default:
                _log?.Error($"Unhandled event type: {e.Type}");
                break;
        }
    }

    protected virtual void OnAppDidEnterBackground(Event @event) {
        AppDidEnterBackground?.Invoke(this, @event);
    }

    protected virtual void OnAppDidEnterForeground(Event @event) {
        AppDidEnterForeground?.Invoke(this, @event);
    }

    protected virtual void OnAppLowMemory(Event @event) {
        AppLowMemory?.Invoke(this, @event);
    }

    protected virtual void OnAppTerminating(Event @event) {
        AppTerminating?.Invoke(this, @event);
    }

    protected virtual void OnAppWillEnterBackground(Event @event) {
        AppWillEnterBackground?.Invoke(this, @event);
    }

    protected virtual void OnAppWillEnterForeground(Event @event) {
        AppWillEnterForeground?.Invoke(this, @event);
    }

    protected virtual void OnAudioDeviceAdded(AudioDeviceEvent eventADevice) {
        AudioDeviceAdded?.Invoke(this, eventADevice);
    }

    protected virtual void OnAudioDeviceRemoved(AudioDeviceEvent eventADevice) {
        AudioDeviceRemoved?.Invoke(this, eventADevice);
    }

    protected virtual void OnClipboardUpdate(Event @event) {
        ClipboardUpdate?.Invoke(this, @event);
    }

    protected virtual void OnControllerAxisMotion(ControllerAxisEvent eventCAxis) {
        ControllerAxisMotion?.Invoke(this, eventCAxis);
    }

    protected virtual void OnControllerButtonDown(ControllerButtonEvent eventCButton) {
        ControllerButtonDown?.Invoke(this, eventCButton);
    }

    protected virtual void OnControllerButtonUp(ControllerButtonEvent eventCButton) {
        ControllerButtonUp?.Invoke(this, eventCButton);
    }

    protected virtual void OnControllerDeviceAdded(ControllerDeviceEvent eventCDevice) {
        ControllerDeviceAdded?.Invoke(this, eventCDevice);
    }

    protected virtual void OnControllerDeviceRemapped(ControllerDeviceEvent eventCDevice) {
        ControllerDeviceRemapped?.Invoke(this, eventCDevice);
    }

    protected virtual void OnControllerDeviceRemoved(ControllerDeviceEvent eventCDevice) {
        ControllerDeviceRemoved?.Invoke(this, eventCDevice);
    }

    protected virtual void OnDisplayEvent(DisplayEvent eventDisplay) {
        DisplayEvents?.Invoke(this, eventDisplay);
    }

    protected virtual void OnDollarGesture(DollarGestureEvent eventDGesture) {
        DollarGesture?.Invoke(this, eventDGesture);
    }

    protected virtual void OnDollarRecord(DollarGestureEvent eventDGesture) {
        DollarRecord?.Invoke(this, eventDGesture);
    }

    protected virtual void OnDropBegin(DropEvent @event) {
        DropBegin?.Invoke(this, @event);
    }

    protected virtual void OnDropComplete(DropEvent @event) {
        DropComplete?.Invoke(this, @event);
    }

    protected virtual void OnDropFile(DropEvent eventDrop) {
        DropFile?.Invoke(this, eventDrop);
    }

    protected virtual void OnDropText(DropEvent eventDrop) {
        DropText?.Invoke(this, eventDrop);
    }

    protected virtual void OnFingerDown(TouchFingerEvent eventFinger) {
        FingerDown?.Invoke(this, eventFinger);
    }

    protected virtual void OnFingerMotion(TouchFingerEvent eventFinger) {
        FingerMotion?.Invoke(this, eventFinger);
    }

    protected virtual void OnFingerUp(TouchFingerEvent eventFinger) {
        FingerUp?.Invoke(this, eventFinger);
    }

    protected virtual void OnFirstEvent(Event @event) {
        FirstEvent?.Invoke(this, @event);
    }

    protected virtual void OnJoyAxisMotion(JoyAxisEvent eventJAxis) {
        JoyAxisMotion?.Invoke(this, eventJAxis);
    }

    protected virtual void OnJoyBallMotion(JoyBallEvent eventJBall) {
        JoyBallMotion?.Invoke(this, eventJBall);
    }

    protected virtual void OnJoyButtonDown(JoyButtonEvent eventJButton) {
        JoyButtonDown?.Invoke(this, eventJButton);
    }

    protected virtual void OnJoyButtonUp(JoyButtonEvent eventJButton) {
        JoyButtonUp?.Invoke(this, eventJButton);
    }

    protected virtual void OnJoyDeviceAdded(JoyDeviceEvent eventJDevice) {
        JoyDeviceAdded?.Invoke(this, eventJDevice);
    }

    protected virtual void OnJoyDeviceRemoved(JoyDeviceEvent eventJDevice) {
        JoyDeviceRemoved?.Invoke(this, eventJDevice);
    }

    //protected virtual void OnJoyBatteryUpdated(JoyBatteryEvent eventJBattery) {
        //JoyBatteryUpdated?.Invoke(this, eventJBattery);
    //}
    protected virtual void OnJoyHatMotion(JoyHatEvent eventJHat) {
        JoyHatMotion?.Invoke(this, eventJHat);
    }

    protected virtual void OnKeyDown(KeyboardEvent eventKey) {
        KeyDown?.Invoke(this, eventKey);
    }

    protected virtual void OnKeymapChanged(KeyboardEvent eventKey) {
        KeymapChanged?.Invoke(this, eventKey);
    }

    protected virtual void OnKeyUp(KeyboardEvent eventKey) {
        KeyUp?.Invoke(this, eventKey);
    }

    protected virtual void OnLastEvent(Event @event) {
        LastEvent?.Invoke(this, @event);
    }

    protected virtual void OnMouseDown(MouseButtonEvent eventButton) {
        MouseDown?.Invoke(this, eventButton);
    }

    protected virtual void OnMouseMove(MouseMotionEvent eventMotion) {
        MouseMove?.Invoke(this, eventMotion);
    }

    protected virtual void OnMouseUp(MouseButtonEvent eventButton) {
        MouseUp?.Invoke(this, eventButton);
    }

    protected virtual void OnMouseWheel(MouseWheelEvent eventWheel) {
        MouseWheel?.Invoke(this, eventWheel);
    }

    protected virtual void OnMultiGesture(MultiGestureEvent eventMGesture) {
        MultiGesture?.Invoke(this, eventMGesture);
    }

    protected virtual void OnQuit(QuitEvent @event) {
        Quit?.Invoke(this, @event);
    }

    protected virtual void OnRenderDeviceReset(Event @event) {
        RenderDeviceReset?.Invoke(this, @event);
    }

    protected virtual void OnRenderTargetsReset(Event @event) {
        RenderTargetsReset?.Invoke(this, @event);
    }

    protected virtual void OnSensorUpdate(SensorEvent eventSensor) {
        SensorUpdate?.Invoke(this, eventSensor);
    }

    protected virtual void OnSysWmEvent(SysWMEvent eventSysWm) {
        SysWMEvent?.Invoke(this, eventSysWm);
    }

    protected virtual void OnTextEditing(TextEditingEvent eventEdit) {
        TextEditing?.Invoke(this, eventEdit);
    }

    protected virtual void OnTextInput(TextInputEvent eventText) {
        TextInput?.Invoke(this, eventText);
    }

    protected virtual void OnUserEvent(UserEvent eventUser) {
        UserEvent?.Invoke(this, eventUser);
    }

    protected virtual void OnWindowEvent(WindowEvent eventWindow) {
        WindowEvent?.Invoke(this, eventWindow);
    }

    protected virtual void OnPollSentinel(Event @event) {
        PollSentinel?.Invoke(this, @event);
    }

    #endregion
}