using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class WeaponSlots : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private WeaponSlot _meleeSlot;

	[SerializeField]
	private WeaponSlot[] _slots;

	private int _activeSlotIndex;

	private void Reset()
	{
		_slots = GetComponentsInChildren<WeaponSlot>();
	}

	private IEnumerator Start()
	{
		_signalBus.Subscribe<LoadoutUpdatedSignal>(OnLoadoutUpdated);
		_signalBus.Subscribe<WeaponStateUpdatedSignal>(OnWeaponStateUpdated);
		_signalBus.Subscribe<ChargeValueUpdatedSignal>(OnChargeValueUpdated);
		_signalBus.Subscribe<ActiveWeaponSlotUpdatedSignal>(OnActiveWeaponSlotUpdated);
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
		Weapon[] array = PlayerController.LocalPlayer.state.Loadouts[0].Weapons.ToArray();
		WeaponProfile[] weaponProfiles = PlayerController.LocalPlayer.LoadoutController.WeaponProfiles;
		OnLoadoutUpdated(new LoadoutUpdatedSignal(-1, PlayerController.LocalPlayer.LoadoutController.MeleeWeaponProfile));
		for (int num = 0; num < array.Length; num++)
		{
			OnLoadoutUpdated(new LoadoutUpdatedSignal(num, weaponProfiles[num]));
			OnWeaponStateUpdated(new WeaponStateUpdatedSignal(num, array[num].RemainingAmmo, array[num].MaxAmmo));
		}
		yield return new WaitUntil(() => PlayerController.LocalPlayer.LoadoutController.Outfit != null);
		_activeSlotIndex = PlayerController.LocalPlayer.WeaponHandler.ActiveWeaponIndex;
		if (_activeSlotIndex >= 0)
		{
			_slots[_activeSlotIndex].SetHighlight(isOn: true);
		}
		else
		{
			_meleeSlot.SetHighlight(isOn: true);
		}
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<LoadoutUpdatedSignal>(OnLoadoutUpdated);
		_signalBus.Unsubscribe<WeaponStateUpdatedSignal>(OnWeaponStateUpdated);
		_signalBus.Unsubscribe<ChargeValueUpdatedSignal>(OnChargeValueUpdated);
		_signalBus.Unsubscribe<ActiveWeaponSlotUpdatedSignal>(OnActiveWeaponSlotUpdated);
	}

	private void OnActiveWeaponSlotUpdated(ActiveWeaponSlotUpdatedSignal activeWeaponSlotUpdatedSignal)
	{
		if (_activeSlotIndex >= 0)
		{
			_slots[_activeSlotIndex].SetHighlight(isOn: false);
		}
		else
		{
			_meleeSlot.SetHighlight(isOn: false);
		}
		_activeSlotIndex = activeWeaponSlotUpdatedSignal.ActiveWeaponSlotIndex;
		if (_activeSlotIndex >= 0)
		{
			_slots[_activeSlotIndex].SetHighlight(isOn: true);
		}
		else
		{
			_meleeSlot.SetHighlight(isOn: true);
		}
	}

	public void ClearHighlights()
	{
		_meleeSlot.SetHighlight(isOn: false);
		WeaponSlot[] slots = _slots;
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i].SetHighlight(isOn: false);
		}
	}

	public bool PointerIsOver(PointerEventData eventData, out WeaponSlot slot)
	{
		slot = _slots.FirstOrDefault((WeaponSlot s) => s.IsPointerOver(eventData));
		return slot != null;
	}

	private void OnLoadoutUpdated(LoadoutUpdatedSignal signal)
	{
		if (signal.index < 0)
		{
			_meleeSlot.Display(signal.profile);
		}
		else
		{
			_slots[signal.index].Display(signal.profile);
		}
	}

	private void OnWeaponStateUpdated(WeaponStateUpdatedSignal signal)
	{
		_slots[signal.index].Display(signal);
	}

	private void OnChargeValueUpdated(ChargeValueUpdatedSignal chargeValueUpdatedSignal)
	{
		if (chargeValueUpdatedSignal.index < 0)
		{
			WeaponSlot[] slots = _slots;
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].UpdateChargeValue(0f);
			}
		}
		else
		{
			_slots[chargeValueUpdatedSignal.index].UpdateChargeValue(chargeValueUpdatedSignal.chargeValue);
		}
	}
}
