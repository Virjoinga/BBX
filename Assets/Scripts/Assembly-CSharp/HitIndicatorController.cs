using System.Collections;
using UnityEngine;
using Zenject;

public class HitIndicatorController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private RectTransform _rect;

	[SerializeField]
	private HitIndicator _hitIndicatorPrefab;

	[SerializeField]
	private float _hitIndicatorRadius = 40f;

	private Transform _playerTransform;

	private IEnumerator Start()
	{
		_signalBus.Subscribe<DamagableDamagedSignal>(OnPlayerDamaged);
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
		_playerTransform = PlayerController.LocalPlayer.transform;
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<DamagableDamagedSignal>(OnPlayerDamaged);
	}

	private void OnPlayerDamaged(DamagableDamagedSignal playerDamagedSignal)
	{
		if (!PlayerController.HasLocalPlayer || (!(PlayerController.LocalPlayer.entity.networkId == playerDamagedSignal.Attacker.networkId) && !(playerDamagedSignal.Victim != PlayerController.LocalPlayer.entity)))
		{
			HitIndicator hitIndicator = SmartPool.Spawn(_hitIndicatorPrefab);
			PositionIndictor(hitIndicator.Rect, playerDamagedSignal.Attacker.transform.position);
		}
	}

	private void PositionIndictor(RectTransform hitIndicator, Vector3 worldPosition)
	{
		Vector3 vector = _playerTransform.InverseTransformPoint(worldPosition);
		vector = Vector3.ProjectOnPlane(vector, _playerTransform.up);
		vector.y = vector.z;
		vector.z = 0f;
		vector.Normalize();
		hitIndicator.position = _rect.position + vector * _hitIndicatorRadius;
		hitIndicator.up = vector;
	}
}
