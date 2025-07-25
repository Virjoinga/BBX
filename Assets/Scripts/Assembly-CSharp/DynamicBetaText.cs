using BSCore;
using BSCore.Constants.Config;
using TMPro;
using UnityEngine;
using Zenject;

public class DynamicBetaText : MonoBehaviour
{
	[Inject]
	private ConfigManager _configManager;

	[SerializeField]
	private TextMeshProUGUI _messageText;

	private void Start()
	{
		_configManager.Fetched += UpdateDisplayText;
		UpdateDisplayText();
	}

	public void UpdateDisplayText()
	{
		string text = _configManager.Get(DataKeys.betaMessage, string.Empty);
		text = text.Replace("<return>", "\n");
		_messageText.text = text;
	}
}
