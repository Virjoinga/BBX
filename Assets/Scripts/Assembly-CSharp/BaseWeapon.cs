using System;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
	[SerializeField]
	protected WeaponAudioPlayer _weaponAudioPlayer;

	public Func<int> GetServerFrame = () => (int)(Time.realtimeSinceStartup / Time.fixedDeltaTime);

	protected int _fireDelay = 1;

	protected WeaponProfile _profile;

	public WeaponAudioPlayer AudioPlayer => _weaponAudioPlayer;

	public WeaponProfile Profile
	{
		get
		{
			return _profile;
		}
		set
		{
			_profile = value;
			UpdateProperties();
		}
	}

	private event Action<bool> _enabledStateChanged;

	public event Action<bool> EnabledStateChanged
	{
		add
		{
			_enabledStateChanged += value;
		}
		remove
		{
			_enabledStateChanged -= value;
		}
	}

	private event Action<HitInfo> _hit;

	public event Action<HitInfo> Hit
	{
		add
		{
			_hit += value;
		}
		remove
		{
			_hit -= value;
		}
	}

	protected virtual void Reset()
	{
		_weaponAudioPlayer = GetComponent<WeaponAudioPlayer>();
	}

	protected virtual void Awake()
	{
		base.name = base.name.Replace("(Clone)", "").Trim();
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnEnable()
	{
		this._enabledStateChanged?.Invoke(obj: true);
	}

	protected virtual void OnDisable()
	{
		this._enabledStateChanged?.Invoke(obj: false);
	}

	protected virtual void UpdateProperties()
	{
		_fireDelay = Mathf.FloorToInt(_profile.Cooldown / Time.fixedDeltaTime);
	}

	protected virtual void RaiseHit(HitInfo hitInfo)
	{
		hitInfo.weaponId = _profile.Id;
		hitInfo.effects = _profile.Effects;
		hitInfo.weaponProfile = _profile;
		this._hit?.Invoke(hitInfo);
	}
}
