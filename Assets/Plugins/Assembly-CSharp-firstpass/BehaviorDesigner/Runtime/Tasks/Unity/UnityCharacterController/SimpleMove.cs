using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
	[TaskCategory("Unity/CharacterController")]
	[TaskDescription("Moves the character with speed. Returns Success.")]
	public class SimpleMove : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The speed of the movement")]
		public SharedVector3 speed;

		private CharacterController characterController;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				characterController = defaultGameObject.GetComponent<CharacterController>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (characterController == null)
			{
				Debug.LogWarning("CharacterController is null");
				return TaskStatus.Failure;
			}
			characterController.SimpleMove(speed.Value);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			speed = Vector3.zero;
		}
	}
}
