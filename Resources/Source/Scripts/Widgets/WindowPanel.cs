using Godot;
using Support;
using Support.Geometrics;
using Support.Numerics;

namespace Support.Scripts.Widgets;

internal partial class WindowPanel : Panel
{
    [Signal] internal delegate void CloseRequestEventHandler();
    [Export] private Label? tileLabel;
    [Export] private MarginContainer? borderMargin;
    [Export] private VBoxContainer? windowRawContent;
    /// <summary>
    /// Direction at pick moment.
    /// </summary>
    private Direction pickDirection;
    /// <summary>
    /// Rectangle at pick moment.
    /// </summary>
    private Rectangle<float> pickRectangle;
    /// <summary>
    /// Events from the mouse drag move zone.
    /// </summary>
    private readonly MouseObserver mouseDragMove = new();
    /// <summary>
    /// Events from the mouse drag resize zone.
    /// </summary>
    private readonly MouseObserver mouseDragResize = new();
    [Export]
    internal string? Tile
    {
        get => tileLabel!.Text;
        set => tileLabel!.Text = value;
    }
    private void OnCloseButtonPressed() => EmitSignalCloseRequest();
    public override void _Ready()
    {
        mouseDragMove.TargetMask = MouseButtonMask.Left;
        mouseDragMove.DragMoved += OnWindowDragMoved;

        mouseDragResize.TargetMask = MouseButtonMask.Left;
        mouseDragResize.Moved += OnWindowResizeMoved;
        mouseDragResize.DragStarted += OnWindowResizeDragStarted;
        mouseDragResize.DragMoved += OnWindowResizeDragMoved;
        mouseDragResize.DragEnded += OnWindowResizeDragEnded;
    }
    private void OnMoveZone(InputEvent @event)
    {
        if (@event is InputEventMouse mouseEvent) { mouseDragMove.FeedMouseEvents(mouseEvent); }
    }
    private void OnResizeZone(InputEvent @event)
    {
        if (@event is InputEventMouse mouseEvent) { mouseDragResize.FeedMouseEvents(mouseEvent); }
    }
    private void OnWindowDragMoved(in MouseObserver.Stats stats) => GlobalPosition += stats.RelativeLast;
    private void OnWindowResizeMoved(in MouseObserver.Stats stats)
    {
        if (mouseDragResize.IsDragging || mouseDragMove.IsDragging) { return; }
        var innerRect = windowRawContent!.GetGlobalRect().ToRectangle();
        var direction = innerRect.PointOutsideDirection(stats.CurrentPosition.ToVec2());
        borderMargin!.MouseDefaultCursorShape = PickCursor(direction);  //TODO: make it more efficient, only update when changed
    }
    private void OnWindowResizeDragStarted(in MouseObserver.Stats stats)
    {
        var innerRect = windowRawContent!.GetGlobalRect().ToRectangle();
        pickDirection = innerRect.PointOutsideDirection(stats.InitialPosition.ToVec2());
        pickRectangle = new(GlobalPosition.ToVec2(), Size.ToVec2());
    }
    private void OnWindowResizeDragMoved(in MouseObserver.Stats stats)
    {
        var resizedRect = CalculateResizedRect(stats, pickDirection);
        GlobalPosition = resizedRect.Min.ToGodotVector2();
        Size = resizedRect.Size.ToGodotVector2();
    }
    private void OnWindowResizeDragEnded(in MouseObserver.Stats _)
    {
        pickDirection = Direction.None;
        borderMargin!.MouseDefaultCursorShape = PickCursor(Direction.None);
    }
    private Rectangle<float> CalculateResizedRect(in MouseObserver.Stats stats, in Direction dir)
    {
        var dirVec = dir.ToVec2Int();
        var calculatedSize = pickRectangle.Size + stats.RelativeInitial.ToVec2() * dirVec.CastTo<float>();

        var minSize = GetCombinedMinimumSize().ToVec2();
        calculatedSize = calculatedSize.Max(minSize);

        var result = pickRectangle;
        switch (dir.asEnum)
        {
            case Direction.ENUM.UP:
            case Direction.ENUM.UP_LEFT:
            case Direction.ENUM.LEFT:
                result.Min = pickRectangle.Max - calculatedSize;
                break;
            case Direction.ENUM.DOWN:
            case Direction.ENUM.DOWN_RIGHT:
            case Direction.ENUM.RIGHT:
                result.Max = pickRectangle.Min + calculatedSize;
                break;
            case Direction.ENUM.UP_RIGHT:
                result.Max = result.Max with { x = result.Min.x + calculatedSize.x };
                result.Min = result.Min with { y = result.Max.y - calculatedSize.y };
                break;
            case Direction.ENUM.DOWN_LEFT:
                result.Min = result.Min with { x = result.Max.x - calculatedSize.x };
                result.Max = result.Max with { y = result.Min.y + calculatedSize.y };
                break;
        }
        return result;
    }
    private static CursorShape PickCursor(in Direction pickDirection)
    {
        if (pickDirection.IsNone) { return CursorShape.Arrow; }
        if (pickDirection.IsCardinal)
        {
            if (pickDirection.IsHorizontal) { return CursorShape.Hsize; }
            return CursorShape.Vsize;
        }
        else
        {
            if (pickDirection.IsTopLeft) { return CursorShape.Fdiagsize; }
            return CursorShape.Bdiagsize;
        }
    }
}
