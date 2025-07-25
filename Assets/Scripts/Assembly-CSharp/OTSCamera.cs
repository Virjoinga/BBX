using BSCore;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class OTSCamera : MonoBehaviourSingleton<OTSCamera>
{
	[Inject(Id = DataStoreKeys.FieldOfView)]
	private DataStoreFloat _fieldOfViewStore;

	[SerializeField]
	private CinemachineVirtualCamera _virtualCamera;

	[SerializeField]
	private GameObject _root;

	[SerializeField]
	private OTSCameraCollider _otsCameraCollider;

	private float _baseFieldOfView = 40f;

	private bool _ironSightEnabled;

	private Tweener _zoomTween;

	private float _tweenTime = 0.25f;

	protected override void Awake()
	{
		base.Awake();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected override void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		base.OnDestroy();
	}

	[Inject]
	private void PostConstruct()
	{
		_fieldOfViewStore.Changed += OnFieldOfViewChanged;
		OnFieldOfViewChanged(_fieldOfViewStore.Value);
	}

	private void OnFieldOfViewChanged(float newValue)
	{
		_baseFieldOfView = newValue;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "MainMenu")
		{
			DestroySelf();
		}
	}

	public void SetTarget(AimPointHandler aimPointHandler)
	{
		_virtualCamera.Follow = aimPointHandler.transform;
		_virtualCamera.LookAt = aimPointHandler.transform;
		SetOffset(aimPointHandler);
	}

	public void SetOffset(AimPointHandler aimPointHandler)
	{
		Vector3 offset = aimPointHandler.Offset;
		CinemachineTransposer cinemachineComponent = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		offset.z = aimPointHandler.CameraDistanceOffset;
		_otsCameraCollider.MaxFollowOffset = Mathf.Abs(offset.z);
		cinemachineComponent.m_FollowOffset = offset;
		CinemachineComposer cinemachineComponent2 = _virtualCamera.GetCinemachineComponent<CinemachineComposer>();
		offset.z = 0f;
		cinemachineComponent2.m_TrackedObjectOffset = offset;
	}

	public void UnSetTarget(Transform followTarget)
	{
		if (_virtualCamera.Follow == followTarget)
		{
			_virtualCamera.Follow = null;
			_virtualCamera.LookAt = null;
		}
	}

	public void SetZoomLevel(float value)
	{
		if (_zoomTween != null && _zoomTween.IsActive())
		{
			_zoomTween.Kill();
		}
		float endValue = _baseFieldOfView * (1f / value);
		_zoomTween = DOTween.To(() => _virtualCamera.m_Lens.FieldOfView, delegate(float x)
		{
			_virtualCamera.m_Lens.FieldOfView = x;
		}, endValue, _tweenTime);
	}

	public void DestroySelf()
	{
		Object.Destroy(_root);
	}

	public void SetIronSightMode(bool enableIronSight, WeaponProfile weaponProfile)
	{
		if (!_ironSightEnabled && enableIronSight)
		{
			if (weaponProfile != null && weaponProfile.Zoom.CanZoom)
			{
				SetZoomLevel(weaponProfile.Zoom.Max);
			}
		}
		else if (_ironSightEnabled && !enableIronSight)
		{
			SetZoomLevel(1f);
		}
		_ironSightEnabled = enableIronSight;
	}
}
