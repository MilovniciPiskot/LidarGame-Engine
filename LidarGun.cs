using Godot;

public partial class LidarGun : Node3D
{
	public float range = 25.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		ScanTrails.instance.EndScan();
		//TODO: Make whole screen scan
		if (Input.IsActionPressed("use_scanner")){
			ScanTrails.instance.BeginScan();
			//ShootRay(-GetLidarForward());
			//ShootCircularPatter(Mathf.DegToRad(14), 8, 0);//PointCloud.instance.instanceCount * 0.15f);
			ShootRandomCircle(Mathf.DegToRad(15), 50);
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
		Vector3 collisionPos = GlobalPosition + ray*range;

		if (result.Count > 0){
			// Hit an object so add it to PointCloud
			Node3D collider = result["collider"].As<Node3D>();
			collisionPos = (Vector3)result["position"];

			Color color = Colors.Red;
			if (collider.HasMeta("pointColor"))
				color = collider.GetMeta("pointColor").As<Color>();

			PointCloud.instance.AddPoint(collisionPos, color);

		}
		// Draw the scan trail
		ScanTrails.instance.AddScanTrail(GlobalPosition, collisionPos, GlobalPosition);
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
			ShootRay(GetRayFromAngles(Mathf.Pi * 2 * random.Randf(), spread * Mathf.Sqrt(random.Randf())));
		}
	}

	// Alpha: angle around the axis <0; 360> deg
	// Beta : angle from the axis
	Vector3 GetRayFromAngles(float alpha, float beta){
		Vector3 forward = -GetLidarForward();
		Vector3 spreadDir = forward.Rotated(GlobalTransform.Basis.Column0.Normalized(), beta);
		return spreadDir.Rotated(forward, alpha);
	}

	Quaternion AlignToCameraQuaterion(){
		// Rotate to the camera forward
		Vector3 forward = GetLidarForward();
		
		Quaternion q = new Quaternion(Vector3.Forward, forward);
		//GD.Print("Point: ", point, " Forward: ", forward, " New: ", q.Normalized()*point, " Quat: ", q);

		return q.Normalized();
	}


	public Vector3 GetLidarForward(){
		return GlobalTransform.Basis.Column2.Normalized();
	}
}
