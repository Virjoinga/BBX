using Constants;

public struct StatusFlagsupdatedSignal
{
	public Match.StatusType Flags;

	public StatusFlagsupdatedSignal(Match.StatusType statusFlags)
	{
		Flags = statusFlags;
	}
}
