using System.Security.Principal;
using Godot;
using Godot.Collections;
public partial class PointCloudManager : Node3D
{
	public static PointCloudManager instance;
	[Export] public uint particleCount = 0;
	const int MAX_SIZE = 100_000;
	private Dictionary<Color, Array<MultiMesh>> clouds = new Dictionary<Color, Array<MultiMesh>>();

	public enum PointColorEnum
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

	public void AddPoint(Vector3 position, PointColorEnum colorEnum = PointColorEnum.WHITE){
		particleCount++;
		
		Color color = GetColorFromEnum(colorEnum);
		MultiMesh cloud = GetCloud(color);

		// Check for particle max limit
		if (cloud.VisibleInstanceCount >= MAX_SIZE){
			// Add new cloud to the list
			cloud = AddCloud(color);
		}
		
		int count = cloud.VisibleInstanceCount++;
		cloud.SetInstanceTransform(count, new Transform3D(Basis.Identity, position));
	}

	MultiMesh CreateMultiMesh(Color color){
		MultiMeshInstance3D instance = new MultiMeshInstance3D();
		MultiMesh multiMesh = new MultiMesh();
		
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		multiMesh.InstanceCount = MAX_SIZE;
		multiMesh.VisibleInstanceCount = 0;

		var pointMesh = new PointMesh();
		pointMesh.Material = CreatePointMaterial(color);
		multiMesh.Mesh = pointMesh;



		// Probably dont need this part
		// for(int i = 0; i < MAX_SIZE; i++){
		// 	multiMesh.SetInstanceTransform(i, zeroTransform);
		// }

		instance.Multimesh = multiMesh;
		instance.GIMode = GeometryInstance3D.GIModeEnum.Disabled;
		AddChild(instance);
		return multiMesh;
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

	Color GetColorFromEnum(PointColorEnum pointColorEnum){
		switch (pointColorEnum)
		{
			case PointColorEnum.RED:
				return Colors.Red;

			case PointColorEnum.BLUE:
				return Colors.Cyan;
			
			case PointColorEnum.GREEN:
				return Colors.Green;
			
			default:
				return Colors.White;
		}
	}

	MultiMesh GetCloud(Color color){
		if (!clouds.ContainsKey(color))
			clouds[color] = new Array<MultiMesh>();

		if (clouds[color].Count == 0)
			AddCloud(color);

		// Return the last one
		return clouds[color][clouds[color].Count-1];
	}

	MultiMesh AddCloud(Color color){
		var cloud = CreateMultiMesh(color);

		if (!clouds.ContainsKey(color))
			clouds[color] = new Array<MultiMesh>();

		clouds[color].Add(cloud);
		return cloud;
	}
}
