using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AlphaButton : MonoBehaviour
{
	[SerializeField]
	private float _alphaThreshold = 0.1f;

	[SerializeField]
	private Button _button;

	private void Start()
	{
		_button.image.alphaHitTestMinimumThreshold = _alphaThreshold;
	}

	private void Reset()
	{
		_button = GetComponent<Button>();
	}
}
