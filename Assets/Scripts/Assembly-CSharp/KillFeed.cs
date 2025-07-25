using BSCore;
using UnityEngine;
using Zenject;

public class KillFeed : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ProfileManager _profileManager;

	[Inject]
	private EmoticonData _emoticonData;

	[SerializeField]
	private KillFeedPlate _killFeedPlatePrefab;

	[SerializeField]
	private NotificationFeedPlate _notificationFeedPlatePrefab;

	[SerializeField]
	private EmoticonPlate _emoticonPlatePrefab;

	private void Start()
	{
		_signalBus.Subscribe<ClientPlayerDiedSignal>(AddDeathToKillFeed);
		_signalBus.Subscribe<NotificationSignal>(AddNotificationToKillFeed);
		_signalBus.Subscribe<ShowEmoticonSignal>(AddEmoticonToKillFeed);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<ClientPlayerDiedSignal>(AddDeathToKillFeed);
		_signalBus.Unsubscribe<NotificationSignal>(AddNotificationToKillFeed);
		_signalBus.Unsubscribe<ShowEmoticonSignal>(AddEmoticonToKillFeed);
	}

	private void AddDeathToKillFeed(ClientPlayerDiedSignal clientPlayerDiedSignal)
	{
		if (base.gameObject.activeInHierarchy && PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.HasTeamSet)
		{
			KillFeedPlate killFeedPlate = SmartPool.Spawn(_killFeedPlatePrefab);
			if (killFeedPlate != null)
			{
				killFeedPlate.Populate(clientPlayerDiedSignal, _profileManager);
			}
			else
			{
				Debug.LogError("[KillFeed] Failed to spawn kill feed plate from smart pool");
			}
		}
	}

	private void AddEmoticonToKillFeed(ShowEmoticonSignal signal)
	{
		if (base.gameObject.activeInHierarchy)
		{
			Sprite spriteForEmoticon = _emoticonData.GetSpriteForEmoticon((EmoticonType)signal.EmoticonId);
			if (spriteForEmoticon != null)
			{
				SmartPool.Spawn(_emoticonPlatePrefab).Populate(signal.Name, spriteForEmoticon);
			}
		}
	}

	private void AddNotificationToKillFeed(NotificationSignal signal)
	{
		if (base.gameObject.activeInHierarchy)
		{
			SmartPool.Spawn(_notificationFeedPlatePrefab).Populate(signal.Message);
		}
	}
}
