using Godot;

/// <summary>
/// Custom control class representing a larger spin box dial
/// </summary>
public class Dial : Control
{
    [Export]
    public int MaxValue  {get; set;} = 100;

    [Export]
    public int MinValue  {get; set;} = 0;

    private int _value;

    public int Value 
    {
        get => _value; 
        set {
            _value = value;
            label.Text = Value.ToString();
        }
    }

    private Label label;

    /// <summary>
    /// Initializes the parameters
    /// </summary>
    public override void _Ready()
    {
        label = GetNode<Label>("Value");
        Value = MinValue;
    }

    /// <summary>
    /// Down button pressed signal catch
    /// </summary>
    private void _on_Down_pressed() {
        Value = Mathf.Max(MinValue, Value - 1);
    }

    /// <summary>
    /// Up button pressed signal catch
    /// </summary>
    private void _on_Up_pressed() {
        Value = Mathf.Min(Value + 1, MaxValue);
    }
}
