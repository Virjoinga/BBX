using UnityEngine;

public struct FireResults
{
	public static readonly FireResults Empty;

	public bool Fired;

	public bool IsCharging;

	public float ChargeTime;

	public HitType HitType;

	public int LaunchFrame;

	public int AmmoUsed;

	public Vector3 Position;

	public Vector3 Forward;

	public override string ToString()
	{
		return $"[FireResults] Fired: {Fired}, IsCharging: {IsCharging}, HitType: {HitType}, LaunchFrame: {LaunchFrame}, AmmoUsed: {AmmoUsed}, Position: {Position}, Forward: {Forward}";
	}
}
