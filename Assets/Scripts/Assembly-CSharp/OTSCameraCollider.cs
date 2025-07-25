using Cinemachine;
using UnityEngine;

public class OTSCameraCollider : CinemachineExtension
{
	private const float PrecisionSlush = 0.001f;

	[SerializeField]
	private float _minDistance;

	[SerializeField]
	private float _cameraWidth = 0.1f;

	[SerializeField]
	private float _dampingSpeed = 0.35f;

	private readonly float _offsetBuffer = 1f;

	private Vector3 _desiredDisplacement = Vector3.zero;

	public float MaxFollowOffset { get; set; } = 6f;

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (state.HasLookAt && stage == CinemachineCore.Stage.Body)
		{
			Vector3 correctedPosition = state.CorrectedPosition;
			Vector3 referenceLookAt = state.ReferenceLookAt;
			Vector3 direction = correctedPosition - referenceLookAt;
			Ray ray = new Ray(referenceLookAt, direction);
			Vector3 vector = Vector3.zero;
			float maxDistance = Vector3.Distance(base.transform.position, referenceLookAt);
			bool flag = true;
			if (Physics.SphereCast(ray, _cameraWidth, out var hitInfo, maxDistance, LayerMaskConfig.GroundLayers))
			{
				vector = ray.GetPoint(Mathf.Max(_minDistance, hitInfo.distance - 0.001f)) - correctedPosition;
				flag = false;
			}
			if (Vector3.Distance(Camera.main.transform.position, referenceLookAt) > MaxFollowOffset + _offsetBuffer)
			{
				flag = false;
			}
			if (flag)
			{
				_desiredDisplacement = Vector3.MoveTowards(_desiredDisplacement, vector, _dampingSpeed);
			}
			else
			{
				_desiredDisplacement = vector;
			}
			state.PositionCorrection += _desiredDisplacement;
		}
	}
}
