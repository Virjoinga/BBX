public class HealthPickupData : PickupData
{
	public float Value;

	public HealthPickupData(HealthPickupConfigData data)
		: base(data)
	{
		Value = data.value;
	}
}
