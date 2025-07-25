using System;
using System.Collections.Generic;

[Serializable]
public class GroundHazardsData
{
	public GroundHazardProfileData[] GroundHazards;

	private Dictionary<string, GroundHazardData> _groundHazardDatasById;

	public bool TryGetGroundHazardById(string id, out GroundHazardData groundHazard)
	{
		if (_groundHazardDatasById == null)
		{
			_groundHazardDatasById = new Dictionary<string, GroundHazardData>();
			GroundHazardProfileData[] groundHazards = GroundHazards;
			foreach (GroundHazardProfileData groundHazardProfileData in groundHazards)
			{
				_groundHazardDatasById.Add(groundHazardProfileData.Id, new GroundHazardData(groundHazardProfileData));
			}
		}
		return _groundHazardDatasById.TryGetValue(id, out groundHazard);
	}
}
