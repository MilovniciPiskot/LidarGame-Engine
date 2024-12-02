using Godot;
using System;

public partial class Scanner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("scan"))
			PointCloudManager.instance.AddPoint(GlobalPosition, PointCloudManager.PointColorEnum.RED);
	}
}
