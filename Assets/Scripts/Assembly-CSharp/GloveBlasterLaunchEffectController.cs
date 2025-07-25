using UnityEngine;

public class GloveBlasterLaunchEffectController : MonoBehaviour, IRaycastEffect
{
	[SerializeField]
	private Transform[] _balls;

	[SerializeField]
	private float _travelSpeed = 20f;

	private Vector3 _startPosition;

	private Vector3 _endPosition;

	private void Reset()
	{
		_balls = new Transform[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			_balls[i] = base.transform.GetChild(i);
		}
	}

	private void OnEnable()
	{
		_balls[0].gameObject.SetActive(value: true);
		_balls[1].gameObject.SetActive(value: false);
		_balls[2].gameObject.SetActive(value: false);
	}

	private void Update()
	{
		Transform[] balls = _balls;
		foreach (Transform ballState in balls)
		{
			SetBallState(ballState);
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, _endPosition, _travelSpeed * Time.deltaTime);
		if (base.transform.position == _endPosition)
		{
			SmartPool.Despawn(base.gameObject);
		}
	}

	private void SetBallState(Transform ball)
	{
		if (Vector3.Distance(ball.position, _startPosition) >= 0.5f)
		{
			ball.gameObject.SetActive(value: true);
		}
		if (Vector3.Distance(ball.position, _endPosition) <= 0.25f)
		{
			ball.gameObject.SetActive(value: false);
		}
	}

	public void Display(Vector3 endPosition, float forwardVelocity)
	{
		_startPosition = base.transform.position;
		_endPosition = endPosition + base.transform.forward;
	}
}
