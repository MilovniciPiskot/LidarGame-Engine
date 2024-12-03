using Godot;
using System;

public partial class Scanner : Node3D 
{
	private CharacterBody3D characterBody;
	const float RAY_LENGTH = 50000;


    public override void _Ready()
    {
        characterBody = GetNode("../../../CharacterBody3D") as CharacterBody3D;
    }
    public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("scan")){
			//PointCloudManager.instance.AddPoint(GlobalPosition, PointCloudManager.PointColorEnum.RED);
			ShootRandomCircle(Mathf.DegToRad(20), 45);
		}
	}

	public void ShootRandomCircle(float spread, float count){
		for(int i = 0; i < count; i++){
			var random = new RandomNumberGenerator();
			float radius = spread * Mathf.Sqrt(random.Randf());
			float theta = random.Randf() * Mathf.Tau;

			ShootRay(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius);
		}
	}

	public void ShootRay(float horAngle, float vertAngle){
		Vector3 direction = -GlobalBasis.Column1;
		direction = direction.Rotated(GlobalBasis.Column2, horAngle);
		direction = direction.Rotated(GlobalBasis.Column0, vertAngle);

		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition + direction * RAY_LENGTH,
		characterBody.CollisionMask, new Godot.Collections.Array<Rid> { characterBody.GetRid() });
    	var result = spaceState.IntersectRay(query);

		// No valid  hits
		if (result.Count <= 0)
			return;

		PointCloudManager.PointColorEnum color = vertAngle == 0 ? PointCloudManager.PointColorEnum.WHITE : PointCloudManager.PointColorEnum.GREEN;
		PointCloudManager.instance.AddPoint((Vector3)result["position"], color);
	}
}
