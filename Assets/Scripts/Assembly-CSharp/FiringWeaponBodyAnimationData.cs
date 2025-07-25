using System;
using UnityEngine;

[Serializable]
public class FiringWeaponBodyAnimationData
{
	[SerializeField]
	private PlayerAnimationController.SectionLayer _section;

	[SerializeField]
	private bool _includeHead;

	[SerializeField]
	private ItemType _weaponType = ItemType.primaryWeapon;

	[SerializeField]
	private bool _prepareBeforeShoot;

	[SerializeField]
	private AnimationClip _idle;

	[SerializeField]
	private AnimationClip _prepare;

	[SerializeField]
	private AnimationClip _shoot;

	[SerializeField]
	private AnimationClip _reload;

	[SerializeField]
	private AnimationClip _extend;

	[SerializeField]
	private AnimationClip _retract;

	[SerializeField]
	private AnimationClip _aiming;

	[SerializeField]
	private AnimationClip _forward;

	[SerializeField]
	private AnimationClip _backward;

	[SerializeField]
	private AnimationClip _sprint;

	public PlayerAnimationController.SectionLayer Section => _section;

	public bool IncludeHead => _includeHead;

	public ItemType WeaponType => _weaponType;

	public bool PrepareBeforeShoot => _prepareBeforeShoot;

	public AnimationClip Idle => _idle;

	public AnimationClip Prepare => _prepare;

	public AnimationClip Shoot => _shoot;

	public AnimationClip Reload => _reload;

	public AnimationClip Extend => _extend;

	public AnimationClip Retract => _retract;

	public AnimationClip Aiming => _aiming;

	public AnimationClip Forward => _forward;

	public AnimationClip Backward => _backward;

	public AnimationClip Sprint => _sprint;
}
