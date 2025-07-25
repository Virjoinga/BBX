using System;
using System.Collections.Generic;
using BSCore;
using NodeClient;
using UnityEngine;
using Zenject;

public class FriendsList : MonoBehaviour
{
	[Inject]
	private GroupManager _groupManager;

	[SerializeField]
	private FriendPlate _friendPlatePrefab;

	[SerializeField]
	private Transform _container;

	private readonly Dictionary<string, FriendPlate> _platesByServiceId = new Dictionary<string, FriendPlate>();

	public void OnFriendStatusUpdated(string serviceId, PlayerStatus status)
	{
		if (_platesByServiceId.TryGetValue(serviceId, out var value))
		{
			value.UpdateStatus(status);
			value.UpdateGroupAvailability(_groupManager.CanInvite(serviceId));
		}
	}

	public void ClearPlates()
	{
		foreach (FriendPlate value in _platesByServiceId.Values)
		{
			UnityEngine.Object.Destroy(value.gameObject);
		}
		_platesByServiceId.Clear();
	}

	public void Populate(Friend friend, Action<string> removeCallback, Action<FriendPlate> inviteToGroupCallback)
	{
		FriendPlate friendPlate = UnityEngine.Object.Instantiate(_friendPlatePrefab, _container);
		friendPlate.Populate(friend, _groupManager);
		friendPlate.RemoveButtonClicked += removeCallback;
		friendPlate.InviteToGroupClicked += inviteToGroupCallback;
		_platesByServiceId.Add(friend.ServiceId, friendPlate);
	}

	public void UpdateGroupAvailability()
	{
		foreach (KeyValuePair<string, FriendPlate> item in _platesByServiceId)
		{
			item.Value.UpdateGroupAvailability(_groupManager.CanInvite(item.Key));
		}
	}

	public void UpdateGroupAvailability(string playerId)
	{
		if (_platesByServiceId.TryGetValue(playerId, out var value))
		{
			value.UpdateGroupAvailability(_groupManager.CanInvite(playerId));
		}
	}
}
