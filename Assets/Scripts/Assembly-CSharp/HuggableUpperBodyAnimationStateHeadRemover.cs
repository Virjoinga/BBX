using UnityEngine;

public class HuggableUpperBodyAnimationStateHeadRemover : MonoBehaviour
{
	protected HuggableOutfit _outfit;

	protected WeaponAnimationController _weaponAnimationController;

	protected virtual void Start()
	{
		_outfit = GetComponentInParent<HuggableOutfit>();
		_weaponAnimationController = GetComponent<WeaponAnimationController>();
		_weaponAnimationController.PlayerAnimationController.UpperBodyAnimationStateUpdated += OnUpperBodyAnimationStateUpdated;
	}

	private void OnDestroy()
	{
		if (_weaponAnimationController != null && _weaponAnimationController.PlayerAnimationController != null)
		{
			_weaponAnimationController.PlayerAnimationController.UpperBodyAnimationStateUpdated -= OnUpperBodyAnimationStateUpdated;
		}
	}

	private void OnUpperBodyAnimationStateUpdated(bool isActive)
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (isActive)
			{
				_outfit.RemoveHead();
			}
			else
			{
				_outfit.CancelRemoveHead();
			}
		}
	}

	private void OnDisable()
	{
		if (_outfit != null)
		{
			_outfit.CancelRemoveHead();
		}
	}
}
