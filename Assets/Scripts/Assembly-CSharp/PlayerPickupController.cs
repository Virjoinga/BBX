using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;
using Zenject;

public class PlayerPickupController : BaseEntityEventListener<IPlayerState>, ICanPickup
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private float _pickupRadius = 1f;

	[SerializeField]
	private LayerMask _pickupsLayer;

	private PlayerAudioStateController _playerAudioStateController;

	private HealthController _healthController;

	private StatusEffectController _statusEffectController;

	private MatchStateHelper _matchStateHelper;

	private bool _enableAutoPickup = true;

	private Collider[] _overlapResults = new Collider[10];

	private int _numPickupsInRange;

	private WeaponPickupEntity _closestPickup;

	private string _lastSentPickupProfileId;

	private bool _wasInRangeOfPickup;

	private int _pickupSlot;

	private bool _sendPickupCommand;

	private new void Awake()
	{
		_playerAudioStateController = GetComponent<PlayerAudioStateController>();
		_healthController = GetComponent<HealthController>();
		_statusEffectController = GetComponent<StatusEffectController>();
		_matchStateHelper = GetComponent<MatchStateHelper>();
	}

	protected override void OnControllerOnlyAttached()
	{
		_signalBus.Subscribe<TryPickupItemSignal>(FlagForPickup);
		base.state.AddCallback("Loadouts[].Weapons[].Id", OnEquippedWeaponChanged);
		base.state.AddCallback("IsPoweredUp", OnIsPoweredUpUpdated);
	}

	protected override void OnOwnerOnlyAttached()
	{
		_healthController.Died += OnDied;
	}

	private void OnIsPoweredUpUpdated()
	{
		Debug.Log($"[PlayerPickupController] IsPoweredUp changed to {base.state.IsPoweredUp}");
	}

	protected override void OnControllerOnlyDetached()
	{
		_signalBus.Unsubscribe<TryPickupItemSignal>(FlagForPickup);
		if (base.state.Damageable.Health > 0f && (base.state.GameModeType == 1 || base.state.GameModeType == 2))
		{
			GameModeEntityHelper.DropAllItems(base.state, base.transform.position, base.transform.rotation, base.state.Loadouts[0].Weapons[0].Id);
		}
	}

	public override void SimulateController()
	{
		if (!base.state.IsSecondLife && _matchStateHelper.MatchStateCached == MatchState.Active)
		{
			HandlePickupInput();
			DetectPickupsInRange();
			UpdatePickupsUI();
		}
	}

	public override void SimulateOwner()
	{
		if (!base.state.IsSecondLife && _matchStateHelper.MatchStateCached == MatchState.Active)
		{
			DetectPickupsInRange();
			HandleAutoPickups();
		}
	}

	private void FlagForPickup(TryPickupItemSignal tryPickupItemSignal)
	{
		_pickupSlot = tryPickupItemSignal.Slot;
		_sendPickupCommand = true;
	}

	private void HandlePickupInput()
	{
		if (_sendPickupCommand && !base.state.Stunned && !base.state.WeaponsDisabled)
		{
			if (_closestPickup != null && _closestPickup.entity != null)
			{
				Debug.Log($"[PlayerPickupController] Sending pickup command {_closestPickup.PickupProfile.Id} | {_pickupSlot}");
				SendPickupCommand(_closestPickup.entity, _pickupSlot, PickupType.Weapon);
			}
			else
			{
				Debug.LogError("[PlayerPickupController] Trying to send pickup command with null pickup entity");
			}
		}
		_sendPickupCommand = false;
	}

	private void SendPickupCommand(BoltEntity pickupEntity, int slot, PickupType pickupType)
	{
		IPickupItemCommandInput pickupItemCommandInput = PickupItemCommand.Create();
		pickupItemCommandInput.Entity = pickupEntity;
		pickupItemCommandInput.Slot = slot;
		pickupItemCommandInput.Type = (int)pickupType;
		base.entity.QueueInput(pickupItemCommandInput);
	}

	private void DetectPickupsInRange()
	{
		_numPickupsInRange = Physics.OverlapSphereNonAlloc(base.transform.position, _pickupRadius, _overlapResults, _pickupsLayer);
	}

	private void UpdatePickupsUI()
	{
		bool flag = false;
		if (!base.state.WeaponsDisabled)
		{
			if (_numPickupsInRange > 0)
			{
				List<WeaponPickupEntity> possibleWeaponPickups = GetPossibleWeaponPickups(_numPickupsInRange);
				if (possibleWeaponPickups.Count > 0)
				{
					flag = true;
					TryFindClosestWeaponPickup(possibleWeaponPickups);
				}
			}
			else
			{
				_closestPickup = null;
			}
		}
		if (!flag && _wasInRangeOfPickup)
		{
			_wasInRangeOfPickup = false;
			_lastSentPickupProfileId = string.Empty;
			_signalBus.Fire<OutOfRangeOfPickupSignal>();
		}
	}

	private List<WeaponPickupEntity> GetPossibleWeaponPickups(int numFound)
	{
		List<WeaponPickupEntity> list = new List<WeaponPickupEntity>();
		for (int i = 0; i < numFound; i++)
		{
			WeaponPickupEntity component = _overlapResults[i].GetComponent<WeaponPickupEntity>();
			if (component != null && component.PickupProfile != null)
			{
				List<string> list2 = new List<string>
				{
					base.state.Loadouts[0].Weapons[1].Id,
					base.state.Loadouts[0].Weapons[2].Id,
					base.state.Loadouts[0].Weapons[0].Id
				};
				if (list2.Count <= 0 || !list2.Contains(component.PickupProfile.Id))
				{
					list.Add(component);
				}
			}
		}
		return list;
	}

	private void TryFindClosestWeaponPickup(List<WeaponPickupEntity> possiblePickups)
	{
		_closestPickup = null;
		float num = float.MaxValue;
		foreach (WeaponPickupEntity possiblePickup in possiblePickups)
		{
			float sqrMagnitude = (base.transform.position - possiblePickup.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				_closestPickup = possiblePickup;
			}
		}
		if (_closestPickup != null && _closestPickup.PickupProfile != null)
		{
			if (!(_lastSentPickupProfileId != _closestPickup.PickupProfile.Id))
			{
				return;
			}
			_lastSentPickupProfileId = _closestPickup.PickupProfile.Id;
			_wasInRangeOfPickup = true;
			if (_enableAutoPickup && _closestPickup.PickupProfile.ItemType != ItemType.meleeWeapon && HasEmptyWeaponSlot())
			{
				for (int i = 1; i < base.state.Loadouts[0].Weapons.Length; i++)
				{
					if (string.IsNullOrEmpty(base.state.Loadouts[0].Weapons[i].Id))
					{
						_pickupSlot = i;
						_sendPickupCommand = true;
						break;
					}
				}
			}
			else
			{
				_signalBus.Fire(new InRangeOfPickupSignal
				{
					PickupProfile = _closestPickup.PickupProfile
				});
			}
		}
		else if (_wasInRangeOfPickup)
		{
			_wasInRangeOfPickup = false;
			_lastSentPickupProfileId = string.Empty;
			_signalBus.Fire<OutOfRangeOfPickupSignal>();
		}
	}

	private bool HasEmptyWeaponSlot()
	{
		return base.state.Loadouts[0].Weapons.Any((Weapon w) => string.IsNullOrEmpty(w.Id));
	}

	private void HandleAutoPickups()
	{
		if (_numPickupsInRange <= 0)
		{
			return;
		}
		for (int i = 0; i < _numPickupsInRange; i++)
		{
			AmmoClipPickupEntity component = _overlapResults[i].GetComponent<AmmoClipPickupEntity>();
			if (component != null && component.IsActive)
			{
				TryPickupAmmoClip(component.entity);
			}
		}
	}

	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (!base.state.IsSecondLife && command is PickupItemCommand)
		{
			ExecuteCommand(command as PickupItemCommand, resetState);
		}
	}

	private void ExecuteCommand(PickupItemCommand cmd, bool resetState)
	{
		if (!base.entity.isOwner || !base.state.InputEnabled || base.state.Stunned || base.state.WeaponsDisabled)
		{
			return;
		}
		BoltEntity boltEntity = cmd.Input.Entity;
		if (boltEntity == null)
		{
			Debug.LogError("[PlayerPickupController] Got Command to pickup Null Entity");
			return;
		}
		switch ((PickupType)cmd.Input.Type)
		{
		case PickupType.Weapon:
			cmd.Result.Successful = TryPickupWeapon(boltEntity, cmd.Input.Slot);
			break;
		case PickupType.AmmoClip:
			cmd.Result.Successful = TryPickupAmmoClip(boltEntity);
			break;
		}
	}

	private bool TryPickupWeapon(BoltEntity pickupEntityObj, int slotIndex)
	{
		WeaponPickupEntity component = pickupEntityObj.gameObject.GetComponent<WeaponPickupEntity>();
		if (component != null && component.TryClaimPickup())
		{
			PickupWeapon(component.PickupProfile, slotIndex);
			BoltNetwork.Destroy(component.gameObject);
			return true;
		}
		return false;
	}

	private bool TryPickupAmmoClip(BoltEntity entity)
	{
		Debug.Log("[PlayerPickupController] Picking up ammo clip");
		base.state.AmmoClips += 1f;
		BoltNetwork.Destroy(entity.gameObject);
		return true;
	}

	private void PickupWeapon(ProfileWithHeroClass itemProfile, int slotIndex)
	{
		if (itemProfile == null)
		{
			Debug.LogError("[PlayerPickupController] Trying to pickup item with a null profile");
		}
		else if (slotIndex != 0 || itemProfile.ItemType == ItemType.meleeWeapon)
		{
			Debug.Log("[PlayerPickupController] Picking up Item " + itemProfile.Id);
			if (!string.IsNullOrEmpty(base.state.Loadouts[0].Weapons[slotIndex].Id))
			{
				GameModeEntityHelper.DropExistingItem(base.state.Loadouts[0].Weapons[slotIndex].Id, base.transform.position, base.transform.rotation);
			}
			if (base.state.Loadouts[0].Weapons.All((Weapon w) => string.IsNullOrEmpty(w.Id)))
			{
				base.state.Loadouts[0].ActiveWeapon = slotIndex;
			}
			base.state.Loadouts[0].Weapons[slotIndex].Id = itemProfile.Id;
		}
	}

	private void OnEquippedWeaponChanged(IState state, string propertyPath, ArrayIndices arrayIndices)
	{
		if (_playerAudioStateController != null)
		{
			_playerAudioStateController.PlayWeaponPickupSFX();
		}
	}

	public bool TryPickup(PickupData pickup, BoltEntity pickupEntity)
	{
		if (pickup is HealthPickupData)
		{
			return TryPickup(pickup as HealthPickupData);
		}
		if (pickup is EffectApplyingPickupData)
		{
			return TryPickup(pickup as EffectApplyingPickupData, pickupEntity);
		}
		Debug.LogError("[PlayerPickupController] Tried to pickup pickup of type " + pickup.GetType().Name + ", but could not find a case for it.");
		return false;
	}

	private bool TryPickup(HealthPickupData healthPickup)
	{
		return _healthController.TryHeal(healthPickup.Value);
	}

	private bool TryPickup(EffectApplyingPickupData effectApplyingPickup, BoltEntity pickupEntity)
	{
		if (!CanPickup(effectApplyingPickup))
		{
			return false;
		}
		if (base.state.IsPoweredUp)
		{
			_statusEffectController.TryKillExistingPowerupEffect();
		}
		bool num = _statusEffectController.TryApplyEffect("", effectApplyingPickup.Effect, pickupEntity, isPowerup: true, onEffectComplete);
		if (num)
		{
			Debug.Log("[PlayerPickupController] Powerup started");
			base.state.IsPoweredUp = true;
		}
		return num;
		void onEffectComplete()
		{
			Debug.Log("[PlayerPickupController] Powerup ended");
			base.state.IsPoweredUp = false;
		}
	}

	private void OnDied()
	{
		base.state.IsPoweredUp = false;
	}

	public bool CanPickup(PickupData pickup)
	{
		if (_matchStateHelper.MatchStateCached != MatchState.Active)
		{
			return false;
		}
		if (pickup is HealthPickupData)
		{
			return CanPickup(pickup as HealthPickupData);
		}
		if (pickup is EffectApplyingPickupData)
		{
			return CanPickup(pickup as EffectApplyingPickupData);
		}
		Debug.LogError("[PlayerPickupController] Tried to mock pickup pickup of type " + pickup.GetType().Name + ", but could not find a case for it.");
		return false;
	}

	private bool CanPickup(HealthPickupData healthPickup)
	{
		return base.state.Damageable.Health < base.state.Damageable.MaxHealth;
	}

	private bool CanPickup(EffectApplyingPickupData effectApplyingPickup)
	{
		return true;
	}
}
