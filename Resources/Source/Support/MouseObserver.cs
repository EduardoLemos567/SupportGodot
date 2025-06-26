using Godot;

namespace Support;

public class MouseObserver
{
    public delegate void ClickedHandler(in Vector2 startPosition);
    public delegate void MovedHandler(in Vector2 position);
    public delegate void DragStartedHandler(in Vector2 startPosition);
    public delegate void DragMovedHandler(in Vector2 position, in Vector2 deltaPosition);
    public delegate void DragEndedHandler(in Vector2 endPosition);
    public Vector2? FirstClickPosition { get; private set; }
    public Vector2? LastDraggedPosition { get; private set; }
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
    private void ProcessButton(InputEventMouseButton @event)
    {
        if (TargetMask != 0 && @event.ButtonMask != TargetMask)
        {
            return; // Ignore events not matching the target mask
        }
        if (@event.IsPressed())
        {
            FirstClickPosition = @event.GlobalPosition;
            Clicked?.Invoke(FirstClickPosition.Value);
        }
        else if (IsDragging && @event.IsReleased())
        {
            ForceRelease();
        }
    }
    private void ProcessMotion(InputEventMouseMotion @event)
    {
        if (IsDragging)
        {
            LastDraggedPosition = @event.GlobalPosition;
            DragMoved?.Invoke(LastDraggedPosition.Value, LastDraggedPosition.Value - FirstClickPosition!.Value);
        }
        else if (FirstClickPosition.HasValue)
        {
            IsDragging = true;
            DragStarted?.Invoke(FirstClickPosition.Value);
        }
        Moved?.Invoke(@event.GlobalPosition);
    }
    public void ForceRelease()
    {
        FirstClickPosition = null;
        if (IsDragging)
        {
            IsDragging = false;
            DragEnded?.Invoke(LastDraggedPosition!.Value);
            LastDraggedPosition = null;
        }
    }
}
