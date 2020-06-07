using Godot;

/// <summary>
/// Main menu control script
/// </summary>
public class MainMenuCanvas : Control
{
    private PackedScene denseNetworkScene;

    /// <summary>
    /// Initializes the parameters
    /// </summary>
    public override void _Ready()
    {  
        denseNetworkScene = ResourceLoader.Load<PackedScene>("res://_Scenes/DenseNetworkVisualizer.tscn");
    }

    /// <summary>
    /// Dense Network button pressed signal catch - switches scene to dense network example
    /// </summary>
    public void _on_Dense_Network_pressed() {
        GetTree().ChangeSceneTo(denseNetworkScene);
    }
}
