using Godot;
using System;

public class Spawner : Spatial
{
    [Export]
    public float SpawnRate {get; set;}

    [Export]
    public PackedScene Spawn {get; set;}

    [Export]
    public int MaxX {get; set;}

    [Export]
    public int MinX {get; set;}

    private Spatial _instanced;

    public override void _Ready()
    {
        _instanced = Spawn?.Instance() as Spatial;
        this.Get<Timer>("Timer")?.Start(SpawnRate);
    }

    private void _on_Timer_timeout() {
        if (_instanced?.Duplicate() is Spatial duplicate) {
            var randX = GD.RandRange(MinX, MaxX);
            GetParentOrNull<Node>()?.AddChild(duplicate);
            duplicate.GlobalTransform = new Transform(duplicate.Transform.basis,  GlobalTransform.origin + (Vector3.Right * (float)randX));
        }
    }
}
