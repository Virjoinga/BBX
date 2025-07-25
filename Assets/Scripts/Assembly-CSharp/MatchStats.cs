using System;

public struct MatchStats
{
	public GameModeType gameMode;

	public string map;

	public DateTime startedAt;

	public DateTime endedAt;

	public bool complete;

	public string weaponId;

	public bool battleWon;

	public int shotsTaken;

	public int kills;

	public int deaths;

	public int damageDealt;

	public int score;

	public string teamMembers;

	public string enemies;
}
