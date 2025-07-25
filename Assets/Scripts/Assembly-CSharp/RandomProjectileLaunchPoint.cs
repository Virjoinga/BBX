using System.Collections.Generic;
using UnityEngine;

public class RandomProjectileLaunchPoint : LaunchPoint
{
	[SerializeField]
	private List<Projectile> _projectileOptions;

	protected override Projectile GetProjectilePrefab()
	{
		return _projectileOptions.Random();
	}
}
