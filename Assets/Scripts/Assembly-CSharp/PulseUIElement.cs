using DG.Tweening;
using UnityEngine;

public class PulseUIElement : MonoBehaviour
{
	[SerializeField]
	private float _tweenTime;

	[SerializeField]
	private float _minScale;

	private void Start()
	{
		base.transform.DOScale(_minScale, _tweenTime).SetLoops(-1, LoopType.Yoyo);
	}
}
