using Godot;

public class RiverWorld : Spatial
{
    private void _on_Destructor_body_entered(Node node) {
        RemoveChild(node);
    }
}
