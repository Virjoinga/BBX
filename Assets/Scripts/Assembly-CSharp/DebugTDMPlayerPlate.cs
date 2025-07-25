using System;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugTDMPlayerPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _displayNameText;

	[SerializeField]
	private TextMeshProUGUI _idText;

	[SerializeField]
	private Button _team1Button;

	[SerializeField]
	private Button _team2Button;

	public string EntityId { get; private set; }

	private event Action<string, TeamId> _teamSelected;

	public event Action<string, TeamId> TeamSelected
	{
		add
		{
			_teamSelected += value;
		}
		remove
		{
			_teamSelected -= value;
		}
	}

	private void Awake()
	{
		_team1Button.onClick.AddListener(ClickedTeam1);
		_team2Button.onClick.AddListener(ClickedTeam2);
	}

	private void ClickedTeam1()
	{
		this._teamSelected?.Invoke(EntityId, TeamId.Team1);
		_team1Button.interactable = false;
		_team2Button.gameObject.SetActive(value: false);
	}

	private void ClickedTeam2()
	{
		this._teamSelected?.Invoke(EntityId, TeamId.Team2);
		_team2Button.interactable = false;
		_team1Button.gameObject.SetActive(value: false);
	}

	public void Populate(PlayerProfile playerProfile)
	{
		_displayNameText.text = playerProfile.DisplayName;
		EntityId = playerProfile.Entity.Id;
		_idText.text = EntityId;
	}
}
