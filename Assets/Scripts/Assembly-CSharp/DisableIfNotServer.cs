using UnityEngine;

public class DisableIfNotServer : MonoBehaviour
{
	private void Awake()
	{
		if (!BoltNetwork.IsServer)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
