using DG.Tweening;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private float _cameraStopZ = -55.7f;

	private bool _isFollowing;

	public void StartCharacterFollow()
	{
		base.transform.DOMove(_target.position, 0.5f).OnComplete(delegate
		{
			_isFollowing = true;
		});
	}

	private void FixedUpdate()
	{
		if (_isFollowing)
		{
			if (base.transform.position.z <= _cameraStopZ)
			{
				_isFollowing = false;
				return;
			}
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y, _target.position.z);
			base.transform.position = position;
		}
	}
}
