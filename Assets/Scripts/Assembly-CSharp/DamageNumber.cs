using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	[ColorUsage(false)]
	private Color _damageColor = Color.red;

	[SerializeField]
	[ColorUsage(false)]
	private Color _healColor = Color.green;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private float _offset = 0.35f;

	[SerializeField]
	private float _screenPresenceMargin = 5f;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	private Transform _followTransform;

	public float Depth { get; private set; }

	public void SetText(float value, Transform followTransform)
	{
		_followTransform = followTransform;
		string text = Mathf.Abs(value).ToString("n0");
		if (value > 0f)
		{
			_text.text = "-" + text;
			_text.color = _damageColor;
		}
		else
		{
			_text.text = "+" + text;
			_text.color = _healColor;
		}
		int value2 = Random.Range(0, 4);
		_animator.SetInteger("RandomAnim", value2);
	}

	private void Update()
	{
		if (!(_followTransform == null))
		{
			UpdatePosition();
		}
	}

	private void UpdatePosition()
	{
		Vector3 vector = new Vector3(_followTransform.position.x, _followTransform.position.y + _offset, _followTransform.position.z);
		Vector3 vector2 = Camera.main.WorldToScreenPoint(vector);
		if (PointIsVisible(vector))
		{
			_canvasGroup.alpha = 1f;
			base.transform.position = new Vector3(vector2.x, vector2.y, vector2.z);
			float magnitude = (vector - Camera.main.transform.position).magnitude;
			Depth = 0f - magnitude;
		}
		else
		{
			_canvasGroup.alpha = 0f;
		}
	}

	private bool PointIsVisible(Vector3 worldPos)
	{
		Vector3 forward = Camera.main.transform.forward;
		Vector3 normalized = (worldPos - Camera.main.transform.position).normalized;
		return Vector3.Angle(forward, normalized) <= Camera.main.fieldOfView + _screenPresenceMargin;
	}
}
