using Godot;
using System;

public partial class LightSwitch : Node3D, IInteractable
{
	[Export] LightCloud lightCloud = null;

    public void Interact()
    {
        if (lightCloud == null){
			GD.PrintErr("(LIGHT SWITCH) lightCloud has not been assigned to ", Name);	
			return;
		}

		if (lightCloud.isActive)
			lightCloud.DisableLight();
		else
			lightCloud.EnableLight();
    }
}
