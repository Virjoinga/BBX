using UnityEngine;

public class HuggableFiringWeaponHeadRemover : MonoBehaviour
{
	protected HuggableOutfit _outfit;

	protected FiringWeapon _weapon;

	protected virtual void Start()
	{
		_outfit = GetComponentInParent<HuggableOutfit>();
		_weapon = GetComponent<FiringWeapon>();
		_weapon.Fired += OnFired;
		_weapon.FireCanceled += OnFireCanceled;
	}

	private void OnDisable()
	{
		if (_outfit != null)
		{
			_outfit.CancelRemoveHead();
		}
	}

	protected virtual void OnFired()
	{
		_outfit.RemoveHead(_weapon.Profile.Cooldown);
	}

	protected virtual void OnFireCanceled()
	{
		_outfit.CancelRemoveHead();
	}
}
