public struct ChargeValueUpdatedSignal
{
	public int index;

	public float chargeValue;

	public ChargeValueUpdatedSignal(int index, float chargeValue)
	{
		this.index = index;
		this.chargeValue = chargeValue;
	}
}
