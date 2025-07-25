using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseMeleeWeaponAnimationController : MonoBehaviour
{
	[SerializeField]
	protected Animator _animator;

	[SerializeField]
	protected MeleeWeapon _meleeWeapon;

	protected virtual void Reset()
	{
		_animator = GetComponent<Animator>();
		_meleeWeapon = GetComponent<MeleeWeapon>();
		if (_meleeWeapon == null)
		{
			_meleeWeapon = GetComponentInParent<MeleeWeapon>();
		}
	}

	protected virtual void Start()
	{
		_meleeWeapon.MeleeAnimationTriggered += TriggerMeleeAnim;
	}

	public abstract void Setup();

	protected abstract void TriggerMeleeAnim(WeaponProfile.MeleeOptionData meleeOption);
}
