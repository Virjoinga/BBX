using UnityEngine;

public class ServerRunningCheck : MonoBehaviour
{
	private float _totalTimer;

	private float _sendLogTimer;

	private void Update()
	{
		_totalTimer += Time.deltaTime;
		_sendLogTimer += Time.deltaTime;
		if (_sendLogTimer >= 5f)
		{
			Debug.Log($"[Server Running Check] - {_totalTimer}");
			_sendLogTimer = 0f;
		}
	}

	private void OnDisable()
	{
		Debug.Log($"[Server Running Check] Disabled at - {_totalTimer}");
	}

	private void OnDestroy()
	{
		Debug.Log($"[Server Running Check] Destroyed at - {_totalTimer}");
	}
}
