using System.Collections;
using UnityEngine;

public class SimpleMeleeWeaponAnimationController : BaseMeleeWeaponAnimationController
{
	public override void Setup()
	{
	}

	protected override void TriggerMeleeAnim(WeaponProfile.MeleeOptionData meleeOption)
	{
		_animator.SetBool("IsAttacking", value: true);
		StartCoroutine(DisableAfterAnim(meleeOption));
	}

	private IEnumerator DisableAfterAnim(WeaponProfile.MeleeOptionData meleeOption)
	{
		yield return new WaitForSeconds(meleeOption.Cooldown);
		_animator.SetBool("IsAttacking", value: true);
	}
}
