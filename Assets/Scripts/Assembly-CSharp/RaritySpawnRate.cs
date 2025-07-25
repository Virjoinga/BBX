using System;

[Serializable]
public class RaritySpawnRate
{
	public string RarityString;

	public Rarity Rarity;

	public float SpawnChance;

	public void PopulateRarity()
	{
		Rarity = Rarity.Common;
		if (Enum<Rarity>.TryParse(RarityString, out var value))
		{
			Rarity = value;
		}
	}
}
