public class HealthPickupsBySizeData
{
	public HealthPickupData Small;

	public HealthPickupData Large;

	public HealthPickupsBySizeData(HealthPickupsBySizeConfigData data)
	{
		Small = new HealthPickupData(data.small);
		Large = new HealthPickupData(data.large);
	}
}
