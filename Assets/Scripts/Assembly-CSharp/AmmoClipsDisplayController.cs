using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AmmoClipsDisplayController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Image _chargeImage;

	[SerializeField]
	private TextMeshProUGUI _clipCount;

	private IEnumerator Start()
	{
		_signalBus.Subscribe<AmmoClipsUpdatedSignal>(OnAmmoClipsUpdated);
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
		OnAmmoClipsUpdated(new AmmoClipsUpdatedSignal(PlayerController.LocalPlayer.state.AmmoClips));
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<AmmoClipsUpdatedSignal>(OnAmmoClipsUpdated);
	}

	private void OnAmmoClipsUpdated(AmmoClipsUpdatedSignal signal)
	{
		int num = Mathf.FloorToInt(signal.AmmoClips);
		int num2 = Mathf.CeilToInt(signal.AmmoClips);
		float num3 = signal.AmmoClips - (float)num;
		if (num3 == 0f && num2 > 0)
		{
			num3 = 1f;
		}
		_clipCount.text = num2.ToString();
		_chargeImage.fillAmount = num3;
	}
}
