using Bolt;
using Constants;

public class PlayerStatusEffectBridge : EntityBehaviour<IPlayerState>, IStatusAffectable
{
	public Match.StatusType StatusFlags
	{
		get
		{
			return (Match.StatusType)base.state.StatusFlags;
		}
		set
		{
			base.state.StatusFlags = (int)value;
		}
	}

	public bool CanGetStatusApplied => base.state.Damageable.Health > 0f;

	public bool Stunned
	{
		get
		{
			return base.state.Stunned;
		}
		set
		{
			base.state.Stunned = value;
		}
	}

	public bool ForcedMovement
	{
		get
		{
			return base.state.Movable.ForcedMovement;
		}
		set
		{
			base.state.Movable.ForcedMovement = value;
		}
	}

	public float SpeedIncrease
	{
		get
		{
			return base.state.Movable.SpeedIncrease;
		}
		set
		{
			base.state.Movable.SpeedIncrease = value;
		}
	}

	public float SpeedDecrease
	{
		get
		{
			return base.state.Movable.SpeedDecrease;
		}
		set
		{
			base.state.Movable.SpeedDecrease = value;
		}
	}

	public float DamageModifier
	{
		get
		{
			return base.state.DamageModifier;
		}
		set
		{
			base.state.DamageModifier = value;
		}
	}

	public float MeleeDamageModifier
	{
		get
		{
			return base.state.MeleeDamageModifier;
		}
		set
		{
			base.state.MeleeDamageModifier = value;
		}
	}

	public float DamageShield
	{
		get
		{
			return base.state.Damageable.Shield;
		}
		set
		{
			base.state.Damageable.Shield = value;
		}
	}

	public float Size
	{
		get
		{
			return base.state.Size;
		}
		set
		{
			base.state.Size = value;
		}
	}

	public bool PreventJump
	{
		get
		{
			return base.state.Movable.PreventJump;
		}
		set
		{
			base.state.Movable.PreventJump = value;
		}
	}
}
