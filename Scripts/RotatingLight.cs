using System;
using Godot;

public partial class RotatingLight : LightCloud
{
	[Export] int scanCount = 50;
	[Export] float spread = 35;
	[Export] float rotationSpeed = 40;
    public override void _Ready()
    {
        base._Ready();
    }
    public override void _PhysicsProcess(double delta)
	{
		if (isActive){
			ScanVerticalLine();
			lightEmmiterObject.RotateY((float)Mathf.DegToRad(rotationSpeed * delta));
		}
	}

	public void ScanVerticalLine(){
		RandomNumberGenerator random = new RandomNumberGenerator();
		for(int i = 0; i < scanCount; i++){
			ShootRay(Mathf.DegToRad(random.Randf() * rotationSpeed / 10), (random.Randf() - 0.5f) * Mathf.DegToRad(spread) * 2);
		}
	}
}
