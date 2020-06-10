using Godot;
using System.Collections.Generic;

public class Log : KinematicBody
{
    private Dictionary<Spatial, ImmediateGeometry> connections = new Dictionary<Spatial, ImmediateGeometry>();

    private Spatial debug;
    private Area detector;

    private readonly float maxX = 7;
    private readonly float minX = -7;

    private DeepQLearning root;

    [Export]
    public int Health {get; set;} = 5;

    public override void _Ready()
    {
        root = this.Get<DeepQLearning>("/root/DeepQLearning");
        debug = this.Get<Spatial>("Debug");
        detector = this.Get<Area>("Detector");
    }

    public void Move(Vector3 direction) {
        var newPos = direction + GlobalTransform.origin; 
        if (newPos.x < maxX && newPos.x > minX) {
            MoveAndSlideWithSnap(direction, Vector3.Up);
        }
    }

    public void TakeDamage() {
        Health--;
        root.UpdateHealth(Health);
    }

    public override void _PhysicsProcess(float delta) {
        foreach (var connection in connections) {
            DrawLine(connection.Value, detector.GlobalTransform.origin - this.GlobalTransform.origin, connection.Key.GlobalTransform.origin - this.GlobalTransform.origin);
        }
    }

    private void DrawLine(ImmediateGeometry geom, Vector3 start, Vector3 end) {
        geom.Clear();
		geom.Begin(Mesh.PrimitiveType.Lines);
		geom.AddVertex(start);
        geom.AddVertex(end);
		geom.End();
    }

    private void VisualizeConnection(Spatial target) {
        var mat = new SpatialMaterial() {AlbedoColor = new Color(0, 0, 0)};
        var geom = new ImmediateGeometry() {Name = this.Name, MaterialOverride = mat};
        debug.AddChild(geom);
        DrawLine(geom, detector.GlobalTransform.origin - this.GlobalTransform.origin, target.GlobalTransform.origin - this.GlobalTransform.origin);
        connections.Add(target, geom);
    }

    private void _on_Detector_body_entered(Node node) {
        if (node is Rock rock) {
            VisualizeConnection(rock);
        }
    }

    private void _on_Detector_body_exited(Node node) {
        if (node is Rock rock && connections.ContainsKey(rock)) {
            var geom = connections[rock];
            debug.RemoveChild(geom);
            connections.Remove(rock);
        }
    }
}
