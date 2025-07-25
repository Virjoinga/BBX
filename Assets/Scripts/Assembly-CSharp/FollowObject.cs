using UnityEngine;

public class FollowObject : MonoBehaviour
{
	[SerializeField]
	private bool _shouldFollow = true;

	[SerializeField]
	private Transform _target;

	private void FixedUpdate()
	{
		if (_shouldFollow)
		{
			base.transform.position = _target.position;
		}
	}
}
