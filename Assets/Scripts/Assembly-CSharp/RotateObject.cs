using UnityEngine;

public class RotateObject : MonoBehaviour
{
	[SerializeField]
	protected float _spinSpeed = 1f;

	[SerializeField]
	protected Vector3 _rotationDirection;

	public float SpinSpeed
	{
		get
		{
			return _spinSpeed;
		}
		set
		{
			_spinSpeed = value;
		}
	}

	public Vector3 RotationDirection
	{
		get
		{
			return _rotationDirection;
		}
		set
		{
			_rotationDirection = value;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		_rotationDirection.Normalize();
	}

	private void Update()
	{
		base.transform.Rotate(_rotationDirection * _spinSpeed);
	}
}
