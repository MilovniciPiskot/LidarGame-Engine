using Godot;

public partial class Movable : Node3D
{
	public void UpdateMovement(){
		//GD.Print("OBJECT MOVED - ", Name, "\tID: ", GetInstanceId());
		PointCloud.instance.ObjectMoved(GetInstanceId());
	}
}
