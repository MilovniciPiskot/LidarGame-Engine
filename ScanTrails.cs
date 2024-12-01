using Godot;

public partial class ScanTrails : MultiMeshInstance3D
{
	public static ScanTrails instance = null;
	int maxInstanceCount = 1000;
	int instanceCount = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (instance == null)
			instance = this;

		Multimesh.InstanceCount = maxInstanceCount;
		// for(int i = 0; i < 0; i++){
		// 	Multimesh.SetInstanceTransform(i, Transform3D.Identity);
		// }
	}

	public override void _Process(double delta)
	{
	}

	public void BeginScan(){
		instanceCount = 0;
		Multimesh.VisibleInstanceCount = instanceCount;
	}

	public void AddScanTrail(Vector3 start, Vector3 end, Vector3 origin){
		Vector3 localEnd = end - origin;
			
		float length = localEnd.Length();
		Quaternion q = new Quaternion(Vector3.Up, localEnd.Normalized());
		Basis basis = new Basis(q) * Basis.Identity.Scaled(new Vector3(0.3f, length, 0.3f));
		
		instanceCount++;
		Multimesh.SetInstanceTransform(instanceCount-1, new Transform3D(basis, origin));
		Multimesh.VisibleInstanceCount = instanceCount;
	}

	public void EndScan(){
		instanceCount = 0;
		Multimesh.VisibleInstanceCount = instanceCount;
	}
}
