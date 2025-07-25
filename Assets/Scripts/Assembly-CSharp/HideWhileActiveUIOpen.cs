using BSCore;
using UnityEngine;
using Zenject;

public class HideWhileActiveUIOpen : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _objectsToHide = new GameObject[0];

	[Inject]
	private void Construct()
	{
		ActiveUI.Manager.ActiveUIToggled += OnActiveUIToggled;
		SetGameObjectsActiveState();
	}

	protected virtual void OnDestroy()
	{
		ActiveUI.Manager.ActiveUIToggled -= OnActiveUIToggled;
	}

	private void OnActiveUIToggled(ActiveUI activeUI, bool isActive)
	{
		SetGameObjectsActiveState();
	}

	protected virtual void SetGameObjectsActiveState()
	{
		GameObject[] objectsToHide = _objectsToHide;
		for (int i = 0; i < objectsToHide.Length; i++)
		{
			objectsToHide[i].SetActive(!ActiveUI.Manager.IsActiveUIShown);
		}
	}
}
