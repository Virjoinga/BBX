using UnityEngine;

public class DisableIfServer : MonoBehaviour
{
	private void Awake()
	{
		if (BoltNetwork.IsServer)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
