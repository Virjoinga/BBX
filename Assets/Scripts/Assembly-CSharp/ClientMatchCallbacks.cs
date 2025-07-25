using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using Constants;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class ClientMatchCallbacks : GlobalEventListener
{
	private SignalBus _signalBus;

	private UserManager _userManager;

	private PlayerController.SpeedHackDetectionConfigData _speedHackDetectionConfig;

	private string _localEntityId;

	private void Start()
	{
		_signalBus = SceneContextHelper.ResolveZenjectBinding<SignalBus>();
		_userManager = SceneContextHelper.ResolveZenjectBinding<UserManager>();
		ConfigManager configManager = SceneContextHelper.ResolveZenjectBinding<ConfigManager>();
		_speedHackDetectionConfig = configManager.Get<PlayerController.SpeedHackDetectionConfigData>(DataKeys.speedHackDetection);
		_localEntityId = _userManager.CurrentUser.Entity.Id;
	}

	public override void OnEvent(DamagableDamaged damagedEvent)
	{
		_signalBus.Fire(new DamagableDamagedSignal(damagedEvent.Attacker, damagedEvent.Victim, damagedEvent.Damage));
		if (damagedEvent.IsPlayer && damagedEvent.Died && damagedEvent.Attacker.StateIs<IPlayerState>())
		{
			IPlayerState state = damagedEvent.Attacker.GetState<IPlayerState>();
			string displayName = state.DisplayName;
			OutfitProfile outfitProfile = damagedEvent.Attacker.GetComponent<LoadoutController>().OutfitProfile;
			IPlayerState state2 = damagedEvent.Victim.GetState<IPlayerState>();
			string displayName2 = state2.DisplayName;
			OutfitProfile outfitProfile2 = damagedEvent.Victim.GetComponent<LoadoutController>().OutfitProfile;
			_signalBus.Fire(new ClientPlayerDiedSignal(displayName, outfitProfile.HeroClass, (TeamId)state.Team, displayName2, outfitProfile2.HeroClass, (TeamId)state2.Team, damagedEvent.WeaponId));
		}
	}

	public override void OnEvent(PlayerDied playerDiedEvent)
	{
		bool isControlled = playerDiedEvent.Attacker.isControlled;
		bool isControlled2 = playerDiedEvent.Victim.isControlled;
		bool localPlayerAssisted = false;
		if (!string.IsNullOrEmpty(playerDiedEvent.AssistingPlayerIds))
		{
			string[] array = playerDiedEvent.AssistingPlayerIds.Split(',');
			if (array != null && array.Length != 0)
			{
				localPlayerAssisted = array.Contains(_localEntityId);
			}
		}
		_signalBus.Fire(new PlayerEliminatedSignal
		{
			LocalPlayerPerformedFinalBlow = isControlled,
			LocalPlayerAssisted = localPlayerAssisted,
			VictimName = playerDiedEvent.VictimName,
			LocalPlayerIsVictim = isControlled2
		});
	}

	public override void OnEvent(ServerMessageEvent evnt)
	{
		switch ((Match.ServerMessageIds)evnt.MessageId)
		{
		case Match.ServerMessageIds.NotEnoughPlayers:
			HandleNotEnoughPlayersMessage();
			break;
		case Match.ServerMessageIds.TeamsRebalanced:
			HandleTeamsRebalancedMessage();
			break;
		case Match.ServerMessageIds.HackingDetected:
			HandleHackDetectedMessage();
			break;
		case Match.ServerMessageIds.KickedForHacking:
			HandleKickedForHackingMessage();
			break;
		default:
			Debug.LogError($"[ClientMatchCallbacks] No case found for server message type {(Match.ServerMessageIds)evnt.MessageId}");
			break;
		}
	}

	private void HandleNotEnoughPlayersMessage()
	{
		DisplayMessagePopup("Not enough players successfully connected. Match canceled");
	}

	private void HandleTeamsRebalancedMessage()
	{
		_signalBus.Fire(new NotificationSignal("Team size mismatch. Some players were moved to the other team."));
	}

	private void HandleHackDetectedMessage()
	{
		DisplayMessagePopup(string.Format(_speedHackDetectionConfig.WarningMessage, _speedHackDetectionConfig.PenaltyDuration));
	}

	private void HandleKickedForHackingMessage()
	{
		DisplayMessagePopup(_speedHackDetectionConfig.KickMessage);
	}

	private void DisplayMessagePopup(string message)
	{
		UIPrefabManager.Instantiate(UIPrefabIds.GenericPopup, delegate(GameObject go)
		{
			OnDisplayMessageUISpawned(go, message);
		}, interactive: true, 99);
	}

	private void OnDisplayMessageUISpawned(GameObject uiGameobject, string message)
	{
		if (!(uiGameobject == null))
		{
			UIGenericPopup component = uiGameobject.GetComponent<UIGenericPopup>();
			GenericPopupDetails popupDetails = new GenericPopupDetails
			{
				AllowCloseButtons = false,
				Message = message,
				Button1Details = new GenericButtonDetails("Ok", onClose, Color.green)
			};
			component.Show(popupDetails, onClose);
		}
		void onClose()
		{
			UIPrefabManager.Destroy(UIPrefabIds.GenericPopup);
		}
	}

	public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
	{
		if (SceneManager.GetActiveScene().name != "MainMenu" && UIPrefabManager.sceneLoad != "MainMenu")
		{
			Debug.Log($"Disconnected From Bolt. Reason - {disconnectReason}");
			if (disconnectReason == UdpConnectionDisconnectReason.Error || disconnectReason == UdpConnectionDisconnectReason.Timeout)
			{
				UIGenericPopupManager.ShowConfirmPopup("Disconnected From Server!", null);
			}
			UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlaySpawned);
		}
	}

	private void OnLoadingOverlaySpawned(GameObject uiGameobject)
	{
		UIPrefabManager.Destroy(UIPrefabIds.TDMMatchLoadingScreen, 2f);
		if (MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
		{
			MonoBehaviourSingleton<OTSCamera>.Instance.DestroySelf();
		}
		SceneManager.LoadScene("MainMenu");
	}
}
