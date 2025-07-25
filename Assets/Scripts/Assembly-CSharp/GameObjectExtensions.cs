using UnityEngine;

public static class GameObjectExtensions
{
	public static void SetLayerRecursively(this GameObject go, int newLayer)
	{
		if (go == null)
		{
			return;
		}
		go.layer = newLayer;
		foreach (Transform item in go.transform)
		{
			item.gameObject.SetLayerRecursively(newLayer);
		}
	}
}
