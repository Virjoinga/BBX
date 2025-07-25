using UnityEngine;

public class JumpPad : MonoBehaviour
{
	[SerializeField]
	private Vector3 _jumpDirection = Vector3.up;

	[SerializeField]
	private float _timeStep = 0.1f;

	[SerializeField]
	private Transform _directionIndicator;

	[SerializeField]
	private bool _displayTrajectory;

	[SerializeField]
	private ParticleSystem _jumpEffect;

	[SerializeField]
	private AudioSource _audio;

	public Vector3 JumpDirection => base.transform.TransformDirection(_jumpDirection);

	private void Awake()
	{
		_jumpEffect.Stop();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			_jumpEffect.Stop();
			_jumpEffect.Play();
			_audio.Stop();
			_audio.Play();
		}
	}
}
