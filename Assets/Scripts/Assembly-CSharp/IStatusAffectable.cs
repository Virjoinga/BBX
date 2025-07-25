using Constants;

public interface IStatusAffectable
{
	BoltEntity entity { get; }

	Match.StatusType StatusFlags { get; set; }

	bool CanGetStatusApplied { get; }

	bool Stunned { get; set; }

	bool ForcedMovement { get; set; }

	float SpeedIncrease { get; set; }

	float SpeedDecrease { get; set; }

	float DamageModifier { get; set; }

	float MeleeDamageModifier { get; set; }

	float DamageShield { get; set; }

	float Size { get; set; }

	bool PreventJump { get; set; }
}
