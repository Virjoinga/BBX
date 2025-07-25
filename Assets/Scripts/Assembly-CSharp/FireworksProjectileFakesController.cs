using System.Collections;
using UnityEngine;

public class FireworksProjectileFakesController : MonoBehaviour
{
	[SerializeField]
	private LaunchPoint _launchPoint;

	[SerializeField]
	private Transform _leftInnerFake;

	[SerializeField]
	private Transform _leftOuterFake;

	[SerializeField]
	private Transform _rightInnerFake;

	[SerializeField]
	private Transform _rightOuterFake;

	[SerializeField]
	private Transform _fakeCenter;

	private float _step;

	private void Start()
	{
		_launchPoint.Fired += OnFired;
	}

	private void OnFired(Vector3 position, int muzzleIndex)
	{
		if (_step == 0f)
		{
			_step = 1f / _launchPoint.Profile.Cooldown * 2f;
		}
		switch (muzzleIndex)
		{
		case 3:
			MoveFakeToPosition(_rightOuterFake, _rightInnerFake.position);
			StartCoroutine(SlideFakeToHome(_rightOuterFake));
			MoveFakeToPosition(_rightInnerFake, _fakeCenter.position);
			StartCoroutine(SlideFakeToHome(_rightInnerFake));
			break;
		case 2:
			MoveFakeToPosition(_leftOuterFake, _leftInnerFake.position);
			StartCoroutine(SlideFakeToHome(_leftOuterFake));
			MoveFakeToPosition(_leftInnerFake, _fakeCenter.position);
			StartCoroutine(SlideFakeToHome(_leftInnerFake));
			break;
		case 1:
			MoveFakeToPosition(_rightInnerFake, _fakeCenter.position);
			StartCoroutine(SlideFakeToHome(_rightInnerFake));
			break;
		default:
			MoveFakeToPosition(_leftInnerFake, _fakeCenter.position);
			StartCoroutine(SlideFakeToHome(_leftInnerFake));
			break;
		}
	}

	private void MoveFakeToPosition(Transform fake, Vector3 position)
	{
		fake.position = position;
	}

	private IEnumerator SlideFakeToHome(Transform fake)
	{
		Vector3 start = fake.localPosition;
		Vector3 end = Vector3.zero;
		for (float t = 0f; t < 1f; t += Time.deltaTime * _step)
		{
			fake.localPosition = Vector3.Lerp(start, end, t);
			yield return null;
		}
	}
}
