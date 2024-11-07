using Godot;

public partial class PointCloud : MultiMeshInstance3D
{
    public static PointCloud instance;
    public int instanceCount = 0;
    int maxInstanceCount = 2_000_000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {

        if (instance == null)
            instance = this;
    
        Multimesh.InstanceCount = 0;
        // Set the format first.
        Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        Multimesh.UseColors = true;

        // Maybe set the scale to 0 for 1000 of them and then just set the scale to 1 when they are supposed to draw
        Multimesh.InstanceCount = maxInstanceCount;
        for (int i  = 0; i < maxInstanceCount; i++){
            // Init all points to scale 0;
            Basis zeroBasis = Basis.FromScale(Vector3.Zero);
            //Multimesh.SetInstanceColor(i, Color.Color8(0, 0, 0));
            Multimesh.SetInstanceTransform(i, new Transform3D(zeroBasis, Vector3.Zero));
        }
        //Multimesh.VisibleinstanceCount = 1000;
    }

    public override void _Process(double delta)
    {
        string title = "LidarGame | PC: " + instanceCount.ToString() + " | FPS: " + Engine.GetFramesPerSecond().ToString();
        DisplayServer.WindowSetTitle(title);
    }

    //TODO: Make a system to accept different colors
    public void AddPoint(Vector3 position, Color color){
        if (instanceCount >= maxInstanceCount)
            GD.PrintErr("POINT CLOUD OVERFLOW");

        instanceCount++;
        Multimesh.VisibleInstanceCount = instanceCount;
        Multimesh.SetInstanceColor(instanceCount-1, color);
        Multimesh.SetInstanceTransform(instanceCount-1, new Transform3D(Basis.Identity, position));
        //GD.Print("Point clound instance: ", instanceCount.ToString());
    }
}
