using System.Collections;
using UnityEngine;

public class ContinuousFireWeapon : FiringWeapon
{
	[SerializeField]
	private GameObject _effect;

	private Coroutine _fireSFXRoutine;

	public override HitType HitType => HitType.Continuous;

	protected override void Start()
	{
		base.Start();
		Outfit componentInParent = base.gameObject.GetComponentInParent<Outfit>();
		if (componentInParent != null && _effect != null)
		{
			_effect.transform.SetParent(componentInParent.transform);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (_effect != null)
		{
			_effect.SetActive(value: true);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (_effect != null)
		{
			_effect.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (_effect != null)
		{
			Object.Destroy(_effect);
		}
	}

	public override void CancelFire()
	{
		base.CancelFire();
		if (_fireSFXRoutine != null)
		{
			StopCoroutine(_fireSFXRoutine);
			_weaponAudioPlayer.StopContinuousFire();
			_fireSFXRoutine = null;
		}
	}

	protected override void TryPlayFireSFX()
	{
		if (_fireSFXRoutine != null)
		{
			StopCoroutine(_fireSFXRoutine);
		}
		else
		{
			_weaponAudioPlayer.PlayContinuousFire();
		}
		_fireSFXRoutine = StartCoroutine(StopFireSFXAfterTime());
	}

	private IEnumerator StopFireSFXAfterTime()
	{
		yield return new WaitForSeconds(_profile.Cooldown + 0.05f);
		_weaponAudioPlayer.StopContinuousFire();
		_fireSFXRoutine = null;
	}
}
