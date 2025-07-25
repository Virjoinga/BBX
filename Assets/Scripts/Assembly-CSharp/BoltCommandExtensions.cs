public static class BoltCommandExtensions
{
	public static string ToStr(this IPlayerInputCommandInput input)
	{
		return $"WeaponIndex: {input.WeaponIndex}, Position: {input.Position}, Forward: {input.Forward}, AmmoUsed: {input.AmmoUsed}";
	}

	public static string ToStr(this IPlayerInputCommandResult result)
	{
		return $"WeaponIndex: {result.WeaponIndex}, Fired: {result.Fired}, RemainingAmmo: {result.RemainingAmmo}, NextFireTime: {result.NextFireTime}, ReloadStartTime: {result.ReloadStartFrame}";
	}

	public static string ToStr(this FiredEvent evt)
	{
		string text = ((evt.Victim != null) ? evt.Victim.name : "null");
		return $"Position: {evt.Position}, Forward: {evt.Forward}, Victim: {text}, IsRayCast: {evt.IsRaycast}, TimeFired: {evt.TimeFired}, TimeRaised: {evt.TimeRaised}";
	}
}
