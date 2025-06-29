using Godot;

namespace Support.Scripts.Widgets;

public partial class SliderBox : HBoxContainer
{
    [Signal] internal delegate void ValueChangedEventHandler(float value);
    private bool editable = true;
    private float value = 0;
    private float minValue = 0;
    private float maxValue = 100;
    private float step = 1;
    private bool rounded = false;
    private bool allowGreater = false;
    private bool allowLesser = false;
    [Export] private HSlider? slider;
    [Export] private SpinBox? spinBox;
    [Export]
    internal bool Editable
    {
        get => editable;
        set
        {
            editable = value;
            slider!.Editable = value;
            spinBox!.Editable = value;
        }
    }
    [Export]
    internal float Value
    {
        get => value;
        set
        {
            this.value = value;
            slider!.Value = value;
            spinBox!.Value = value;
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
            spinBox!.MinValue = value;
            slider!.MinValue = value;
        }
    }
    [Export]
    internal float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = value;
            spinBox!.MaxValue = value;
            slider!.MaxValue = value;
        }
    }
    [Export]
    internal float Step
    {
        get => step;
        set
        {
            step = value;
            spinBox!.Step = value;
            slider!.Step = value;
        }
    }
    [Export]
    internal bool Rounded
    {
        get => rounded;
        set
        {
            rounded = value;
            spinBox!.Rounded = value;
            slider!.Rounded = value;
        }
    }
    [Export]
    internal bool AllowGreater
    {
        get => allowGreater;
        set
        {
            allowGreater = value;
            spinBox!.AllowGreater = value;
            slider!.AllowGreater = value;
        }
    }
    [Export]
    internal bool AllowLesser
    {
        get => allowLesser;
        set
        {
            allowLesser = value;
            spinBox!.AllowLesser = value;
            slider!.AllowLesser = value;
        }
    }
    internal void SetValueNoSignal(float value)
    {
        spinBox!.SetValueNoSignal(value);
        slider!.SetValueNoSignal(value);
    }
    private void OnSpinBoxValueChanged(float value)
    {
        this.value = value;
        slider!.SetValueNoSignal(value);
        EmitSignalValueChanged(this.value);
    }
    private void OnSliderValueChanged(float value)
    {
        this.value = value;
        spinBox!.SetValueNoSignal(value);
        EmitSignalValueChanged(this.value);
    }
}
