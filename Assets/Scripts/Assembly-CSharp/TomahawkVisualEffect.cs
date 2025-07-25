using UnityEngine;

public class TomahawkVisualEffect : CameraBlockingEffect
{
	public override void Setup(Outfit outfit)
	{
		base.transform.position = new Vector3(base.transform.position.x, outfit.HatContainer.position.y, base.transform.position.z);
	}
}
