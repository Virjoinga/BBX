using TMPro;
using UnityEngine;

public class ChatLogItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	public void Init(string formattedMessage)
	{
		_text.text = formattedMessage;
	}
}
