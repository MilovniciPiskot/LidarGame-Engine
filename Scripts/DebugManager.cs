using Godot;
using System;

public partial class DebugManager : Node3D
{
	private WorldEnvironment enviro; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enviro = GetNode<WorldEnvironment>("../WorldEnvironment");
		ToggleLight();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("(D) toggle light")){
			ToggleLight();
		}

		string fps = Engine.GetFramesPerSecond().ToString();
		string particleCount = PointCloud.instance.particleCount.ToString();
		GetWindow().Title = "LidarGame FPS: " + fps + " | PC: " + particleCount; 
	}

	void ToggleLight(){
		if (enviro.Environment.AmbientLightSource == Godot.Environment.AmbientSource.Disabled)
			enviro.Environment.AmbientLightSource = Godot.Environment.AmbientSource.Sky;
		else
			enviro.Environment.AmbientLightSource = Godot.Environment.AmbientSource.Disabled;
	}
}
