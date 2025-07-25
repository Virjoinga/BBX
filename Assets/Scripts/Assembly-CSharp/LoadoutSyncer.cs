using System.Collections;
using Bolt;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(LoadoutController))]
public class LoadoutSyncer : EntityEventListener<IPlayerState>
{
	[Inject]
	private SignalBus _signalBus;

	private LoadoutController _loadoutController;

	private StatusEffectController _statusEffectController;

	private PlayerAnimationController _animationController;

	private PlayerController _playerController;

	private WeaponController _weaponController;

	protected virtual void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		_loadoutController = GetComponent<LoadoutController>();
		_statusEffectController = GetComponent<StatusEffectController>();
		_animationController = GetComponent<PlayerAnimationController>();
		_weaponController = GetComponent<WeaponController>();
	}

	public override void Attached()
	{
		base.state.AddCallback("Loadouts[].MeleeWeapon.Id", EquipMeleeWeapon);
		base.state.AddCallback("Loadouts[].Weapons[].Id", EquipWeapon);
		base.state.AddCallback("Loadouts[].Backpack", EquipBackpack);
		base.state.AddCallback("Loadouts[].Hat", EquipHat);
		base.state.AddCallback("Loadouts[].Outfit", EquipOutfit);
		base.state.AddCallback("Loadouts[].MajorEquipmentSlot", EquipMajorEquipmentSlot);
		base.state.AddCallback("Loadouts[].MinorEquipmentSlot", EquipMinorEquipmentSlot);
	}

	private void EquipOutfit()
	{
		Debug.Log($"[LoadoutSyncer] Equipping Outfit - {base.state.Loadouts[0].Outfit} on player ({base.gameObject.name}). Is Local? {base.entity.isControlled}");
		_loadoutController.EquipOutfit(base.state.Loadouts[0].Outfit, OnOutfitEquipCompleted);
	}

	private void OnOutfitEquipCompleted(bool didEquip)
	{
		if (didEquip)
		{
			_loadoutController.Outfit.AimPointHandler.IsLocalPlayer = base.entity.isControlled && !base.entity.isOwner;
			Animator component = _loadoutController.Outfit.GetComponent<Animator>();
			if (component != null)
			{
				base.state.SetAnimator(component);
			}
			else
			{
				Debug.LogError("[LoadoutSyncer] Failed to get animator from outfit - " + _loadoutController.Outfit.Profile.Id + ". Unable to set animator on bolt state");
			}
			_statusEffectController.Setup(_loadoutController.Outfit);
			if (base.entity.isOwner)
			{
				_animationController.CullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			StartCoroutine(DeployWeaponWhenReady());
		}
	}

	private IEnumerator DeployWeaponWhenReady()
	{
		yield return new WaitUntil(() => _loadoutController.Weapons[base.state.Loadouts[0].ActiveWeapon] != null);
		_weaponController.DeployWeapon(base.state.Loadouts[0].ActiveWeapon);
	}

	private void EquipMeleeWeapon()
	{
		WeaponProfile weaponProfile = _loadoutController.EquipMeleeWeapon(base.state.Loadouts[0].MeleeWeapon.Id);
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			Debug.Log($"[LoadoutSyncer] Sending Weapon Loadout update signal. index {-1} | profile {weaponProfile}");
			_signalBus.Fire(new LoadoutUpdatedSignal(-1, weaponProfile));
		}
	}

	private void EquipWeapon(IState iState, string propertyPath, ArrayIndices arrayIndices)
	{
		int num = arrayIndices[1];
		Weapon weapon = base.state.Loadouts[0].Weapons[num];
		WeaponProfile weaponProfile = _loadoutController.EquipWeapon(num, weapon.Id, setProfileOnly: false, WeaponEquipCompleted);
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			Debug.Log($"[LoadoutSyncer] Sending Weapon Loadout update signal. index {num} | profile {weaponProfile}");
			_signalBus.Fire(new LoadoutUpdatedSignal(num, weaponProfile));
		}
	}

	private void WeaponEquipCompleted(int index, bool didSet)
	{
		if (index == base.state.Loadouts[0].ActiveWeapon)
		{
			_weaponController.DeployWeapon(index);
		}
	}

	private void EquipBackpack()
	{
		if (!string.IsNullOrEmpty(base.state.Loadouts[0].Backpack))
		{
			_loadoutController.EquipBackpack(base.state.Loadouts[0].Backpack);
		}
	}

	private void EquipHat()
	{
		if (!string.IsNullOrEmpty(base.state.Loadouts[0].Hat))
		{
			_loadoutController.EquipHat(base.state.Loadouts[0].Hat);
		}
	}

	private void EquipMajorEquipmentSlot()
	{
		if (!string.IsNullOrEmpty(base.state.Loadouts[0].MajorEquipmentSlot))
		{
			_loadoutController.EquipMajorEquipmentSlot(base.state.Loadouts[0].MajorEquipmentSlot);
		}
	}

	private void EquipMinorEquipmentSlot()
	{
		if (!string.IsNullOrEmpty(base.state.Loadouts[0].MinorEquipmentSlot))
		{
			_loadoutController.EquipMinorEquipmentSlot(base.state.Loadouts[0].MinorEquipmentSlot);
		}
	}
}
