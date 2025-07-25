using UnityEngine;

public class SpawnUIPrefabOnAwake : MonoBehaviour
{
	[SerializeField]
	private UIPrefabIds _uiPrefabId;

	private void Awake()
	{
		UIPrefabManager.Instantiate(_uiPrefabId);
	}
}
