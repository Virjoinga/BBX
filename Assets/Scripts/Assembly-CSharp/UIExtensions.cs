using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIExtensions
{
	public static bool IsInputFieldFocused()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			return false;
		}
		InputField component = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
		if (component != null && component.isFocused)
		{
			return true;
		}
		TMP_InputField component2 = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
		if (component2 != null && component2.isFocused)
		{
			return true;
		}
		return false;
	}
}
