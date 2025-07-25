using UnityEngine;

public class RandomRotateObjectOnEnable : RotateObject
{
	[SerializeField]
	private Vector2 _spinSpeedRange = Vector2.zero;

	protected override void Initialize()
	{
		base.transform.rotation = Quaternion.Euler(Random.onUnitSphere * 360f);
		_spinSpeed = Random.Range(_spinSpeedRange.x, _spinSpeedRange.y);
		_rotationDirection = GetRandomAxis();
		base.Initialize();
	}

	private Vector3 GetRandomAxis()
	{
		switch (Random.Range(0, 4))
		{
		case 0:
			return Vector3.right;
		case 1:
			return Vector3.left;
		case 2:
			return Vector3.up;
		default:
			return Vector3.down;
		}
	}
}
