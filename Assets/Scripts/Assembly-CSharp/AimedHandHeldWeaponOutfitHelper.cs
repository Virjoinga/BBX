using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;

public class AimedHandHeldWeaponOutfitHelper : MonoBehaviour
{
	public enum Hand
	{
		Right = 0,
		Left = 1
	}

	[SerializeField]
	private Outfit _outfit;

	[SerializeField]
	private AimIK _rightArmAimIK;

	[SerializeField]
	private AimIK _leftArmAimIK;

	private bool _aimDisabled;

	private Coroutine _aimChangeRoutine;

	public Outfit Outfit => _outfit;

	public void DisableAiming()
	{
		_aimDisabled = true;
		if (_rightArmAimIK != null)
		{
			_rightArmAimIK.solver.IKPositionWeight = 0f;
		}
		if (_leftArmAimIK != null)
		{
			_leftArmAimIK.solver.IKPositionWeight = 0f;
		}
	}

	public void OnWeaponEquipped(FiringWeapon weapon, Hand hand)
	{
		if (hand == Hand.Right && _rightArmAimIK != null)
		{
			_rightArmAimIK.solver.transform = weapon.LaunchPoint.transform;
		}
		else if (hand == Hand.Left && _leftArmAimIK != null)
		{
			_leftArmAimIK.solver.transform = weapon.LaunchPoint.transform;
		}
	}

	public void OnExtendedStateChanged(bool isExtended, Hand hand, float delay)
	{
		SetAimIkEnabledState(isExtended, hand, delay);
	}

	public void OnReloadingStateChanged(bool isReloading, Hand hand)
	{
		SetAimIkEnabledState(!isReloading, hand);
	}

	private void SetAimIkEnabledState(bool shouldAim, Hand hand, float delay = 0f)
	{
		if (!_aimDisabled)
		{
			if (_aimChangeRoutine != null)
			{
				StopCoroutine(_aimChangeRoutine);
			}
			AimIK aimIK = ((hand == Hand.Right) ? _rightArmAimIK : _leftArmAimIK);
			if (aimIK != null)
			{
				_aimChangeRoutine = StartCoroutine(SetAimIkEnabledStateRoutine(shouldAim, aimIK, delay));
			}
		}
	}

	private IEnumerator SetAimIkEnabledStateRoutine(bool shouldAim, AimIK aimIK, float delay = 0f)
	{
		if (shouldAim && delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		float start = aimIK.solver.IKPositionWeight;
		float end = (shouldAim ? 1f : 0f);
		for (float t = 0f; t < 1f; t += Time.deltaTime * 8f)
		{
			aimIK.solver.IKPositionWeight = Mathf.SmoothStep(start, end, t);
			yield return null;
		}
		aimIK.solver.IKPositionWeight = end;
		_aimChangeRoutine = null;
	}
}
