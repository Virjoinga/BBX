using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public class PrefabLauncher : MonoBehaviour
	{
		public Rigidbody ToLaunch;

		public float Force;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Rigidbody rigidbody = Object.Instantiate(ToLaunch, base.transform.position, base.transform.rotation);
				rigidbody.AddForce(base.transform.forward * Force, ForceMode.VelocityChange);
				Object.Destroy(rigidbody.gameObject, 8f);
			}
		}
	}
}
