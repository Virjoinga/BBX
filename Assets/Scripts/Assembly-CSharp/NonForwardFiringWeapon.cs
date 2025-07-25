using UnityEngine;

public class NonForwardFiringWeapon : FiringWeapon
{
	private enum Direction
	{
		Upward = 0,
		Downward = 1,
		Left = 2,
		Right = 3
	}

	[SerializeField]
	private Direction _direction = Direction.Downward;

	public override int LaunchPointFire(Vector3 position, ref Vector3 forward, int serverFrame, int shotCount, bool isMock = false)
	{
		forward = GetDirection();
		return base.LaunchPointFire(position, ref forward, serverFrame, shotCount, isMock);
	}

	private Vector3 GetDirection()
	{
		switch (_direction)
		{
		case Direction.Left:
			return Vector3.Cross(Vector3.up, base.transform.forward);
		case Direction.Right:
			return -Vector3.Cross(Vector3.up, base.transform.forward);
		case Direction.Upward:
			return Vector3.up;
		default:
			return Vector3.down;
		}
	}
}
