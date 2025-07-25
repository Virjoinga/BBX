using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapVariationManager : MonoBehaviourSingleton<MapVariationManager>
{
	[Serializable]
	public class MapVariationMapping
	{
		public int VariationId;

		public List<GameObject> VariationObjects;
	}

	[SerializeField]
	private List<MapVariationMapping> _mapVariationMappings;

	public int GetRandomMapVariation()
	{
		if (_mapVariationMappings == null || _mapVariationMappings.Count <= 0)
		{
			Debug.LogError("[MapVariationManager] Map Variations not setup!");
			return 0;
		}
		List<int> list = new List<int>();
		foreach (MapVariationMapping mapVariationMapping in _mapVariationMappings)
		{
			list.Add(mapVariationMapping.VariationId);
		}
		return list.Random();
	}

	public void ApplyMapVariation(int variationId)
	{
		DisableAllVariations();
		MapVariationMapping mapVariationMapping = _mapVariationMappings.FirstOrDefault((MapVariationMapping x) => x.VariationId == variationId);
		if (mapVariationMapping != null)
		{
			foreach (GameObject variationObject in mapVariationMapping.VariationObjects)
			{
				variationObject.SetActive(value: true);
			}
			return;
		}
		Debug.LogError($"[MapVariationManager] Unable to find map variant mapping with id {variationId}");
	}

	private void DisableAllVariations()
	{
		foreach (MapVariationMapping mapVariationMapping in _mapVariationMappings)
		{
			foreach (GameObject variationObject in mapVariationMapping.VariationObjects)
			{
				variationObject.SetActive(value: false);
			}
		}
	}
}
