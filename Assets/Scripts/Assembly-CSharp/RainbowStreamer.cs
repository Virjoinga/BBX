using System.Collections;
using UnityEngine;

public class RainbowStreamer : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _line;

	[SerializeField]
	private float _timeToFullSize = 0.25f;

	[SerializeField]
	private float _cycleSpeed = 4f;

	private void Reset()
	{
		_line = GetComponent<LineRenderer>();
	}

	private void OnEnable()
	{
		StartCoroutine(GrowStreamer());
	}

	private void Update()
	{
		_line.material.mainTextureOffset = new Vector2(0f, Time.time * _cycleSpeed);
	}

	private IEnumerator GrowStreamer()
	{
		Vector3 start = new Vector3(0f, 0f, -0.25f);
		Vector3 end = new Vector3(0f, 0f, -1.5f);
		float step = 1f / _timeToFullSize;
		for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
		{
			_line.SetPosition(2, Vector3.Lerp(start, end, t));
			yield return null;
		}
	}
}
