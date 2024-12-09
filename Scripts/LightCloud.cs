using Godot;

public partial class LightCloud : Node3D
{
	[Export] float RAY_LENGTH = 10;
	[Export] public uint maxPoints = 500;
	[Export] public int scanCount = 50;
	[Export] public Color lightColor = Colors.Gold;
	[Export] public Node3D lightEmmiterObject = null;
	[Export] bool isLingering = true;
	[Export] public bool isActive = false;
	[Export] public float pointSize = 3f;
	private uint particleCount = 0;
	public MultiMesh multiMesh = null;

	public override void _Ready()
    {
        CreateMultiMesh();
		if (lightEmmiterObject == null){
			GD.PrintErr("LightCloud: ", Name, " has no lightEmmiterObject asssigned!!");
			QueueFree();
		}
    }
	public void CreateMultiMesh(){
		MultiMeshInstance3D instance = new MultiMeshInstance3D();
		multiMesh = new MultiMesh();
		
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		multiMesh.InstanceCount = (int)maxPoints;
		multiMesh.VisibleInstanceCount = 0;

		var pointMesh = new PointMesh();
		pointMesh.Material = PointCloud.instance.CreatePointMaterial(lightColor, pointSize);
		multiMesh.Mesh = pointMesh;

		instance.Multimesh = multiMesh;
		instance.GIMode = GeometryInstance3D.GIModeEnum.Disabled;
		AddChild(instance);
		instance.GlobalPosition = Vector3.Zero;
		instance.GlobalRotation = Vector3.Zero;

		PointCloud.instance.particleCount += maxPoints;
	}

	public void AddPoint(Vector3 position)
    {
		particleCount++;
		RandomNumberGenerator random = new RandomNumberGenerator();
        int lingering = (int)(random.Randi() % (isLingering ? 2 : 1));
		
		// If the cloud is being deleted then ignore adding points;
		if (multiMesh == null){
			GD.PrintErr("No multimesh initiated");
			return;
		}

		multiMesh.VisibleInstanceCount = (int)(particleCount > maxPoints ? maxPoints : particleCount);
		multiMesh.SetInstanceTransform((int)((particleCount + lingering) % maxPoints), new Transform3D(Basis.Identity, position));
	}

	//TODO: refactor this
	public void ShootRay(float horAngle, float vertAngle){
		Vector3 direction = -lightEmmiterObject.GlobalBasis.Column2;
		direction = direction.Rotated(lightEmmiterObject.GlobalBasis.Column1.Normalized(), horAngle);
		direction = direction.Rotated(lightEmmiterObject.GlobalBasis.Column0.Normalized(), vertAngle);

		var spaceState = Scanner.instance.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(lightEmmiterObject.GlobalPosition, lightEmmiterObject.GlobalPosition + direction * RAY_LENGTH);
		// query.HitFromInside = true;
		// query.HitBackFaces = true;

		//query.Exclude = new Godot.Collections.Array<Rid> { };//Scanner.instance.characterBody.GetRid() };
    	var result = spaceState.IntersectRay(query);

		// No valid  hits
		if (result.Count <= 0)
			return;

		// if ((Node3D)result["collider"] is not StaticBody3D)
		// 	return;

		AddPoint((Vector3)result["position"]);
	} 

	public void ShootRayQuat(Quaternion quat){
		Vector3 direction = quat.Inverse() * -lightEmmiterObject.GlobalBasis.Column2 * quat;

		var spaceState = Scanner.instance.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(lightEmmiterObject.GlobalPosition, lightEmmiterObject.GlobalPosition + direction * RAY_LENGTH);

		//query.Exclude = new Godot.Collections.Array<Rid> { };//Scanner.instance.characterBody.GetRid() };
    	var result = spaceState.IntersectRay(query);

		// No valid  hits
		if (result.Count <= 0)
			return;

		// if ((Node3D)result["collider"] is not StaticBody3D)
		// 	return;

		AddPoint((Vector3)result["position"]);
	}

	public void EnableLight(){
		isActive = true;
	}

	public void DisableLight(){
		isActive = false;
		particleCount = 0;
		multiMesh.VisibleInstanceCount = 0;
	}
}
