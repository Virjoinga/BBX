public struct ClientPlayerDiedSignal
{
	public string AttackerName;

	public HeroClass AttackerHeroClass;

	public TeamId AttackerTeam;

	public string VictimName;

	public HeroClass VictimHeroClass;

	public TeamId VictimTeam;

	public string WeaponId;

	public ClientPlayerDiedSignal(string attackerName, HeroClass attackerHeroClass, TeamId attackerTeam, string victimName, HeroClass victimHeroClass, TeamId victimTeam, string weaponId)
	{
		AttackerName = attackerName;
		AttackerHeroClass = attackerHeroClass;
		AttackerTeam = attackerTeam;
		VictimName = victimName;
		VictimHeroClass = victimHeroClass;
		VictimTeam = victimTeam;
		WeaponId = weaponId;
	}
}
