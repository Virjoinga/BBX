using UnityEngine;

public class ZombieHuggableWeaponContainer : MonoBehaviour
{
	[SerializeField]
	private Transform _leftHand;

	public Transform LeftHand => _leftHand;
}
