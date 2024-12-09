using Godot;
using Godot.Collections;

public partial class StationaryLight : LightCloud
{
	[Export] ArrayMesh icoSphere;
	private int animationCounter = 0;
	int size;
	Array verts;
	public override void _Ready()
	{
		base._Ready();
		size = icoSphere.SurfaceGetArrayLen(0);
		verts = icoSphere.SurfaceGetArrays(0);
	}

    public override void _PhysicsProcess(double delta)
    {
        if (isActive){
			// Shoot animated light
			ShootAnimatedIcoSphere();
		}
    }

	//TODO: Soooo slow and inefficently shooting rays
	void ShootSphere(){
		float stepSize = Mathf.Tau / Mathf.Sqrt(maxPoints);
		for(float i = 0; i <= Mathf.Tau; i += stepSize){
			for(float j = 0; j < Mathf.Tau; j += stepSize){
				// Cast the rays
				ShootRay(i, j);
			}
		}
	}

	static Vector2 GetSphereCoords(Vector3 orthoCoords){
		Vector2 angles;
		float xx = Mathf.Pow(orthoCoords.X, 2);
		float yy = Mathf.Pow(orthoCoords.Y, 2);
		float zz = Mathf.Pow(orthoCoords.Z, 2);

		float xyz = Mathf.Sqrt(xx + yy + zz);
		float xy = Mathf.Sqrt(xx + yy);

		angles.X = Mathf.Acos(orthoCoords.Z / xyz);
		angles.Y = Mathf.Sign(orthoCoords.Y) * Mathf.Acos(orthoCoords.X / xy);
		return angles;
	}

	//TODO: Make a ico sphere point casting
	Quaternion GetIcoSphereAngles(int index){
		
		//GD.Print(((Array)verts[0]).Count);
		
		Vector3 vertex = (Vector3)((Array)verts[0])[index%size];
		Quaternion quat = new Quaternion(Vector3.Up, vertex.Normalized());

		return quat.Normalized();
	}


	//TODO: Make the light randomly choose 50 or so points from the ico sphere and update their positions
	void ShootAnimatedIcoSphere(){
		for(int i = 0; i < scanCount; i++, animationCounter++){
			Quaternion quat = GetIcoSphereAngles(animationCounter);
			ShootRayQuat(quat);
		}
	}

}
