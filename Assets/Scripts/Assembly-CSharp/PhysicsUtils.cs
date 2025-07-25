using System.Collections.Generic;
using UnityEngine;

public static class PhysicsUtils
{
	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		RaycastHit hit;
		return TrajectoryCast(start, initialVelocity, castRadius, castheight, out hit, timeStep, maxTime, debug);
	}

	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, out RaycastHit hit, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		Vector3[] points;
		return TrajectoryCast(start, initialVelocity, castRadius, castheight, out hit, out points, timeStep, maxTime, debug);
	}

	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, out Vector3[] points, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		RaycastHit hit;
		return TrajectoryCast(start, initialVelocity, castRadius, castheight, out hit, out points, -1, timeStep, maxTime, debug);
	}

	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, out RaycastHit hit, out Vector3[] points, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		return TrajectoryCast(start, initialVelocity, castRadius, castheight, out hit, out points, -1, timeStep, maxTime, debug);
	}

	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, out RaycastHit hit, out Vector3[] points, LayerMask ignoreLayer, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		return TrajectoryCast(start, initialVelocity, castRadius, castheight, out hit, out points, ignoreLayer, Physics.gravity, timeStep, maxTime, debug);
	}

	public static bool TrajectoryCast(Vector3 start, Vector3 initialVelocity, float castRadius, float castheight, out RaycastHit hit, out Vector3[] points, LayerMask ignoreLayer, Vector3 gravityVector, float timeStep = 0.1f, float maxTime = 10f, bool debug = false)
	{
		if (timeStep == 0f)
		{
			timeStep = 0.1f;
		}
		float num = 0f;
		bool flag = false;
		hit = default(RaycastHit);
		List<Vector3> list = new List<Vector3> { start };
		Vector3 vector = start;
		Vector3 vector2 = Vector3.zero;
		float num2 = castheight * 0.25f;
		Vector3 vector3 = Vector3.up * num2;
		Vector3 vector4 = Vector3.up * num2 * 3f;
		float num3 = timeStep;
		int num4 = Mathf.FloorToInt(maxTime / timeStep);
		for (int i = 0; i < num4; i++)
		{
			Vector3 vector5 = PlotTrajectoryAtTime(start, initialVelocity, gravityVector, num3);
			Vector3 direction = vector5 - vector;
			float maxDistance = Vector3.Distance(vector, vector5);
			if (Physics.CapsuleCast(vector + vector3, vector + vector4, castRadius, direction, out hit, maxDistance, ignoreLayer))
			{
				flag = true;
				Vector3 vector6 = vector + direction.normalized * hit.distance;
				num += hit.distance;
				list.Add(vector6);
				if (debug)
				{
					DebugExtension.DebugPoint(hit.point, Color.red, 0.25f);
					DebugExtension.DebugPoint(vector6, Color.magenta, 0.25f);
					Debug.DrawLine(vector, vector6, Color.red);
				}
				break;
			}
			num += Vector3.Distance(vector, vector5);
			list.Add(vector5);
			if (num >= 20f)
			{
				vector2 = vector5;
			}
			if (debug)
			{
				Debug.DrawLine(vector, vector5, Color.cyan);
			}
			vector = vector5;
			num3 += timeStep;
			if (debug)
			{
				DebugExtension.DebugPoint(vector5, Color.yellow, 0.125f);
			}
		}
		if (flag)
		{
			points = list.ToArray();
		}
		else
		{
			points = new Vector3[1] { vector2 };
		}
		if (debug)
		{
			_ = ref points[points.Length - 1];
			_ = points[points.Length - 1] + vector4 + vector3;
		}
		return flag;
	}

	private static Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, Vector3 gravityVector, float time)
	{
		return start + startVelocity * time + gravityVector * time * time * 0.5f;
	}

	public static float AngleOfReach(Vector3 start, Vector3 end, float velocity)
	{
		Vector3 a = start;
		a.y = 0f;
		Vector3 b = end;
		b.y = 0f;
		float x = Vector3.Distance(a, b);
		float y = end.y - start.y;
		return AngleOfReach(velocity, y, x);
	}

	public static float AngleOfReach(float v, float y, float x)
	{
		float num = v * v;
		return Mathf.Atan2(x: Physics.gravity.magnitude * x, y: num - Mathf.Sqrt(CalculateDelta(v, x, y))) * 57.29578f;
	}

	public static float CalculateDelta(float v, float x, float y)
	{
		float num = v * v;
		float num2 = num * num;
		float num3 = x * x;
		float magnitude = Physics.gravity.magnitude;
		return num2 - magnitude * (magnitude * num3 + 2f * y * num);
	}
}
