namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
	[TaskCategory("Unity/Math")]
	[TaskDescription("Performs comparison between two integers: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
	public class IntComparison : Conditional
	{
		public enum Operation
		{
			LessThan = 0,
			LessThanOrEqualTo = 1,
			EqualTo = 2,
			NotEqualTo = 3,
			GreaterThanOrEqualTo = 4,
			GreaterThan = 5
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first integer")]
		public SharedInt integer1;

		[Tooltip("The second integer")]
		public SharedInt integer2;

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.LessThan:
				if (integer1.Value >= integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			case Operation.LessThanOrEqualTo:
				if (integer1.Value > integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			case Operation.EqualTo:
				if (integer1.Value != integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			case Operation.NotEqualTo:
				if (integer1.Value == integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			case Operation.GreaterThanOrEqualTo:
				if (integer1.Value < integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			case Operation.GreaterThan:
				if (integer1.Value <= integer2.Value)
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			default:
				return TaskStatus.Failure;
			}
		}

		public override void OnReset()
		{
			operation = Operation.LessThan;
			integer1.Value = 0;
			integer2.Value = 0;
		}
	}
}
