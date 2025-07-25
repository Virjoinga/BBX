using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Log is a simple task which will output the specified text and return success. It can be used for debugging.")]
	[TaskIcon("{SkinColor}LogIcon.png")]
	public class Log : Action
	{
		[Tooltip("Text to output to the log")]
		public SharedString text;

		[Tooltip("Is this text an error?")]
		public SharedBool logError;

		public override TaskStatus OnUpdate()
		{
			if (logError.Value)
			{
				Debug.LogError(text);
			}
			else
			{
				Debug.Log(text);
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			text = "";
			logError = false;
		}
	}
}
