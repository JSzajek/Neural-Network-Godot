using Godot;
using System;

public class Rock : KinematicBody
{
    [Export]
    public float MoveSpeed {get; set;}

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta) {
        MoveAndSlide(Vector3.Forward * MoveSpeed, Vector3.Up);
    }

    private void _on_Detector_body_entered(Node node) {
        if (node is Log log) {
            log.TakeDamage();
            QueueFree();
        }
    } 
}
