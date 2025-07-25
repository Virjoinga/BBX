using System;
using BSCore;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestsList : MonoBehaviour
{
	[SerializeField]
	private FriendRequestPlate _friendRequestPlatePrefab;

	[SerializeField]
	private Transform _container;

	[SerializeField]
	private Button _addFriendButton;

	[SerializeField]
	private AddFriendPopup _addFriendPopup;

	private void Awake()
	{
		_addFriendButton.onClick.AddListener(delegate
		{
			_addFriendPopup.gameObject.SetActive(value: true);
		});
	}

	public void ClearPlates()
	{
		_container.DestroyChildren();
	}

	public void Populate(Friend friend, Action<string> approveCallback, Action<string> removeCallback)
	{
		FriendRequestPlate friendRequestPlate = UnityEngine.Object.Instantiate(_friendRequestPlatePrefab, _container);
		friendRequestPlate.Populate(friend);
		friendRequestPlate.RemoveFriend -= removeCallback;
		friendRequestPlate.ApproveFriend -= approveCallback;
		friendRequestPlate.RemoveFriend += removeCallback;
		friendRequestPlate.ApproveFriend += approveCallback;
	}
}
