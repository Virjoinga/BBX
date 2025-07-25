using Constants;
using UnityEngine;

public class OfflineStatusAffectable : MonoBehaviour, IStatusAffectable
{
	public BoltEntity entity => null;

	public Match.StatusType StatusFlags { get; set; }

	public bool CanGetStatusApplied => true;

	public bool Stunned { get; set; }

	public bool ForcedMovement { get; set; }

	public float SpeedIncrease { get; set; }

	public float SpeedDecrease { get; set; }

	public float DamageModifier { get; set; }

	public float MeleeDamageModifier { get; set; }

	public float DamageShield { get; set; }

	public float Size { get; set; }

	public bool PreventJump { get; set; }
}
