using UnityEngine;

public class InterpolatedTransformUpdater : MonoBehaviour
{
	private InterpolatedTransform m_interpolatedTransform;

	private void Start()
	{
		m_interpolatedTransform = GetComponent<InterpolatedTransform>();
	}

	private void FixedUpdate()
	{
		if (m_interpolatedTransform != null)
		{
			m_interpolatedTransform.LateFixedUpdate();
		}
	}
}
