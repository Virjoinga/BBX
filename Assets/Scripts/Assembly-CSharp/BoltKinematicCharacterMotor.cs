using KinematicCharacterController;
using UnityEngine;

public class BoltKinematicCharacterMotor : KinematicCharacterMotor
{
	public enum CharacterSystemInterpolationMethod
	{
		None = 0,
		Unity = 1,
		Custom = 2
	}

	private float _lastCustomInterpolationStartTime = -1f;

	private float _lastCustomInterpolationDeltaTime = -1f;

	[SerializeField]
	private CharacterSystemInterpolationMethod _internalInterpolationMethod;

	public CharacterSystemInterpolationMethod InterpolationMethod
	{
		get
		{
			return _internalInterpolationMethod;
		}
		set
		{
			_internalInterpolationMethod = value;
			MoveActorsToDestination();
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
		{
			CustomInterpolationUpdate();
		}
	}

	public void PerformFullSimulation(float deltaTime)
	{
		PreSimulationUpdate(deltaTime);
		Simulate(deltaTime);
		PostSimulationUpdate(deltaTime);
	}

	public void PreSimulationUpdate(float deltaTime)
	{
		if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
		{
			MoveActorsToDestination();
		}
		InitialTickPosition = base.Transform.position;
		InitialTickRotation = base.Transform.rotation;
	}

	public void Simulate(float deltaTime)
	{
		UpdatePhase1(deltaTime);
		UpdatePhase2(deltaTime);
		base.Transform.SetPositionAndRotation(base.TransientPosition, base.TransientRotation);
		(CharacterController as IBoltKinematicCharacterController).AfterPositionRotationUpdate(deltaTime);
	}

	public void PostSimulationUpdate(float deltaTime)
	{
		Physics.SyncTransforms();
		if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
		{
			_lastCustomInterpolationStartTime = Time.time;
			_lastCustomInterpolationDeltaTime = deltaTime;
		}
		InitialTickPosition = base.TransientPosition;
		InitialTickRotation = base.TransientRotation;
		base.Transform.SetPositionAndRotation(base.TransientPosition, base.TransientRotation);
	}

	private void MoveActorsToDestination()
	{
		base.Transform.SetPositionAndRotation(base.TransientPosition, base.TransientRotation);
	}

	private void CustomInterpolationUpdate()
	{
		float t = Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);
		base.Transform.SetPositionAndRotation(Vector3.Lerp(InitialTickPosition, base.TransientPosition, t), Quaternion.Slerp(InitialTickRotation, base.TransientRotation, t));
	}
}
