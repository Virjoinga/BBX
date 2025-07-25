using DG.Tweening;
using UnityEngine;

public class BRVictoryScreen : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _swishClip;

	[SerializeField]
	private RectTransform _plateshadowImage;

	[SerializeField]
	private RectTransform _plateImage;

	[SerializeField]
	private RectTransform _bearImage;

	[SerializeField]
	private float _slideInTime = 0.3f;

	private void Start()
	{
		_plateshadowImage.DOAnchorPosX(0f, _slideInTime).OnComplete(delegate
		{
			_audioSource.PlayOneShot(_swishClip);
			_plateImage.DOAnchorPosX(0f, _slideInTime).OnComplete(delegate
			{
				_audioSource.PlayOneShot(_swishClip);
				_bearImage.DOAnchorPosY(0f, _slideInTime).OnComplete(delegate
				{
					_audioSource.PlayOneShot(_swishClip);
				});
			});
		});
	}
}
