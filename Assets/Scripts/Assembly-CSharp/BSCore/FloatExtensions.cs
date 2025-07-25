namespace BSCore
{
	public static class FloatExtensions
	{
		public static float WrapAngle(this float value)
		{
			return value.WrapAround(180f);
		}

		public static float WrapAround(this float value, float limit)
		{
			if (value == 0f || limit == 0f)
			{
				return value;
			}
			while (value > limit)
			{
				value -= limit * 2f;
			}
			while (value < 0f - limit)
			{
				value += limit * 2f;
			}
			return value;
		}
	}
}
