using UnityEngine;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
	public class StressTestManager : MonoBehaviour
	{
		public Camera Camera;

		public LayerMask UIMask;

		public InputField CountField;

		public Image RenderOn;

		public Image SimOn;

		public Image InterpOn;

		public ExampleCharacterController CharacterPrefab;

		public ExampleAIController AIController;

		public int SpawnCount = 100;

		public float SpawnDistance = 2f;

		private void Start()
		{
			KinematicCharacterSystem.EnsureCreation();
			CountField.text = SpawnCount.ToString();
			UpdateOnImages();
		}

		private void UpdateOnImages()
		{
			RenderOn.enabled = Camera.cullingMask == -1;
			SimOn.enabled = Physics.autoSimulation;
			InterpOn.enabled = KinematicCharacterSystem.Settings.Interpolate;
		}

		public void SetSpawnCount(string count)
		{
			if (int.TryParse(count, out var result))
			{
				SpawnCount = result;
			}
		}

		public void ToggleRendering()
		{
			if (Camera.cullingMask == -1)
			{
				Camera.cullingMask = UIMask;
			}
			else
			{
				Camera.cullingMask = -1;
			}
			UpdateOnImages();
		}

		public void TogglePhysicsSim()
		{
			Physics.autoSimulation = !Physics.autoSimulation;
			UpdateOnImages();
		}

		public void ToggleInterpolation()
		{
			KinematicCharacterSystem.Settings.Interpolate = !KinematicCharacterSystem.Settings.Interpolate;
			UpdateOnImages();
		}

		public void Spawn()
		{
			for (int i = 0; i < AIController.Characters.Count; i++)
			{
				Object.Destroy(AIController.Characters[i].gameObject);
			}
			AIController.Characters.Clear();
			int num = Mathf.CeilToInt(Mathf.Sqrt(SpawnCount));
			Vector3 vector = (float)num * SpawnDistance * 0.5f * -Vector3.one;
			vector.y = 0f;
			for (int j = 0; j < SpawnCount; j++)
			{
				int num2 = j / num;
				int num3 = j % num;
				Vector3 position = vector + Vector3.right * num2 * SpawnDistance + Vector3.forward * num3 * SpawnDistance;
				ExampleCharacterController exampleCharacterController = Object.Instantiate(CharacterPrefab);
				exampleCharacterController.Motor.SetPosition(position);
				AIController.Characters.Add(exampleCharacterController);
			}
		}
	}
}
