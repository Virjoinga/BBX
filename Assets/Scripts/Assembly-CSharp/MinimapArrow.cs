using UnityEngine;
using UnityEngine.UI;

public class MinimapArrow : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Color _teammateColor;

	[SerializeField]
	private RectTransform _scalableRoot;

	public void SetAsTeammate()
	{
		_image.color = _teammateColor;
		_scalableRoot.localScale = new Vector3(0.75f, 0.75f, 0.75f);
	}
}
