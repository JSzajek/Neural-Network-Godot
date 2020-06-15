using Godot;
using System.Collections.Generic;
using System.Linq;

public class Log : KinematicBody
{
    private Dictionary<Spatial, ImmediateGeometry> connections = new Dictionary<Spatial, ImmediateGeometry>();

    private Spatial debug;
    private Area detector;

    private readonly float maxX = 7;
    private readonly float minX = -7;

    private float miniReward = 0;

    private DeepQLearning root;
    private Vector3 velocity = Vector3.Zero;

    [Export]
    public int Health {get; set;} = 5;

    public override void _Ready()
    {
        root = this.Get<DeepQLearning>("/root/DeepQLearning");
        debug = this.Get<Spatial>("Debug");
        detector = this.Get<Area>("Detector");
        root?.UpdateHealth(Health);
    }

    public void Move(Vector3 direction) {
        var newPos = direction + GlobalTransform.origin; 
        if (newPos.x < maxX && newPos.x > minX) {
            MoveAndSlide(direction * 40, Vector3.Up);
        }
    }

    public void TakeDamage() {
        Health--;
        miniReward -= Health == 0 ? 300 : 100; // Punish damage taking
        root?.UpdateHealth(Health);
    }

    public override void _PhysicsProcess(float delta) {
        foreach (var connection in connections) {
            DrawLine(connection.Value, detector.GlobalTransform.origin - this.GlobalTransform.origin, connection.Key.GlobalTransform.origin - this.GlobalTransform.origin);
        }
        miniReward += 0.75f; // Reward longer life time w/ out damage
    }

    public void performAction(int action) {
        switch(action) {
            case 0:
                break;
            case 1:
                Move(Vector3.Left);
                break;
            case 2:
                Move(Vector3.Right);
                break;
        }
    }

    public int[] getCurrentValue() {
        var values = new int[2] { (int)miniReward, Health } ;
        miniReward = 0;
        return values;
    }

    public List<float> getCurrentState()
    {
        var origin = GlobalTransform.origin;
        var states = new List<float>();
        states.Add(Health);
        states.Add(origin.x);

        var closest = connections.Keys.OrderBy(x => origin.DistanceTo(x.GlobalTransform.origin)).Take(5);

        foreach (var close in closest) {
            states.Add(origin.DistanceTo(close.GlobalTransform.origin));
            states.Add(origin.AngleTo(close.GlobalTransform.origin));    
        }

        while (states.Count != 12) {
            states.Add(0);
        }
        return states;
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
