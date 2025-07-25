using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugShowCurrentAnimName : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private List<string> _animClipNames;

	private void Update()
	{
		if (_animator == null)
		{
			Outfit componentInChildren = GetComponentInChildren<Outfit>();
			if (componentInChildren != null)
			{
				_animator = componentInChildren.GetComponent<Animator>();
			}
		}
		if (!(_animator != null))
		{
			return;
		}
		_animClipNames.Clear();
		for (int i = 0; i < _animator.layerCount; i++)
		{
			_animClipNames.AddRange((from ci in _animator.GetCurrentAnimatorClipInfo(i)
				select ci.clip.name).ToList());
		}
	}
}
