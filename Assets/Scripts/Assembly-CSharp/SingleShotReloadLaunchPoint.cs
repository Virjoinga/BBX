using System;
using System.Collections;
using UnityEngine;

public class SingleShotReloadLaunchPoint : LaunchPoint
{
	[SerializeField]
	private GameObject _projectileModel;

	[SerializeField]
	private float _fireDelay = 0.25f;

	[SerializeField]
	private float _projectileSpawnDelay = 1.5f;

	protected override void OnEnable()
	{
		_projectileModel.SetActive(value: true);
		base.OnEnable();
	}

	protected override void FireInternal(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		StartCoroutine(FireAfterDelay(forward, fireDelay, projectileId, serverFrame, onHit, isMock));
	}

	private IEnumerator FireAfterDelay(Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		yield return new WaitForSeconds(_fireDelay);
		_projectileModel.SetActive(value: false);
		FireProjectile(base.transform.position, ref forward, fireDelay, projectileId, serverFrame, onHit, isMock);
		StartCoroutine(SpawnInNewProjectile());
	}

	protected IEnumerator SpawnInNewProjectile()
	{
		yield return new WaitForSeconds(_projectileSpawnDelay);
		_projectileModel.SetActive(value: true);
	}
}
