using BSCore;
using Cinemachine;
using UnityEngine;
using Zenject;

public class VirtualCameraFieldOfViewSettingApplier : MonoBehaviour
{
	[Inject(Id = DataStoreKeys.FieldOfView)]
	private DataStoreFloat _fieldOfViewStore;

	private CinemachineVirtualCamera _virtualCamera;

	[Inject]
	private void PostConstruct()
	{
		_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		_fieldOfViewStore.Changed += OnFieldOfViewChanged;
		OnFieldOfViewChanged(_fieldOfViewStore.Value);
	}

	private void OnFieldOfViewChanged(float newValue)
	{
		_virtualCamera.m_Lens.FieldOfView = newValue;
	}
}
