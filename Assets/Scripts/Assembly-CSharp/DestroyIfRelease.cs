using UnityEngine;

public class DestroyIfRelease : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
