using Godot;
using System.Collections.Generic;

/// <summary>
/// Class visually representing an Axon in the context
/// of the neural network
/// </summary>
public class Axon : Panel
{
    private float _bias;
    private Dictionary<Control, Line2D> lines = new Dictionary<Control, Line2D>();

    public float Bias 
    { 
        get => _bias; 
        set  {
            _bias = value;
            this.Get<Label>("Node2D/Label").Text = value.ToString("0.000");
        } 
    }

    private int _index;

    public int IndexIndicator {
        get => _index;
        set {
            _index = value;
            if (this.Get<Label>("Node2D/Index") is Label label) {
                label.Show();
                label.Text = value.ToString();
            }
        }
    }

    /// <summary>
    /// Creates a visual 2d line connecting the Axon to the passed
    /// target.
    /// </summary>
    /// <param name="target">The target to connect to</param>
    /// <returns>The visual 2d line</returns>
    public Line2D CreateLine(Control target) {
        var line = new Line2D();
        line.ZIndex = -2;
        AddChild(line);
        lines[target] = line;
        return line;
    }

    /// <summary>
    /// Updates the neuron connection to the passed target with the new weight value
    /// </summary>
    /// <param name="target">The target connection</param>
    /// <param name="weight">The new weight bias</param>
    public void UpdateAxon(Control target, float weight) {
        lines[target].Width = sigmoid(weight) * 2;
        lines[target].DefaultColor = weight < 0 ? Colors.Red : Colors.Blue;  
    }

    /// <summary>
    /// Connects the Axon to the passed target with the given weight.
    /// </summary>
    /// <param name="target">The target to connect to</param>
    /// <param name="weight">The weight value of the connection</param>
    public void ConnectToNode(Control target, float weight) {
        if (lines.ContainsKey(target)) {
            UpdateAxon(target, weight);
        }
        else {
            var line = CreateLine(target);
            var image = GetNode<Sprite>("Node");
            line.AddPoint(RectSize / 2);
            line.AddPoint(image.ToLocal(target.GetGlobalTransform().origin + target.RectSize));
            UpdateAxon(target, weight);
        }
    }

    /// <summary>
    /// Helper method to calculate the sigmoid value
    /// </summary>
    /// <param name="x">The value to calculate the sigmoid value of</param>
    /// <returns>The sigmoid of x</returns>
    private float sigmoid(float x) {
        return (1f) / (1f + Mathf.Exp(-x));
    }
}
