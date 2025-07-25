using System.Collections;
using UnityEngine;

public class TestCharacterSelector : MonoBehaviour
{
	[SerializeField]
	private TestCharacterSelection[] _options;

	private void Start()
	{
		TestCharacterSelection[] options = _options;
		for (int i = 0; i < options.Length; i++)
		{
			options[i].Clicked += OnCharacterSelected;
		}
		StartCoroutine(SubscribeToLocalPlayer());
	}

	private void OnCharacterSelected(TestCharacterSelection selectedOption)
	{
		DisableAll();
	}

	private IEnumerator SubscribeToLocalPlayer()
	{
		DisableAll();
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
	}

	private void CharacterModelChanged(string characterId)
	{
		TestCharacterSelection[] options = _options;
		foreach (TestCharacterSelection obj in options)
		{
			obj.Interactable = obj.Character != characterId;
		}
	}

	private void DisableAll()
	{
		TestCharacterSelection[] options = _options;
		for (int i = 0; i < options.Length; i++)
		{
			options[i].Interactable = false;
		}
	}
}
