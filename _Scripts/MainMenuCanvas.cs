using Godot;

public class MainMenuCanvas : Control
{
    private PackedScene denseNetworkScene;

    public override void _Ready()
    {  
        denseNetworkScene = ResourceLoader.Load<PackedScene>("res://_Scenes/DenseNetworkVisualizer.tscn");
    }

    public void _on_Dense_Network_pressed() {
        GetTree().ChangeSceneTo(denseNetworkScene);
    }
}
