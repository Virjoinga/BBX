using System;
using System.Collections;
using BSCore;
using BSCore.Constants.Config;
using UnityEngine;
using Zenject;

public class ZigguratDoorsController : BaseEntityBehaviour<IBearrensZigguratDoorsState>
{
	[Serializable]
	private class DoorDropConfig
	{
		public float openCooldownMin;

		public float openCooldownMax;

		public float openDuration;

		public float dropDuration;

		public float resetDuration;
	}

	[Inject]
	private ConfigManager _configManager;

	[SerializeField]
	private Transform _positiveAngleDoor;

	[SerializeField]
	private Transform _negativeAngleDoor;

	private DoorDropConfig _doorDropConfig;

	protected override void OnRemoteOnlyAttached()
	{
		base.state.AddCallback("DoorAngle", OnDoorAngleChanged);
	}

	protected override void OnOwnerOnlyAttached()
	{
		_doorDropConfig = _configManager.Get<DoorDropConfig>(DataKeys.BearrensDoors);
		StartCoroutine(DoorDropRoutine());
	}

	private void OnDoorAngleChanged()
	{
		_positiveAngleDoor.localEulerAngles = new Vector3(base.state.DoorAngle, 0f, 0f);
		_negativeAngleDoor.localEulerAngles = new Vector3(0f - base.state.DoorAngle, 0f, 0f);
	}

	private void DestroyIfNotAttached()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator DoorDropRoutine()
	{
		float dropStep = 1f / _doorDropConfig.dropDuration;
		float resetStep = 1f / _doorDropConfig.resetDuration;
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(_doorDropConfig.openCooldownMin, _doorDropConfig.openCooldownMax));
			for (float t = 0f; t < 1f; t += dropStep * Time.deltaTime)
			{
				base.state.DoorAngle = Mathf.Lerp(0f, 90f, Mathf.SmoothStep(0f, 1f, t));
				OnDoorAngleChanged();
				yield return null;
			}
			yield return new WaitForSeconds(_doorDropConfig.openDuration);
			for (float t = 0f; t < 1f; t += resetStep * Time.deltaTime)
			{
				base.state.DoorAngle = Mathf.Lerp(90f, 0f, Mathf.SmoothStep(0f, 1f, t));
				OnDoorAngleChanged();
				yield return null;
			}
		}
	}
}
