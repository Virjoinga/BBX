using BSCore;
using BSCore.Constants.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BRSecondLifeButton : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ConfigManager _configManager;

	[SerializeField]
	private BRSpectateButton _spectateButton;

	[SerializeField]
	private TextMeshProUGUI _timerText;

	[SerializeField]
	private Button[] _secondLifeButtons;

	private BattleRoyaleConfigData _config;

	private float _timeRemaining;

	private void Awake()
	{
		_config = _configManager.Get<BattleRoyaleConfigData>(DataKeys.BattleRoyale);
		if (PlayerController.HasLocalPlayer)
		{
			Button[] secondLifeButtons = _secondLifeButtons;
			for (int i = 0; i < secondLifeButtons.Length; i++)
			{
				secondLifeButtons[i].onClick.AddListener(OnClick);
			}
			if (PlayerController.LocalPlayer.state.CanSecondLife)
			{
				StartTimer();
				return;
			}
			base.gameObject.SetActive(value: false);
			PlayerController.LocalPlayer.state.AddCallback("CanSecondLife", OnCanSecondLifeUpdated);
		}
	}

	private void Update()
	{
		if (!PlayerController.HasLocalPlayer)
		{
			base.gameObject.SetActive(value: false);
		}
		_timeRemaining -= Time.deltaTime;
		string text = _timeRemaining.ToString("#0.0");
		_timerText.text = "(" + text + " s)";
	}

	private void OnCanSecondLifeUpdated()
	{
		if (base.gameObject.activeInHierarchy && !PlayerController.LocalPlayer.state.CanSecondLife)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (!base.gameObject.activeInHierarchy && PlayerController.LocalPlayer.state.CanSecondLife)
		{
			base.gameObject.SetActive(value: true);
			StartTimer();
		}
	}

	private void StartTimer()
	{
		_timeRemaining = _config.SecondLife.TimeLimit - 0.5f;
		Debug.Log($"[BRSecondLifeButton] _timeRemaining set to {_timeRemaining}");
	}

	private void OnClick()
	{
		base.gameObject.SetActive(value: false);
		_spectateButton.gameObject.SetActive(value: false);
		_signalBus.Fire(default(SecondLifeButtonClickedSignal));
	}
}
