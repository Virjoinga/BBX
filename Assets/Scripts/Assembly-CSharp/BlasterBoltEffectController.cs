using UnityEngine;

public class BlasterBoltEffectController : MonoBehaviour, IRaycastEffect
{
	[SerializeField]
	private AudioSource _audio;

	[SerializeField]
	private LineRenderer _line;

	[SerializeField]
	private float _cycleSpeed = 4f;

	[SerializeField]
	private float _timeToLive = 0.6f;

	[SerializeField]
	private float _streakTime = 0.35f;

	[SerializeField]
	private float _fadeTime = 0.25f;

	private float _lifetime;

	private Vector3 _endPosition;

	private Color _fadeColor;

	private void Reset()
	{
		_line = GetComponent<LineRenderer>();
	}

	private void OnEnable()
	{
		_lifetime = 0f;
		_fadeColor = Color.white;
		if (!BoltNetwork.IsServer)
		{
			_audio.Stop();
			_audio.Play();
		}
	}

	private void Update()
	{
		_line.material.mainTextureOffset = new Vector2(0f, _lifetime * _cycleSpeed);
		SetStreakPosition();
		SetFadeLevel();
		_lifetime += Time.deltaTime;
		if (_lifetime >= _timeToLive)
		{
			SmartPool.Despawn(base.gameObject);
		}
	}

	private void SetStreakPosition()
	{
		if (_lifetime < _streakTime)
		{
			float t = _lifetime / _streakTime;
			_line.SetPosition(1, Vector3.Lerp(_line.GetPosition(0), _endPosition, t));
		}
	}

	private void SetFadeLevel()
	{
		if (_lifetime >= _streakTime)
		{
			float a = (_lifetime - _streakTime) / _fadeTime;
			_fadeColor.a = a;
			_line.material.SetColor("_TineColor", _fadeColor);
		}
	}

	public void Display(Vector3 endPosition, float forwardVelocity)
	{
		_line.SetPosition(0, base.transform.position);
		_line.SetPosition(1, base.transform.position);
		_endPosition = endPosition;
	}
}
