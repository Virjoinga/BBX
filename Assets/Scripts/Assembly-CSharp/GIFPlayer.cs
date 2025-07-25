using UnityEngine;
using UnityEngine.UI;

public class GIFPlayer : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Sprite[] _frames;

	[SerializeField]
	private int _framesPerSecond = 10;

	private void Update()
	{
		int num = (int)(Time.time * (float)_framesPerSecond) % _frames.Length;
		_image.sprite = _frames[num];
	}
}
