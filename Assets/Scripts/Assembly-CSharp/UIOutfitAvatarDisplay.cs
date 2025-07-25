using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIOutfitAvatarDisplay : MonoBehaviour
{
	[SerializeField]
	private Image _outfitIcon;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
		_outfitIcon.sprite = PlayerController.LocalPlayer.LoadoutController.OutfitProfile.Icon;
	}
}
