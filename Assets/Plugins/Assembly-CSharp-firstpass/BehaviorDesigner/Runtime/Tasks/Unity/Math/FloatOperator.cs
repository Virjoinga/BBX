using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
	[TaskCategory("Unity/Math")]
	[TaskDescription("Performs a math operation on two floats: Add, Subtract, Multiply, Divide, Min, or Max.")]
	public class FloatOperator : Action
	{
		public enum Operation
		{
			Add = 0,
			Subtract = 1,
			Multiply = 2,
			Divide = 3,
			Min = 4,
			Max = 5,
			Modulo = 6
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first float")]
		public SharedFloat float1;

		[Tooltip("The second float")]
		public SharedFloat float2;

		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.Add:
				storeResult.Value = float1.Value + float2.Value;
				break;
			case Operation.Subtract:
				storeResult.Value = float1.Value - float2.Value;
				break;
			case Operation.Multiply:
				storeResult.Value = float1.Value * float2.Value;
				break;
			case Operation.Divide:
				storeResult.Value = float1.Value / float2.Value;
				break;
			case Operation.Min:
				storeResult.Value = Mathf.Min(float1.Value, float2.Value);
				break;
			case Operation.Max:
				storeResult.Value = Mathf.Max(float1.Value, float2.Value);
				break;
			case Operation.Modulo:
				storeResult.Value = float1.Value % float2.Value;
				break;
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			operation = Operation.Add;
			float1.Value = 0f;
			float2.Value = 0f;
			storeResult.Value = 0f;
		}
	}
}
