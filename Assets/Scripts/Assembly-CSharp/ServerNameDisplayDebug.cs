using UnityEngine;

public class ServerNameDisplayDebug : MonoBehaviour
{
	private string _serverName = string.Empty;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SetServerName(string serverName)
	{
		_serverName = serverName;
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(10f, 10f, 1000f, 50f), _serverName);
	}
}
