using Godot;
using System.Collections.Generic;

public class Neuron : Panel
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

    public Line2D CreateLine(Control target) {
        var line = new Line2D();
        line.ZIndex = -2;
        AddChild(line);
        lines[target] = line;
        return line;
    }

    public void UpdateNeuron(Control target, float bias) {
        lines[target].Width = sigmoid(bias) * 2;
        lines[target].DefaultColor = bias < 0 ? Colors.Red : Colors.Blue;  
    }

    public void ConnectToNode(Control target, float bias) {
        if (lines.ContainsKey(target)) {
            UpdateNeuron(target, bias);
        }
        else {
            var line = CreateLine(target);
            var image = GetNode<Sprite>("Node");
            line.AddPoint(RectSize / 2);
            line.AddPoint(image.ToLocal(target.GetGlobalTransform().origin + target.RectSize));
            UpdateNeuron(target, bias);
        }
    }

    private float sigmoid(float x) {
        return (1f) / (1f + Mathf.Exp(-x));
    }
}
