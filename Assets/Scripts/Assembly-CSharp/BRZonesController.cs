using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BRZonesController : MonoBehaviourSingleton<BRZonesController>
{
	[SerializeField]
	private List<BRZoneEntity> _zones = new List<BRZoneEntity>();

	private BattleRoyaleConfigData _configData;

	public static List<BRZoneEntity> GetNonOpenZones()
	{
		return MonoBehaviourSingleton<BRZonesController>.Instance._zones.Where((BRZoneEntity ze) => ze.Status != BRZoneStatus.Open).ToList();
	}

	protected override void Awake()
	{
		base.Awake();
		foreach (BRZoneEntity zone in _zones)
		{
			Object.Destroy(zone.gameObject);
		}
	}

	public void SetConfigData(BattleRoyaleConfigData configData)
	{
		_configData = configData;
	}

	public void StartZoneClosures()
	{
		StartCoroutine(ZoneClosureRoutine());
	}

	private IEnumerator ZoneClosureRoutine()
	{
		yield return new WaitForSeconds(_configData.ZoneCloseConfig.TimeToStartClosing);
		bool didCloseZone = true;
		while (didCloseZone)
		{
			didCloseZone = TryCloseZone();
			if (!didCloseZone)
			{
				Debug.Log("Closed All Zones");
				break;
			}
			yield return new WaitForSeconds(_configData.ZoneCloseConfig.ClosingIncrement);
		}
	}

	private bool TryCloseZone()
	{
		List<BRZoneEntity> list = _zones.Where((BRZoneEntity x) => x.Status == BRZoneStatus.Open).ToList();
		if (GetNonOpenZones().Count < 2)
		{
			list.RemoveAll((BRZoneEntity x) => x.Id == 7);
		}
		if (list == null || list.Count <= 1)
		{
			Debug.Log("Unable to close zone. All zones closed");
			return false;
		}
		List<BRZoneEntity> list2 = new List<BRZoneEntity>(list);
		bool flag = false;
		while (!flag)
		{
			BRZoneEntity bRZoneEntity = list2.Random();
			Debug.Log($"Trying to close zone {bRZoneEntity.Id}");
			list2.Remove(bRZoneEntity);
			list.Remove(bRZoneEntity);
			if (list2.Count <= 0 || CanCloseZone(list))
			{
				bRZoneEntity.CloseZone(_configData.ZoneCloseConfig.TimeToClose);
				flag = true;
				return true;
			}
			list.Add(bRZoneEntity);
			Debug.Log($"Unable to close zone {bRZoneEntity.Id}. Moving on");
		}
		return false;
	}

	private bool CanCloseZone(List<BRZoneEntity> openZones)
	{
		foreach (BRZoneEntity openZone in openZones)
		{
			List<BRZoneEntity> list = new List<BRZoneEntity>(openZones);
			list.Remove(openZone);
			foreach (BRZoneEntity item in list)
			{
				List<int> checkedZoneIds = new List<int> { openZone.Id };
				if (!openZone.IsConnectedToZone(item.Id, checkedZoneIds, openZones.Select((BRZoneEntity x) => x.Id).ToList()))
				{
					Debug.Log("Failed to connect two open zones. Invalid Zone Closure");
					return false;
				}
			}
		}
		return true;
	}
}
