using System.Collections;
using BSCore;
using UnityEngine;

public class AimedHandHeldWeaponHelper : MonoBehaviour
{
	[SerializeField]
	private AimedHandHeldWeaponOutfitHelper.Hand _hand;

	[SerializeField]
	private Transform _objectToPosition;

	[SerializeField]
	private float _enableDelay;

	[SerializeField]
	private float _disableDelay;

	[SerializeField]
	private bool _placeUnderHand = true;

	private AimedHandHeldWeaponOutfitHelper _outfitHelper;

	private Transform _handContainer;

	private IEnumerator Start()
	{
		FiringWeapon weapon = GetComponent<FiringWeapon>();
		weapon.AnimationController.ExtendedStateChanged += OnExtendedStateChanged;
		weapon.AnimationController.ReloadingStateChanged += OnReloadingStateChanged;
		_outfitHelper = GetComponentInParent<AimedHandHeldWeaponOutfitHelper>();
		if (_outfitHelper == null)
		{
			do
			{
				yield return new WaitForSeconds(0.1f);
				_outfitHelper = GetComponentInParent<AimedHandHeldWeaponOutfitHelper>();
			}
			while (_outfitHelper == null);
		}
		_outfitHelper.OnWeaponEquipped(weapon, _hand);
		_handContainer = ((_hand == AimedHandHeldWeaponOutfitHelper.Hand.Left) ? _outfitHelper.Outfit.LeftHandContainer : _outfitHelper.Outfit.RightHandContainer);
		if (_placeUnderHand)
		{
			PlaceUnderHand();
		}
		if (weapon.AnimationController.IsExtended)
		{
			OnExtendedStateChanged(weapon.AnimationController.IsExtended);
		}
	}

	private void OnExtendedStateChanged(bool isExtended)
	{
		if (!(_outfitHelper == null))
		{
			_outfitHelper.OnExtendedStateChanged(isExtended, _hand, _enableDelay);
		}
	}

	private void OnReloadingStateChanged(bool isReloading)
	{
		if (!(_outfitHelper == null))
		{
			_outfitHelper.OnReloadingStateChanged(isReloading, _hand);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(SetActiveAfterDelay(_enableDelay, isActive: true));
	}

	private void OnDisable()
	{
		if (_objectToPosition != null)
		{
			if (base.gameObject == null)
			{
				Object.Destroy(_objectToPosition.gameObject);
			}
			else
			{
				DelayedAction.RunCoroutine(SetActiveAfterDelay(_disableDelay, isActive: false));
			}
		}
	}

	private void OnDestroy()
	{
		if (_objectToPosition != null)
		{
			Object.Destroy(_objectToPosition.gameObject);
		}
	}

	private IEnumerator SetActiveAfterDelay(float delay, bool isActive)
	{
		if (!(_objectToPosition != null))
		{
			yield break;
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (!(_objectToPosition == null))
		{
			if (isActive)
			{
				_objectToPosition.gameObject.SetActive(value: true);
				PlaceUnderHand();
			}
			else
			{
				_objectToPosition.SetParent(base.transform);
				_objectToPosition.gameObject.SetActive(value: false);
			}
		}
	}

	private void PlaceUnderHand()
	{
		_objectToPosition.SetParent(_handContainer);
		_objectToPosition.localPosition = Vector3.zero;
		_objectToPosition.localScale = Vector3.one;
		_objectToPosition.localRotation = Quaternion.identity;
	}
}
