namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
	[TaskCategory("Unity/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedGameObject : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedGameObject variable;

		[Tooltip("The variable to compare to")]
		public SharedGameObject compareTo;

		public override TaskStatus OnUpdate()
		{
			if (variable.Value == null && compareTo.Value != null)
			{
				return TaskStatus.Failure;
			}
			if (variable.Value == null && compareTo.Value == null)
			{
				return TaskStatus.Success;
			}
			if (!variable.Value.Equals(compareTo.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = null;
			compareTo = null;
		}
	}
}
