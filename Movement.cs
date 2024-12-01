using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float mouse_sens = 0.02f;
    public float camera_anglev=0;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// // Handle Jump.
		// if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		// {
		// 	velocity.Y = JumpVelocity;
		// }

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("movement_left", "movement_right", "movement_up", "movement_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		inputDir = Input.GetVector("look_up", "look_down", "look_left", "look_right");
		Camera3D camera = GetNode<Camera3D>("Camera3D");

		// Better clamping
		if (inputDir.X != 0){
			if ((camera.Rotation.X < Mathf.Pi / 2 && inputDir.X < 0) || (camera.Rotation.X >= -Mathf.Pi / 2 && inputDir.X > 0))
				camera.RotateX(inputDir.X * -mouse_sens);
		}
		if (inputDir.Y != 0)
			RotateY(inputDir.Y * -mouse_sens);
	}
    // public override void _Input(InputEvent @event)
    // {
    //     base._Input(@event);
	// 	if (@event is InputEventMouseMotion){
	// 		Camera3D camera = GetNode<Camera3D>("Camera");

	// 		var changev= -(@event as InputEventMouseMotion).Relative.Y * mouse_sens;
	// 		if (camera_anglev + changev > -50 && camera_anglev + changev < 50){
	// 			camera_anglev += changev;
	// 			camera.RotateX(Mathf.DegToRad(changev));
	// 		} 	
	// 	}
    // }
    // var mouse_sens = 0.3;
    // var camera_anglev=0;

    // func _input(event):  		
    // 	if event is InputEventMouseMotion:
    // 		$Camera.rotate_y(deg2rad(-event.relative.x*mouse_sens))
    // 		var changev=-event.relative.y*mouse_sens
    // 		if camera_anglev+changev>-50 and camera_anglev+changev<50:
    // 			camera_anglev+=changev
    // 			$Camera.rotate_x(deg2rad(changev))
}
