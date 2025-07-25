using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BRMinimapZoneController : MonoBehaviourSingleton<BRMinimapZoneController>
{
	[SerializeField]
	private List<BRMinimapZone> _miniMapZones;

	private Dictionary<int, BRMinimapZone> _zonesById = new Dictionary<int, BRMinimapZone>();

	private void Reset()
	{
		_miniMapZones = GetComponentsInChildren<BRMinimapZone>().ToList();
	}

	protected override void Awake()
	{
		base.Awake();
		foreach (BRMinimapZone miniMapZone in _miniMapZones)
		{
			_zonesById.Add(miniMapZone.ZoneId, miniMapZone);
		}
	}

	public void ClosingZone(int zoneId)
	{
		if (_zonesById.ContainsKey(zoneId))
		{
			_zonesById[zoneId].ShowClosing();
		}
		else
		{
			Debug.LogError($"[BRMinimapZoneController] Zone {zoneId} not setup correctly");
		}
	}

	public void ClosedZone(int zoneId)
	{
		if (_zonesById.ContainsKey(zoneId))
		{
			_zonesById[zoneId].ShowClosed();
		}
		else
		{
			Debug.LogError($"[BRMinimapZoneController] Zone {zoneId} not setup correctly");
		}
	}
}
