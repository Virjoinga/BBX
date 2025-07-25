using System.Collections;
using UnityEngine;

public class LeftHandMeleeWeaponPositioner : MonoBehaviour
{
	[SerializeField]
	private Transform _leftHandWeapon;

	private Transform _leftHandContainer;

	private IEnumerator Start()
	{
		Outfit componentInParent;
		do
		{
			yield return null;
			componentInParent = GetComponentInParent<Outfit>();
		}
		while (componentInParent == null);
		_leftHandContainer = componentInParent.LeftHandContainer;
	}

	private void LateUpdate()
	{
		if (!(_leftHandContainer == null))
		{
			_leftHandWeapon.position = _leftHandContainer.position;
			_leftHandWeapon.rotation = _leftHandContainer.rotation;
		}
	}
}
