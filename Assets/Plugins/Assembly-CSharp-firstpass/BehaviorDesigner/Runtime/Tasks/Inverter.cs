namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The inverter task will invert the return value of the child task after it has finished executing. If the child returns success, the inverter task will return failure. If the child returns failure, the inverter task will return success.")]
	[TaskIcon("{SkinColor}InverterIcon.png")]
	public class Inverter : Decorator
	{
		private TaskStatus executionStatus;

		public override bool CanExecute()
		{
			if (executionStatus != TaskStatus.Inactive)
			{
				return executionStatus == TaskStatus.Running;
			}
			return true;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			executionStatus = childStatus;
		}

		public override TaskStatus Decorate(TaskStatus status)
		{
			switch (status)
			{
			case TaskStatus.Success:
				return TaskStatus.Failure;
			case TaskStatus.Failure:
				return TaskStatus.Success;
			default:
				return status;
			}
		}

		public override void OnEnd()
		{
			executionStatus = TaskStatus.Inactive;
		}
	}
}
