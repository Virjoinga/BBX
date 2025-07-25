public struct DamagableDamagedSignal
{
	public BoltEntity Attacker;

	public BoltEntity Victim;

	public float Damage;

	public DamagableDamagedSignal(BoltEntity attacker, BoltEntity victim, float damage)
	{
		Attacker = attacker;
		Victim = victim;
		Damage = damage;
	}
}
