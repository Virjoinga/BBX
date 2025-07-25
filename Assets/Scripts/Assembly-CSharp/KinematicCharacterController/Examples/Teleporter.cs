using UnityEngine;
using UnityEngine.Events;

namespace KinematicCharacterController.Examples
{
	public class Teleporter : MonoBehaviour
	{
		public Teleporter TeleportTo;

		public UnityAction<ExampleCharacterController> OnCharacterTeleport;

		public bool isBeingTeleportedTo { get; set; }

		private void OnTriggerEnter(Collider other)
		{
			if (!isBeingTeleportedTo)
			{
				ExampleCharacterController component = other.GetComponent<ExampleCharacterController>();
				if ((bool)component)
				{
					component.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);
					if (OnCharacterTeleport != null)
					{
						OnCharacterTeleport(component);
					}
					TeleportTo.isBeingTeleportedTo = true;
				}
			}
			isBeingTeleportedTo = false;
		}
	}
}
