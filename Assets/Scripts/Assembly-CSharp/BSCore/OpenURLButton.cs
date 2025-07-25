using UnityEngine;

namespace BSCore
{
	public class OpenURLButton : UIBaseButtonClickHandler
	{
		[SerializeField]
		private string _url = string.Empty;

		protected override void OnClick()
		{
			Application.OpenURL(_url);
		}
	}
}
