using BSCore;
using TMPro;
using UnityEngine;

public class ShowServerId : MonoBehaviour
{
	[SerializeField]
	private ActiveUI _activeUI;

	[SerializeField]
	private TextMeshProUGUI _serverIdText;

	private void Awake()
	{
		_serverIdText.text = string.Empty;
		_activeUI.LocalActiveUIToggled += OnActiveUIToggled;
	}

	private void OnActiveUIToggled(bool isShown)
	{
		_serverIdText.text = string.Empty;
		if (isShown && !string.IsNullOrEmpty(ClientTeamDeathMatchGameModeEntity.SERVERID))
		{
			_serverIdText.text = ClientTeamDeathMatchGameModeEntity.SERVERID;
		}
	}
}
