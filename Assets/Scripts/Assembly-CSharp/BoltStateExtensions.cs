using System.Text;

public static class BoltStateExtensions
{
	public static string DebugState(this IPlayerState state)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine($"StrafeSpeed: {state.StrafeSpeed}");
		stringBuilder.AppendLine($"AmmoClips: {state.AmmoClips}");
		stringBuilder.AppendLine($"WeaponsDisabled: {state.WeaponsDisabled}");
		stringBuilder.AppendLine($"LastSpecialUse: {state.LastSpecialUse}");
		stringBuilder.AppendLine($"Team: {state.Team}");
		stringBuilder.AppendLine($"Stunned: {state.Stunned}");
		stringBuilder.AppendLine($"StatusFlags: {state.StatusFlags}");
		stringBuilder.AppendLine($"CanSecondLife: {state.CanSecondLife}");
		stringBuilder.AppendLine($"IsSecondLife: {state.IsSecondLife}");
		stringBuilder.AppendLine($"GameModeType: {state.GameModeType}");
		stringBuilder.AppendLine($"VerticalSpeed: {state.VerticalSpeed}");
		stringBuilder.AppendLine($"TurnSpeed: {state.TurnSpeed}");
		stringBuilder.AppendLine($"NextMeleeTime: {state.NextMeleeTime}");
		stringBuilder.AppendLine($"InputEnabled: {state.InputEnabled}");
		stringBuilder.AppendLine("DisplayName: " + state.DisplayName);
		stringBuilder.AppendLine($"Speed: {state.Speed}");
		stringBuilder.AppendLine($"Damageable.Health: {state.Damageable.Health}");
		stringBuilder.AppendLine($"Damageable.MaxHealth: {state.Damageable.MaxHealth}");
		stringBuilder.AppendLine($"Damageable.DamageReduction: {state.Damageable.DamageReduction}");
		stringBuilder.AppendLine($"Movable.IsGrounded: {state.Movable.IsGrounded}");
		stringBuilder.AppendLine($"Movable.Velocity: {state.Movable.Velocity}");
		stringBuilder.AppendLine($"Movable.SpeedIncrease: {state.Movable.SpeedIncrease}");
		stringBuilder.AppendLine($"Movable.SpeedDecrease: {state.Movable.SpeedDecrease}");
		stringBuilder.AppendLine("Loadouts");
		foreach (Loadout loadout in state.Loadouts)
		{
			stringBuilder.AppendLine("----------------");
			stringBuilder.AppendLine($"ActiveWeapon: {loadout.ActiveWeapon}");
			stringBuilder.AppendLine($"MeleeWeapon: {loadout.MeleeWeapon}");
			stringBuilder.AppendLine("Outfit: " + loadout.Outfit);
			stringBuilder.AppendLine("Hat: " + loadout.Hat);
			stringBuilder.AppendLine("Emote: " + loadout.Emote);
			stringBuilder.AppendLine("Weapons");
			foreach (Weapon weapon in loadout.Weapons)
			{
				stringBuilder.AppendLine("----------------");
				stringBuilder.AppendLine("Id: " + weapon.Id);
				stringBuilder.AppendLine($"RemainingAmmo: {weapon.RemainingAmmo}");
				stringBuilder.AppendLine($"MaxAmmo: {weapon.MaxAmmo}");
				stringBuilder.AppendLine($"NextFireFrame: {weapon.NextFireFrame}");
				stringBuilder.AppendLine($"ChargeValue: {weapon.ChargeValue}");
				stringBuilder.AppendLine($"IsReloading: {weapon.IsReloading}");
				stringBuilder.AppendLine($"ReloadStartFrame: {weapon.ReloadStartFrame}");
			}
		}
		return stringBuilder.ToString();
	}

	public static string DebugState(this ITeamDeathMatchGameModeState state)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Map: " + state.Map);
		stringBuilder.AppendLine($"MatchState: {state.MatchState}");
		stringBuilder.AppendLine($"MatchStartTime: {state.MatchStartTime}");
		stringBuilder.AppendLine("ServerId: " + state.ServerId);
		stringBuilder.AppendLine("Players");
		foreach (TDMPlayerState player in state.Players)
		{
			stringBuilder.AppendLine("----------------");
			stringBuilder.AppendLine("EntityId: " + player.EntityId);
			stringBuilder.AppendLine($"NetworkId: {player.NetworkId}");
			stringBuilder.AppendLine("DisplayName: " + player.DisplayName);
			stringBuilder.AppendLine($"Team: {player.Team}");
			stringBuilder.AppendLine($"Kills: {player.Kills}");
			stringBuilder.AppendLine($"Deaths: {player.Deaths}");
			stringBuilder.AppendLine("Loadout: " + player.Loadout);
			stringBuilder.AppendLine($"Platform: {player.Platform}");
			stringBuilder.AppendLine($"Disconnected: {player.Disconnected}");
		}
		return stringBuilder.ToString();
	}
}
