using UnityEngine;

public class TryDestroyUIPrefabOnAwake : MonoBehaviour
{
	[SerializeField]
	private UIPrefabIds _uiPrefabId;

	private void Awake()
	{
		UIPrefabManager.Destroy(_uiPrefabId);
	}
}
