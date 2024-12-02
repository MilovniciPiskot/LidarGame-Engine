using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float mouseSensitivity = 0.3f;
	public Node3D camera;

	private Vector2 cameraInput = Vector2.Zero;
	private float cameraAngle = 0f;
    public override void _Ready()
    {
        camera = GetNode<Node3D>("Camera3D");
    }

    public override void _PhysicsProcess(double delta)
	{
		RotateView();
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
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
		
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion){
			cameraInput = mouseMotion.Relative;
		}
    }

	void RotateView(){
		const float MaxCameraAngle = 90;
		const float MinCameraAngle = -90;

		RotateY(Mathf.DegToRad(-cameraInput.X * mouseSensitivity));

		var change = -cameraInput.Y * mouseSensitivity;

		if (change + cameraAngle < MaxCameraAngle && change + cameraAngle > MinCameraAngle) {
			camera.RotateX(Mathf.DegToRad(change));

			cameraAngle += change;
		}

		cameraInput = Vector2.Zero;
	}
}
