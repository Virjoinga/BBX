using UnityEngine;

public class BombardmentAimIndicator : MonoBehaviour
{
	[SerializeField]
	private Color _allyColor = Color.blue;

	[SerializeField]
	private Color _enemyColor = Color.red;

	private Renderer[] _renderers;

	private void Awake()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}

	public void Setup(IPlayerController playerController)
	{
		Color value = ((playerController.IsLocal || playerController.IsLocalPlayerTeammate) ? _allyColor : _enemyColor);
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_BaseColor", value);
			for (int j = 0; j < renderer.materials.Length; j++)
			{
				renderer.SetPropertyBlock(materialPropertyBlock, j);
			}
		}
	}
}
