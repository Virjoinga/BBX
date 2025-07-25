using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[SerializeField]
	private Transform _destination;

	[SerializeField]
	private bool _forceRotation;

	[SerializeField]
	private ParticleSystem _baseEffects;

	[SerializeField]
	private ParticleSystem _destinationEffects;

	private void Awake()
	{
		_baseEffects.Stop();
		_destinationEffects.Stop();
	}

	private void OnTriggerEnter(Collider other)
	{
		IPlayerController component = other.GetComponent<IPlayerController>();
		if (component != null)
		{
			if (component.entity == null || component.entity.isOwner || component.entity.isControlled)
			{
				Quaternion rotation = (_forceRotation ? _destination.rotation : component.transform.rotation);
				component.SetPosition(_destination.position, rotation);
			}
			_baseEffects.Stop();
			_baseEffects.Play();
			_destinationEffects.Stop();
			_destinationEffects.Play();
		}
	}
}
