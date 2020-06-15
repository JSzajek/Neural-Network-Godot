using Godot;

/// <summary>
/// Main menu control script
/// </summary>
public class MainMenuCanvas : Control
{
    private PackedScene denseNetworkScene;
    private PackedScene deepQNetworkScene;

    /// <summary>
    /// Initializes the parameters
    /// </summary>
    public override void _Ready()
    {  
        denseNetworkScene = ResourceLoader.Load<PackedScene>("res://_Scenes/DenseNetworkVisualizer.tscn");
        deepQNetworkScene = ResourceLoader.Load<PackedScene>("res://_Scenes/DeepQLearning.tscn");
    }

    /// <summary>
    /// Dense Network button pressed signal catch - switches scene to dense network example
    /// </summary>
    private void _on_Dense_Network_pressed() {
        GetTree().ChangeSceneTo(denseNetworkScene);
    }

    /// <summary>
    /// Deep Q Learning pressed signal catch - switches scene to deep q learning example
    /// </summary>
    private void _on_Deep_QLearning_pressed() {
        GetTree().ChangeSceneTo(deepQNetworkScene);
    }
}
