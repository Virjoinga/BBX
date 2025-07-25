using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;

public class DebugTDMPlayersController : MonoBehaviour
{
	[SerializeField]
	private DebugTDMPlayerPlate _playerPlatePrefab;

	[SerializeField]
	private Transform _container;

	private List<DebugTDMPlayerPlate> _playerPlates = new List<DebugTDMPlayerPlate>();

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

	public void AddPlayer(PlayerProfile playerProfile)
	{
		DebugTDMPlayerPlate debugTDMPlayerPlate = UnityEngine.Object.Instantiate(_playerPlatePrefab, _container);
		debugTDMPlayerPlate.Populate(playerProfile);
		debugTDMPlayerPlate.TeamSelected += OnPlayerSelectedTeam;
		_playerPlates.Add(debugTDMPlayerPlate);
	}

	public void RemovePlayer(string entityId)
	{
		DebugTDMPlayerPlate debugTDMPlayerPlate = _playerPlates.Where((DebugTDMPlayerPlate x) => x.EntityId == entityId).First();
		if (debugTDMPlayerPlate != null)
		{
			UnityEngine.Object.Destroy(debugTDMPlayerPlate.gameObject);
		}
	}

	private void OnPlayerSelectedTeam(string entityId, TeamId team)
	{
		this._teamSelected?.Invoke(entityId, team);
	}
}
