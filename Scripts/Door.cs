using Godot;
using System;

public partial class Door : Movable, IInteractable
{
	[Export] public Node3D hinge;
	[Export] public float maxRotation = 90f;
	[Export] public float minRotation = -90f;
	[Export] public float closedRotation = 0f;
	[Export] public float speed = 25f;
	[Export] public bool isInteractable = true;

	public enum StateEnum {OPENED_INSIDE, CLOSED, OPENED_OUTSIDE};
	[Export] public StateEnum state = StateEnum.CLOSED;
	[Export] private float rotation = 0f;

	
	public void Open(){
		//TODO: Make doors open both ways
		if (isInteractable){
			isInteractable = false;
			state = StateEnum.OPENED_INSIDE;
		}
	}

	public void Close(){
		if (isInteractable){
			isInteractable = false;
			state = StateEnum.CLOSED;
		}
	}

	public void ToggleState(){
		if (state == StateEnum.CLOSED)
			Open();
		else
			Close();
	}

    public override void _PhysicsProcess(double delta)	
    {
		if (!isInteractable) {
			// Animate
			int direction = state == StateEnum.CLOSED ? Mathf.Sign(closedRotation - rotation) : (int)state - 1;
			float change = speed * direction * (float)delta;

			rotation = Mathf.Clamp(rotation + change, minRotation, maxRotation);
			if (rotation == minRotation || rotation == maxRotation || 
				(state == StateEnum.CLOSED && Mathf.Abs(rotation - closedRotation) <= 1f))
				isInteractable = true;
			
			// Set the rotation
			RotateY(Mathf.DegToRad(change));
			UpdateMovementWithEffect();
		}
    }

    public void Interact()
    {
        ToggleState();
    }
}
