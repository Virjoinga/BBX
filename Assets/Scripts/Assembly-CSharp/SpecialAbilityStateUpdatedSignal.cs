public struct SpecialAbilityStateUpdatedSignal
{
	public bool CanUse;

	public float ChargePercent;

	public SpecialAbilityStateUpdatedSignal(bool canUse, float chargePercent)
	{
		CanUse = canUse;
		ChargePercent = chargePercent;
	}
}
