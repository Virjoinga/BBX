using UnityEngine;

public class ZombieHuggableMeleeWeapon : MeleeWeapon
{
	[SerializeField]
	private Transform _leftHandCollider;

	private Transform _leftHand;

	protected override void Start()
	{
		ZombieHuggableWeaponContainer componentInParent = GetComponentInParent<ZombieHuggableWeaponContainer>();
		if (componentInParent != null)
		{
			_leftHand = componentInParent.LeftHand;
		}
		base.Start();
	}

	protected override void Update()
	{
		if (_leftHand != null)
		{
			_leftHandCollider.position = _leftHand.position;
			_leftHandCollider.rotation = _leftHand.rotation;
		}
		base.Update();
	}
}
