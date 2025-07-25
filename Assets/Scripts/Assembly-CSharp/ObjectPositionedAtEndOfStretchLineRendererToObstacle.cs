using UnityEngine;

public class ObjectPositionedAtEndOfStretchLineRendererToObstacle : StretchLineRendererToObstacle
{
	[SerializeField]
	private Transform _object;

	protected override void OnStretchDetermined(Vector3 hitPoint)
	{
		base.OnStretchDetermined(hitPoint);
		_object.position = hitPoint;
		_object.LookAt(base.transform);
	}
}
