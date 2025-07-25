using UnityEngine;

namespace BSCore
{
	public static class TransformExtensions
	{
		public static void DestroyChildren(this Transform root)
		{
			foreach (Transform item in root)
			{
				Object.Destroy(item.gameObject);
			}
		}

		public static void SetLayerOnAll(this Transform root, int layer)
		{
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
		}
	}
}
