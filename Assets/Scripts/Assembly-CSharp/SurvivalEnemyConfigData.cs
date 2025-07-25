using System;

[Serializable]
public struct SurvivalEnemyConfigData
{
	public string Name;

	public float Speed;

	public float Acceleration;

	public float RotationSpeed;

	public float Damage;

	public float Health;

	public int StartWave;

	public int Percentage;

	public float PointModifier;

	public WeaponProfile.EffectData Effect;
}
