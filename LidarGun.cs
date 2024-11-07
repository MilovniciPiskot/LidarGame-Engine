using Godot;

public partial class LidarGun : Node3D
{
	public float range = 500.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		//TODO: Make whole screen scan
		if (Input.IsActionPressed("use_scanner")){
			//ShootRay(-GlobalTransform.Basis.Column2.Normalized());
			//ShootCircularPatter(Mathf.DegToRad(14), 50, PointCloud.instance.instanceCount * 0.15f);
			ShootRandomCircle(Mathf.DegToRad(15), 30);
		}
	}

	//TODO: Create the scanning pattern
	void ShootRay(Vector3 ray){
		//GD.Print("Shooting ray!");

		var spaceState = GetWorld3D().DirectSpaceState;
		// use global coordinates, not local to node
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition + ray*range);
		//query.Exclude = new Godot.Collections.Array<Rid> { GetNode<CharacterBody3D>("../../").GetRid() };
		
		var result = spaceState.IntersectRay(query);

		if (result.Count > 0){
			Node3D collider = result["collider"].As<Node3D>();
			Color color = Colors.Red;
			if (collider.HasMeta("pointColor"))
				color = collider.GetMeta("pointColor").As<Color>();
			PointCloud.instance.AddPoint((Vector3)result["position"], color);
		}
	}

	void ShootCircularPatter(float spread, int count, float offset=0){
		const float tau = Mathf.Pi * 2;
		float stepSize = tau / count;

		for(int i = 0; i < count; i++){
			ShootRay(GetRayFromAngles(stepSize*i + offset, spread));
		}
	}

	void ShootRandomCircle(float spread, int count){
		RandomNumberGenerator random = new RandomNumberGenerator();
		for(int i = 0; i < count; i++){
			ShootRay(GetRayFromAngles(Mathf.Pi * 2 * random.Randf(), spread * random.Randf()));
		}
	}

	// Alpha: angle around the axis <0; 360> deg
	// Beta : angle from the axis
	Vector3 GetRayFromAngles(float alpha, float beta){
		Vector3 forward = -GlobalTransform.Basis.Column2.Normalized();
		Vector3 spreadDir = forward.Rotated(GlobalTransform.Basis.Column0.Normalized(), beta);
		return spreadDir.Rotated(forward, alpha);
	}

	Quaternion AlignToCameraQuaterion(){
		// Rotate to the camera forward
		Vector3 forward = GlobalTransform.Basis.Column2.Normalized();
		
		Quaternion q = new Quaternion(Vector3.Forward, forward);
		//GD.Print("Point: ", point, " Forward: ", forward, " New: ", q.Normalized()*point, " Quat: ", q);

		return q.Normalized();
	}
}
