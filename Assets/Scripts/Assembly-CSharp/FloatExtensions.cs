using UnityEngine;

public static class FloatExtensions
{
	public static float NormalizeAngle(this float value, float min, float max)
	{
		while (value > max)
		{
			value -= 360f;
		}
		while (value < min)
		{
			value += 360f;
		}
		return value;
	}

	public static float NormalizeAngle(this float value)
	{
		return value.NormalizeAngle(-180f, 180f);
	}

	public static Vector3 WrapEulerAngles(this Vector3 vector)
	{
		vector.x = vector.x.NormalizeAngle();
		vector.y = vector.y.NormalizeAngle();
		vector.z = vector.z.NormalizeAngle();
		return vector;
	}
}
