using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class WeaponSlot : UIBaseButtonClickHandler
{
	public enum SlotIndex
	{
		Melee = -1,
		Slot2 = 0,
		Slot3 = 1,
		Slot4 = 2,
		Slot5 = 3
	}

	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private GameObject _ammoContainer;

	[SerializeField]
	private TextMeshProUGUI _remainingAmmo;

	[SerializeField]
	private TextMeshProUGUI _maxAmmo;

	[SerializeField]
	private Option _keyCode;

	[SerializeField]
	private GameObject _highlight;

	[SerializeField]
	private SlotIndex _index = SlotIndex.Melee;

	[SerializeField]
	private Image _chargeFillBar;

	[SerializeField]
	private GameObject _chargeBar;

	[SerializeField]
	private Image _cooldownBar;

	[SerializeField]
	private GameObject _deployableContainer;

	[SerializeField]
	private TextMeshProUGUI _maxDeployables;

	[SerializeField]
	private TextMeshProUGUI _activeDeployables;

	private WeaponProfile _weaponProfile;

	private RectTransform _rectTransform;

	private int _activeDeployableCount;

	public int Index => (int)_index;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_highlight.SetActive(value: false);
		_deployableContainer.SetActive(value: false);
		_icon.enabled = false;
		_ammoContainer.SetActive(value: false);
		_signalBus.Subscribe<DeployableCreatedDestroyedSignal>(OnDeployableCreatedDestroyed);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<DeployableCreatedDestroyedSignal>(OnDeployableCreatedDestroyed);
	}

	private void OnDeployableCreatedDestroyed(DeployableCreatedDestroyedSignal signal)
	{
		if (_weaponProfile != null && _weaponProfile.SpawnedEntity.IsDeployable && !(_weaponProfile.Id != signal.WeaponProfileId))
		{
			if (signal.IsActive)
			{
				_activeDeployableCount++;
			}
			else
			{
				_activeDeployableCount--;
			}
			_activeDeployableCount = Mathf.Clamp(_activeDeployableCount, 0, _weaponProfile.SpawnedEntity.MaxDeployedCount);
			_activeDeployables.text = _activeDeployableCount.ToString();
		}
	}

	private void Update()
	{
		if (BSCoreInput.GetButtonDown(_keyCode))
		{
			OnClick();
		}
	}

	public bool IsPointerOver(PointerEventData eventData)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, eventData.position);
	}

	public void SetHighlight(bool isOn)
	{
		_highlight.SetActive(isOn);
	}

	protected override void OnClick()
	{
		_signalBus.Fire(new WeaponSlotSelectedSignal(Index));
	}

	public void Display(WeaponProfile profile)
	{
		_weaponProfile = profile;
		if (_weaponProfile == null)
		{
			_icon.enabled = false;
			_ammoContainer.SetActive(value: false);
			_deployableContainer.SetActive(value: false);
			return;
		}
		_icon.sprite = _weaponProfile.Icon;
		_icon.enabled = true;
		_ammoContainer.SetActive(!_weaponProfile.IsMelee && !_weaponProfile.Reload.InBackground);
		_chargeBar.SetActive(_weaponProfile.Charge.CanCharge);
		_cooldownBar.gameObject.SetActive(_weaponProfile.Reload.InBackground);
		_cooldownBar.fillAmount = 0f;
		_deployableContainer.SetActive(_weaponProfile.SpawnedEntity.IsDeployable);
		_activeDeployables.text = "0";
		_maxDeployables.text = _weaponProfile.SpawnedEntity.MaxDeployedCount.ToString();
	}

	public void Display(WeaponStateUpdatedSignal signal)
	{
		if (signal.reloadsInBackground)
		{
			_cooldownBar.gameObject.SetActive(signal.reloadPercent > 0f);
			_cooldownBar.fillAmount = signal.reloadPercent;
		}
		else
		{
			_remainingAmmo.text = signal.remainingAmmo.ToString();
			_maxAmmo.text = signal.maxAmmo.ToString();
		}
	}

	public void UpdateChargeValue(float chargeValue)
	{
		_chargeFillBar.fillAmount = chargeValue;
	}
}
