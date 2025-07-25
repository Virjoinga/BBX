using UnityEngine;

public interface IMovable
{
	Vector3 Velocity { get; }

	void ForceMove(Vector3 force, bool breaksMelee);
}
