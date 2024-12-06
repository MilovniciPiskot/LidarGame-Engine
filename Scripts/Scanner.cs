using Godot;
using Godot.Collections;
public partial class Scanner : Node3D 
{
	private CharacterBody3D characterBody;
	const float RAY_LENGTH = 50000;
	const float INTERACT_REACH = 2f;

	const float angleWidth = 100;
	const float angleHeight = 80;
	float currentScanIndex = float.MaxValue;

	public bool isFocused = false;
	[Export] public bool isScanning = false;
	[Export] private int scanSpeed = 50;
	[Export] private float scanDensity = 1.33337f;
	[Export] private int scanSpread = 20;

    public override void _Ready()
    {
        characterBody = GetNode("../../CharacterBody3D") as CharacterBody3D;	
		Input.MouseMode = Input.MouseModeEnum.Captured;
		isFocused = true;
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("Escape")){
			Input.MouseMode = Input.MouseModeEnum.Visible;
			isFocused = false;
		}

		if (@event.IsActionPressed("scanNormal")){
			if (Input.MouseMode == Input.MouseModeEnum.Visible){
				Input.MouseMode = Input.MouseModeEnum.Captured;
				isFocused = true;
			}
		}

		if(@event.IsActionPressed("Interact")){
			// Shoot interact ray
			//TODO: Show hand as a crosshair or smth
			InteractRay();
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		if (isScanning = currentScanIndex <= angleWidth * angleHeight ){
			for(int i = 0;  i < scanSpeed; i++, currentScanIndex+=1/scanDensity){
				float x =  currentScanIndex % angleWidth;
				float y =  currentScanIndex / angleWidth;
				x -= angleWidth/2;
				y -= angleHeight/2;
				//GD.Print(x, " | ", y);

				ShootRay(Mathf.DegToRad(x), Mathf.DegToRad(y));
			}
		}

		// Dont try to raycast if the window is not focused
		if (!isFocused)
			return;

		if (Input.IsActionPressed("scanNormal") && isScanning == false){
			//PointCloudManager.instance.AddPoint(GlobalPosition, PointCloudManager.PointColorEnum.RED);
			ScanCircle(Mathf.DegToRad(scanSpread));
		}

		if (Input.IsActionJustPressed("scanSpecial")){
			ScanTopdown();
		}
	}

	void InteractRay(){
		Vector3 direction = -GlobalBasis.Column2;

		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition + direction * INTERACT_REACH);
		query.Exclude = new Array<Rid> { characterBody.GetRid() };
    	var result = spaceState.IntersectRay(query);

		// No valid  hits
		if (result.Count <= 0)
			return;

		Node parent = ((Node)result["collider"]).GetParent();
		if (parent is IInteractable interactable){
			interactable.Interact();
		}
	}

	public void ScanCircle(float spread){
		for(int i = 0; i < scanSpeed/2; i++){
			var random = new RandomNumberGenerator();
			float radius = spread * Mathf.Sqrt(random.Randf());
			float theta = random.Randf() * Mathf.Tau;

			ShootRay(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius);
		}
	}

	public void ScanTopdown(){
		if (isScanning == false){
			isScanning = true;
			currentScanIndex = 0;
		}
	}

	public void ShootRay(float horAngle, float vertAngle){
		Vector3 direction = -GlobalBasis.Column2;
		direction = direction.Rotated(GlobalBasis.Column1, horAngle);
		direction = direction.Rotated(GlobalBasis.Column0, vertAngle);

		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition + direction * RAY_LENGTH);
		query.Exclude = new Array<Rid> { characterBody.GetRid() };
    	var result = spaceState.IntersectRay(query);

		// No valid  hits
		if (result.Count <= 0)
			return;

		//Variant meta = result["metadata"];
		Node parent = ((Node)result["collider"]).GetParent();
		PointCloud.ColorEnum color = PointCloud.ColorEnum.WHITE;

		if(parent.HasMeta("SurfaceType")){
			color = (PointCloud.ColorEnum)(int)parent.GetMeta("SurfaceType");
		}

		ulong key = 0;
		if(parent is Movable){
			key = parent.GetInstanceId();
		}

		PointCloud.instance.AddPoint((Vector3)result["position"], color, key);
	}
}
