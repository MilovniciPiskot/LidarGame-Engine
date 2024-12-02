using Godot;
using System;

public partial class DebugManager : Node3D
{
	private DirectionalLight3D light; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		light = GetNode<DirectionalLight3D>("../DirectionalLight3D");
		ToggleLight();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("(D) toggle light")){
			ToggleLight();
		}

		string fps = Engine.GetFramesPerSecond().ToString();
		string particleCount = "0";
		GetWindow().Title = "LidarGame FPS: " + fps + " | PC: " + particleCount; 
	}

	void ToggleLight(){
		light.Visible = !light.Visible;
	}
}
