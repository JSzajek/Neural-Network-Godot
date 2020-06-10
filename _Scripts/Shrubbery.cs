using Godot;
using System;

public class Shrubbery : KinematicBody
{
    [Export]
    public float MoveSpeed {get; set;}

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta) {
        MoveAndSlide(Vector3.Forward * MoveSpeed, Vector3.Up);
    }
}
