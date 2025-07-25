public class PickupData
{
	public string PrefabPath;

	public float Cooldown;

	public int Chance;

	public PickupData(PickupConfigData data)
	{
		PrefabPath = data.prefabPath;
		Cooldown = data.cooldown;
		Chance = data.chance;
	}
}
