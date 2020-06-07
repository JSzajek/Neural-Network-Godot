using Godot;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Setup window control representing a user
/// control to set up the layout of the network
/// </summary>
public class SetupWindow : WindowDialog
{
    private Dial dial;
    private HBoxContainer hiddenLayers;
    private Button minus;

    /// <summary>
    /// Initializes the parameters
    /// </summary>
    public override void _Ready()
    {
        dial = (GD.Load("res://_Assets/Dial.tscn") as PackedScene)?.Instance() as Dial;
        hiddenLayers = GetNode<HBoxContainer>("HiddenLayers/ScrollContainer/HBoxContainer");
        minus = GetNode<Button>("HiddenLayers/Minus");
        AddDial();
    }

    /// <summary>
    /// Adds a dial to the hidden layers
    /// </summary>
    private void AddDial() {
        var toAdd = dial.Duplicate() as Dial;
        toAdd.MinValue = 1;
        hiddenLayers.AddChild(toAdd);
        if (minus.Disabled) {
            minus.Disabled = false;
        }
    }

    /// <summary>
    /// Removes the most recent dial from the hidden layers
    /// </summary>
    private void RemoveDial() {
        var dials = hiddenLayers.GetChildren<Dial>();
        var toRemove = dials.LastOrDefault();
        dials.Remove(toRemove);
        toRemove.QueueFree();

        if (dials.Count == 0) {
            minus.Disabled = true;
        }
    }

    /// <summary>
    /// Create button pressed signal catch - triggers the creation of the network
    /// </summary>
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

    /// <summary>
    /// Helper method that retrieves the topology from the 
    /// current dials.
    /// </summary>
    /// <returns>The network topology</returns>
    private List<int> GetTopology() {
        var topology = new List<int>();
        topology.Add((int)this.Get<Dial>("Input")?.Value);
        foreach(var dial in hiddenLayers.GetChildren<Dial>()) {
            topology.Add(dial.Value);
        }
        topology.Add((int)this.Get<Dial>("Output")?.Value);
        return topology;
    }

    /// <summary>
    /// Minus button pressed signal catch
    /// </summary>
    private void _on_Minus_pressed() {
        RemoveDial();
    }

    /// <summary>
    /// Plus button pressed signal catch
    /// </summary>
    private void _on_Plus_pressed() {
        AddDial();
    }

    /// <summary>
    /// Import button pressed signal catch
    /// </summary>
    private void _on_Import_pressed() {
        this.Get<FileDialog>("LoadDialog").Show();
    }

    /// <summary>
    /// Load Dialog file selected signal catch - triggers network import
    /// </summary>
    /// <param name="filepath"></param>
    private void _on_LoadDialog_file_selected(string filepath) {
        if (GetParent() is DenseNetworkVisualizer visualizer) {
            visualizer.LoadWeights(filepath);
        }
    }
}
