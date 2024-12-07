using Godot;

public partial class Movable : Node3D
{
	const int maxEffectOffset = 6;
	uint effectOffset = 0;

	public void UpdateMovement(){
		//GD.Print("OBJECT MOVED - ", Name, "\tID: ", GetInstanceId());
		PointCloud.instance.ObjectMoved(GetInstanceId());
	}

	public void UpdateMovementWithEffect(){
		if (effectOffset++ % maxEffectOffset == 0){
			UpdateMovement();
		}
	}
}
