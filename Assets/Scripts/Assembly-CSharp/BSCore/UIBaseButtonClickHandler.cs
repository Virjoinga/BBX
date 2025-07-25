using UnityEngine;
using UnityEngine.UI;

namespace BSCore
{
	public abstract class UIBaseButtonClickHandler : MonoBehaviour
	{
		[SerializeField]
		protected Button _button;

		protected virtual void Reset()
		{
			_button = GetComponentInChildren<Button>();
		}

		protected virtual void Start()
		{
			_button.onClick.AddListener(OnClick);
		}

		protected abstract void OnClick();
	}
}
