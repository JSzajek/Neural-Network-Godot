using Godot;
using System.Linq;
using System.Collections.Generic;

public class SetupWindow : WindowDialog
{
    private Dial dial;
    private HBoxContainer hiddenLayers;
    private Button minus;

    public override void _Ready()
    {
        dial = (GD.Load("res://_Assets/Dial.tscn") as PackedScene)?.Instance() as Dial;
        hiddenLayers = GetNode<HBoxContainer>("HiddenLayers/ScrollContainer/HBoxContainer");
        minus = GetNode<Button>("HiddenLayers/Minus");
        AddDial();
    }

    private void AddDial() {
        var toAdd = dial.Duplicate() as Dial;
        toAdd.MinValue = 1;
        hiddenLayers.AddChild(toAdd);
        if (minus.Disabled) {
            minus.Disabled = false;
        }
    }

    private void RemoveDial() {
        var dials = hiddenLayers.GetChildren<Dial>();
        var toRemove = dials.LastOrDefault();
        dials.Remove(toRemove);
        toRemove.QueueFree();

        if (dials.Count == 0) {
            minus.Disabled = true;
        }
    }

    private void _on_Create_pressed() {
        var topology = GetTopology();

        if (topology.Count <= 2) {
            return; // Indicate not enough layers
        }
        Visible = false;
        if (GetParent() is DenseNetworkVisualizer visualizer) {
            visualizer.SetUp(topology, false);
        }
    }

    private List<int> GetTopology() {
        var topology = new List<int>();
        topology.Add((int)this.Get<Dial>("Input")?.Value);
        foreach(var dial in hiddenLayers.GetChildren<Dial>()) {
            topology.Add(dial.Value);
        }
        topology.Add((int)this.Get<Dial>("Output")?.Value);
        return topology;
    }

    private void _on_Minus_pressed() {
        RemoveDial();
    }

    private void _on_Plus_pressed() {
        AddDial();
    }

    private void _on_Import_pressed() {
        this.Get<FileDialog>("LoadDialog").Show();
    }

    private void _on_LoadDialog_file_selected(string filepath) {
        if (GetParent() is DenseNetworkVisualizer visualizer) {
            visualizer.LoadWeights(filepath);
        }
    }
}
