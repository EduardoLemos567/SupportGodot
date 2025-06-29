using Godot;

namespace Support;

public class MouseEventsObserver
{
    public readonly ref struct Stats
    {
        public Vector2 InitialPosition { get; init; }
        public Vector2 CurrentPosition { get; init; }
        public Vector2 RelativeLast { get; init; }
        public Vector2 RelativeInitial => CurrentPosition - InitialPosition;
    }
    public delegate void ClickedHandler(in Stats stats);
    public delegate void MovedHandler(in Stats stats);
    public delegate void DragStartedHandler(in Stats stats);
    public delegate void DragMovedHandler(in Stats stats);
    public delegate void DragEndedHandler(in Stats stats);
    public Vector2? ClickPosition { get; private set; }
    public bool IsDragging { get; private set; }
    /// <summary>
    /// Mask of mouse buttons that this observer is interested in. If zero, it will process all inputs.
    /// </summary>
    public MouseButtonMask TargetMask { get; set; }
    public event ClickedHandler? Clicked;
    public event MovedHandler? Moved;
    public event DragStartedHandler? DragStarted;
    public event DragMovedHandler? DragMoved;
    public event DragEndedHandler? DragEnded;
    public void FeedMouseEvents(InputEventMouse @event)
    {
        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                ProcessButton(mouseButton);
                break;
            case InputEventMouseMotion mouseMotion:
                ProcessMotion(mouseMotion);
                break;
        }
    }
    private void ProcessButton(InputEventMouseButton buttonEvent)
    {
        if (buttonEvent.IsPressed())
        {
            if (TargetMask != 0 && buttonEvent.ButtonMask != TargetMask)
            {
                return; // Ignore press events not matching the target mask.
            }
            ClickPosition = buttonEvent.GlobalPosition;
            Clicked?.Invoke(new()
            {
                InitialPosition = ClickPosition.Value,
                CurrentPosition = ClickPosition.Value,
            });
        }
        else if (buttonEvent.IsReleased())
        {
            if (IsDragging)
            {
                IsDragging = false;
                DragEnded?.Invoke(new Stats()
                {
                    InitialPosition = ClickPosition!.Value,
                    CurrentPosition = buttonEvent.GlobalPosition,
                });
            }
            ClickPosition = null;
        }
    }
    private void ProcessMotion(InputEventMouseMotion motionEvent)
    {
        if (IsDragging)
        {
            DragMoved?.Invoke(new Stats()
            {
                InitialPosition = ClickPosition!.Value,
                CurrentPosition = motionEvent.GlobalPosition,
                RelativeLast = motionEvent.Relative,
            });
        }
        else if (ClickPosition.HasValue)
        {
            IsDragging = true;
            DragStarted?.Invoke(new()
            {
                InitialPosition = ClickPosition.Value,
                CurrentPosition = motionEvent.GlobalPosition,
                RelativeLast = motionEvent.Relative,
            });
        }
        Moved?.Invoke(new Stats()
        {
            InitialPosition = motionEvent.GlobalPosition - motionEvent.Relative,
            CurrentPosition = motionEvent.GlobalPosition,
            RelativeLast = motionEvent.Relative,
        });
    }
    public void ForceRelease()
    {
        IsDragging = false;
        ClickPosition = null;
    }
}
