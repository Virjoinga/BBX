using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitIndicator : MonoBehaviour
{
	[SerializeField]
	private RectTransform _rect;

	[SerializeField]
	private float _lifeTime = 1f;

	[SerializeField]
	private Image _image;

	public RectTransform Rect => _rect;

	private void OnEnable()
	{
		_image.DOFade(1f, 0f);
		StartCoroutine(DestroyAfterLifetimeRoutine());
	}

	private IEnumerator DestroyAfterLifetimeRoutine()
	{
		yield return new WaitForSeconds(_lifeTime);
		_image.DOFade(0f, 0.25f).OnComplete(delegate
		{
			SmartPool.Despawn(base.gameObject);
		});
	}
}
