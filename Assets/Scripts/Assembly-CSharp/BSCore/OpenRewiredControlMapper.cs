using Rewired.UI.ControlMapper;
using UnityEngine;

namespace BSCore
{
	public class OpenRewiredControlMapper : UIBaseButtonClickHandler
	{
		[SerializeField]
		private ControlMapper _controlMapper;

		protected override void OnClick()
		{
			_controlMapper.Open();
		}
	}
}
