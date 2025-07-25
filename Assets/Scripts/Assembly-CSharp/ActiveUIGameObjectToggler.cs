using UnityEngine;

public class ActiveUIGameObjectToggler : ActiveUIStateChangedBase
{
	[SerializeField]
	private GameObject[] _objectsToToggle = new GameObject[0];

	[SerializeField]
	private bool _disableWhenActiveUIActive = true;

	protected override void OnActiveUIShown()
	{
		GameObject[] objectsToToggle = _objectsToToggle;
		for (int i = 0; i < objectsToToggle.Length; i++)
		{
			objectsToToggle[i].SetActive(!_disableWhenActiveUIActive);
		}
	}

	protected override void OnActiveUIHidden()
	{
		GameObject[] objectsToToggle = _objectsToToggle;
		for (int i = 0; i < objectsToToggle.Length; i++)
		{
			objectsToToggle[i].SetActive(_disableWhenActiveUIActive);
		}
	}
}
