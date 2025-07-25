using UnityEngine;

public class StretchLineRenderBetweenTransforms : MonoBehaviour
{
	[SerializeField]
	protected Transform[] _transforms;

	[SerializeField]
	private LineRenderer _lineRenderer;

	protected virtual void Reset()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.useWorldSpace = true;
	}

	protected virtual void Start()
	{
		_lineRenderer.positionCount = _transforms.Length;
	}

	private void Update()
	{
		for (int i = 0; i < _transforms.Length; i++)
		{
			_lineRenderer.SetPosition(i, _transforms[i].position);
		}
	}
}
