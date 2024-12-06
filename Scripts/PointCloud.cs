using System.Security.Principal;
using Godot;
using Godot.Collections;
public partial class PointCloud : Node3D
{
	public static PointCloud instance;
	[Export] public uint particleCount = 0;
	const int MAX_SIZE = 100_000;
	private Dictionary<ulong, Array<ulong>> clouds = new Dictionary<ulong, Array<ulong>>();

	public enum ColorEnum
	{
		WHITE, RED, BLUE, GREEN
	}

	private Transform3D zeroTransform = Transform3D.Identity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		if (instance == null)
			instance = this;
	}

    public void AddPoint(Vector3 position, ColorEnum colorEnum, ulong id = 0)
    {
		particleCount++;

		// I hope the id will never default to 0 so i can use it as OFF state
		//BUG
		if (id > 0 && id < 100)
			GD.PrintErr("POINT CLOUD ID WAS LESS THAN 100!!");
		MultiMesh cloud = GetCloud(colorEnum, id);

		// Check for particle max limit
		if (cloud.VisibleInstanceCount >= MAX_SIZE){
			// Add new cloud to the list
			AddCloud(colorEnum, id);
			cloud = GetCloud(colorEnum, id);
		}
		
		// If the cloud is being deleted than ignore adding points;
		if (cloud == null)
			return;

		int count = cloud.VisibleInstanceCount++;
		cloud.SetInstanceTransform(count, new Transform3D(Basis.Identity, position));
	}

	ulong CreateMultiMesh(Color color){
		MultiMeshInstance3D instance = new MultiMeshInstance3D();
		MultiMesh multiMesh = new MultiMesh();
		
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		multiMesh.InstanceCount = MAX_SIZE;
		multiMesh.VisibleInstanceCount = 0;

		var pointMesh = new PointMesh();
		pointMesh.Material = CreatePointMaterial(color);
		multiMesh.Mesh = pointMesh;

		instance.Multimesh = multiMesh;
		instance.GIMode = GeometryInstance3D.GIModeEnum.Disabled;
		AddChild(instance);
		return instance.GetInstanceId();
	}

	Material CreatePointMaterial(Color color){
		StandardMaterial3D material = new StandardMaterial3D();

		material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		material.DisableReceiveShadows = true;
		material.DisableAmbientLight = true;
		material.NoDepthTest = true;
		material.DisableFog = true;
		material.AlbedoColor = color;
		material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.Disabled;
		material.SpecularMode = BaseMaterial3D.SpecularModeEnum.Disabled;
		material.BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles;
		material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		material.FixedSize = true;
		material.UsePointSize = true;
		material.PointSize = 2.5f;

		return material;
	}

	static Color GetColorFromEnum(ColorEnum pointColorEnum){
		switch (pointColorEnum)
		{
			case ColorEnum.RED:
				return Colors.Red;

			case ColorEnum.BLUE:
				return Colors.DarkCyan;
			
			case ColorEnum.GREEN:
				return Colors.Green;
			
			default:
				return Colors.White;
		}
	}

	MultiMesh GetCloud(ColorEnum color, ulong id){
		ulong key = id == 0 ? (ulong)color : id;

		if (!clouds.ContainsKey(key))
			clouds[key] = new Array<ulong>();

		if (clouds[key].Count == 0)
			AddCloud(color, id);

		// Return the last one
		ulong last = clouds[key][clouds[key].Count-1];

		MultiMeshInstance3D instance = (MultiMeshInstance3D)InstanceFromId(last);
		if (instance.IsQueuedForDeletion()){
			GD.Print("A");
			return null;
		}

		return instance.Multimesh;
	}

	void AddCloud(ColorEnum color, ulong id){
		ulong key = id == 0 ? (ulong)color : id;

		var cloud = CreateMultiMesh(GetColorFromEnum(color));

		if (!clouds.ContainsKey(key))
			clouds[key] = new Array<ulong>();

		clouds[key].Add(cloud);
	}

	public void ObjectMoved(ulong id){
		if (!clouds.ContainsKey(id))
			return;

		// Destroy all points for this object
		foreach (ulong cloud in clouds[id]){
			((Node3D)InstanceFromId(cloud)).QueueFree();
			clouds[id].Remove(cloud);
		}
	}
}
