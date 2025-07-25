using BSCore;
using BSCore.Constants.Config;
using UnityEngine;
using Zenject;

public class GroundHazardsEntity : BaseEntityEventListener<IGroundHazardsState>
{
	[Inject]
	private ConfigManager _configManager;

	[SerializeField]
	private GroundHazard[] _groundHazards;

	private void Reset()
	{
		_groundHazards = GetComponentsInChildren<GroundHazard>();
	}

	protected override void OnRemoteOnlyAttached()
	{
		Debug.Log("[GroundHazardsEntity] Disabling all ground hazard scripts");
		GroundHazard[] groundHazards = _groundHazards;
		for (int i = 0; i < groundHazards.Length; i++)
		{
			groundHazards[i].enabled = false;
		}
	}

	protected override void OnOwnerOnlyAttached()
	{
		Debug.Log("[GroundHazardsEntity] Enabling all ground hazard scripts");
		GroundHazardsData groundHazardsData = _configManager.Get<GroundHazardsData>(DataKeys.GroundHazards);
		GroundHazard[] groundHazards = _groundHazards;
		foreach (GroundHazard groundHazard in groundHazards)
		{
			if (groundHazardsData.TryGetGroundHazardById(groundHazard.Id, out var groundHazard2))
			{
				groundHazard.RegisterHazardData(groundHazard2, base.entity);
			}
			else
			{
				Debug.LogError("[GroundHazardEntity] Could not find hazard data for ground hazard " + groundHazard.Id);
			}
		}
	}
}
