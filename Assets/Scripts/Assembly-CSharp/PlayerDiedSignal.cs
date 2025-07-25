public struct PlayerDiedSignal
{
	public PlayerController victim;

	public BoltEntity attacker;

	public PlayerDiedSignal(PlayerController player, BoltEntity attacker)
	{
		victim = player;
		this.attacker = attacker;
	}
}
