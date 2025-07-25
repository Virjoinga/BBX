using System;
using System.Collections;
using UnityEngine;

public class BombardmentProjectile : Projectile
{
	[SerializeField]
	private BombardmentAimIndicator _aimIndicator;

	[SerializeField]
	private float _crestheight = 50f;

	private bool _ascending = true;

	private bool _landed;

	private Vector3 _aimTarget;

	private float _travelTime;

	private float _halfTravelTime;

	private Vector3 _ascendCrestPosition;

	private Vector3 _decendCrestPosition;

	private Vector3 _startPosition;

	protected override void Start()
	{
		base.Start();
		if (_aimIndicator != null)
		{
			_aimIndicator.transform.SetParent(null);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (_aimIndicator != null)
		{
			_aimIndicator.gameObject.SetActive(value: false);
		}
	}

	public override void Launch(LaunchDetails details, Action<HitInfo> onHit, bool isMock = false)
	{
		base.Launch(details, onHit, isMock);
		_ascending = true;
		_landed = false;
		_aimTarget = details.aimTarget;
		_travelTime = details.weaponProfile.Projectile.TravelTime;
		_halfTravelTime = _travelTime * 0.5f;
		_ascendCrestPosition = base.transform.position;
		_ascendCrestPosition.y = _crestheight;
		_decendCrestPosition = _aimTarget;
		_decendCrestPosition.y = _crestheight;
		_startPosition = base.transform.position;
		if (details.playerController.IsLocal)
		{
			DisplayAimIndicator(details.playerController);
		}
		else if (details.playerController.IsLocalPlayerTeammate)
		{
			StartCoroutine(DelayedDisplayAimIndicator(details.playerController));
		}
	}

	private void DisplayAimIndicator(IPlayerController owner)
	{
		if (!(_aimIndicator == null))
		{
			_aimIndicator.gameObject.SetActive(value: true);
			_aimIndicator.transform.position = _details.aimTarget;
			_aimIndicator.transform.up = Vector3.up;
			_aimIndicator.transform.localScale = Vector3.one * _details.weaponProfile.Explosion.Range * 2f;
			_aimIndicator.Setup(owner);
		}
	}

	private IEnumerator DelayedDisplayAimIndicator(IPlayerController owner)
	{
		yield return new WaitForSeconds(_travelTime * 0.5f);
		DisplayAimIndicator(owner);
	}

	protected override void Move(float speed)
	{
		if (_ascending)
		{
			base.transform.position = Vector3.Lerp(_startPosition, _ascendCrestPosition, Mathf.SmoothStep(0f, 1f, _lifetime / _halfTravelTime));
			if (_lifetime >= _halfTravelTime)
			{
				base.transform.position = _decendCrestPosition;
				base.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
				_ascending = false;
			}
		}
		else
		{
			base.transform.position = Vector3.Lerp(_decendCrestPosition, _aimTarget, (_lifetime - _halfTravelTime) / _halfTravelTime);
			if (_lifetime >= _travelTime)
			{
				_landed = true;
			}
		}
	}

	protected override bool CheckForImpact(Ray ray, out RaycastHit hit)
	{
		if (_landed)
		{
			hit = new RaycastHit
			{
				point = _aimTarget,
				normal = Vector3.up
			};
		}
		else
		{
			hit = default(RaycastHit);
		}
		return _landed;
	}

	protected override HitInfo GenerateHitInfo(RaycastHit hit)
	{
		HitInfo result = base.GenerateHitInfo(hit);
		result.forwardOnHit = Vector3.down;
		return result;
	}

	protected override GameObject TrySpawnImpactEffect(Vector3 position, Vector3 normal)
	{
		GameObject gameObject = base.TrySpawnImpactEffect(position, normal);
		Debug.Log($"[BombardmentProjectile] Spawning impact effect with radius: {_details.effectRadius}");
		if (_details.effectRadius > 0f)
		{
			gameObject.transform.localScale = Vector3.one * _details.effectRadius;
			DebugExtension.DebugWireSphere(gameObject.transform.position, Color.red, _details.effectRadius, 5f);
		}
		return gameObject;
	}
}
