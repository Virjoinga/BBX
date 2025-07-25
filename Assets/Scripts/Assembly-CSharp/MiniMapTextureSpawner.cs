using UnityEngine;
using UnityEngine.UI;

public class MiniMapTextureSpawner : MonoBehaviour
{
	[SerializeField]
	private RawImage _image;

	private void Start()
	{
		RenderTexture texture = Resources.Load<RenderTexture>("MiniMapRenderTexture");
		_image.texture = texture;
	}
}
