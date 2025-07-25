using UnityEngine;

public class StretchLineRendererToObstacle : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private float _radius = 0.1f;

	[SerializeField]
	private float _baseOffset;

	private FiringWeapon _weapon;

	private void Reset()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.positionCount = 2;
	}

	private void Awake()
	{
		_weapon = GetComponentInParent<FiringWeapon>();
	}

	protected virtual void LateUpdate()
	{
		if (_weapon.Profile != null)
		{
			float num = _weapon.Profile.Range;
			if (num == 0f)
			{
				num = 200f;
			}
			if (!Physics.SphereCast(base.transform.position, _radius, base.transform.forward, out var hitInfo, num, LayerMaskConfig.HitableLayers))
			{
				hitInfo.point = base.transform.position + base.transform.forward * num;
			}
			OnStretchDetermined(hitInfo.point);
		}
	}

	protected virtual void OnStretchDetermined(Vector3 hitPoint)
	{
		_lineRenderer.SetPosition(0, base.transform.position + base.transform.forward * _baseOffset);
		_lineRenderer.SetPosition(1, hitPoint);
	}
}
