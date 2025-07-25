using UnityEngine;

public class AbilityFiringWeapon : MonoBehaviour
{
	private Transform _emenationPoint;

	private FiringWeapon _firingWeapon;

	private void Awake()
	{
		_firingWeapon = GetComponent<FiringWeapon>();
	}

	private void Start()
	{
		_emenationPoint = GetComponentInParent<Outfit>().AbilityEmenationPoint;
	}

	private void LateUpdate()
	{
		if (_emenationPoint != null)
		{
			_firingWeapon.LaunchPoint.transform.position = _emenationPoint.position;
		}
	}
}
