using Godot;

namespace Support.Scripts.Widgets;

internal partial class Vector3Box : HBoxContainer
{
    [Signal] internal delegate void ValueChangedEventHandler(Vector3 value);
    private bool editable = true;
    private Vector3 value = Vector3.Zero;
    private float minValue = 0;
    private float maxValue = 100;
    private float step = 1;
    private bool rounded = false;
    private bool allowGreater = false;
    private bool allowLesser = false;
    [Export] private SpinBox? xSpinBox;
    [Export] private SpinBox? ySpinBox;
    [Export] private SpinBox? zSpinBox;
    [Export]
    internal bool Editable
    {
        get => editable;
        set
        {
            editable = value;
            xSpinBox!.Editable = value;
            ySpinBox!.Editable = value;
            zSpinBox!.Editable = value;
        }
    }
    [Export]
    internal Vector3 Value
    {
        get => value;
        set
        {
            this.value = value;
            xSpinBox!.Value = value.X;
            ySpinBox!.Value = value.Y;
            zSpinBox!.Value = value.Y;
            EmitSignalValueChanged(value);
        }
    }
    [Export]
    internal float MinValue
    {
        get => minValue;
        set
        {
            minValue = value;
            xSpinBox!.MinValue = value;
            ySpinBox!.MinValue = value;
            zSpinBox!.MinValue = value;
        }
    }
    [Export]
    internal float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = value;
            xSpinBox!.MaxValue = value;
            ySpinBox!.MaxValue = value;
            zSpinBox!.MaxValue = value;
        }
    }
    [Export]
    internal float Step
    {
        get => step;
        set
        {
            step = value;
            xSpinBox!.Step = value;
            ySpinBox!.Step = value;
            zSpinBox!.Step = value;
        }
    }
    [Export]
    internal bool Rounded
    {
        get => rounded;
        set
        {
            rounded = value;
            xSpinBox!.Rounded = value;
            ySpinBox!.Rounded = value;
            zSpinBox!.Rounded = value;
        }
    }
    [Export]
    internal bool AllowGreater
    {
        get => allowGreater;
        set
        {
            allowGreater = value;
            xSpinBox!.AllowGreater = value;
            ySpinBox!.AllowGreater = value;
            zSpinBox!.AllowGreater = value;
        }
    }
    [Export]
    internal bool AllowLesser
    {
        get => allowLesser;
        set
        {
            allowLesser = value;
            xSpinBox!.AllowLesser = value;
            ySpinBox!.AllowLesser = value;
            zSpinBox!.AllowLesser = value;
        }
    }
    internal void SetValueNoSignal(Vector3 value)
    {
        xSpinBox!.SetValueNoSignal(value.X);
        ySpinBox!.SetValueNoSignal(value.Y);
        zSpinBox!.SetValueNoSignal(value.Z);
    }
    private void OnXValueChanged(float value)
    {
        this.value.X = value;
        EmitSignalValueChanged(this.value);
    }
    private void OnYValueChanged(float value)
    {
        this.value.Y = value;
        EmitSignalValueChanged(this.value);
    }
    private void OnZValueChanged(float value)
    {
        this.value.Z = value;
        EmitSignalValueChanged(this.value);
    }
}
