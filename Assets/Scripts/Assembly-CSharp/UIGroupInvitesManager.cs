using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIGroupInvitesManager : MonoBehaviour
{
	[Inject]
	private GroupManager _groupManager;

	[SerializeField]
	private UIGroupInvitePlate _platePrefab;

	private readonly Dictionary<string, UIGroupInvitePlate> _platesByInviterId = new Dictionary<string, UIGroupInvitePlate>();

	private void Start()
	{
		_groupManager.InviteReceived += OnInviteReceived;
		foreach (KeyValuePair<string, InvitedRelayCrumb> receivedInvite in _groupManager.ReceivedInvites)
		{
			OnInviteReceived(receivedInvite.Value);
		}
	}

	private void OnDestroy()
	{
		_groupManager.InviteReceived -= OnInviteReceived;
	}

	private void OnInviteReceived(InvitedRelayCrumb crumb)
	{
		UIGroupInvitePlate uIGroupInvitePlate = SmartPool.Spawn(_platePrefab);
		uIGroupInvitePlate.Populate(crumb);
		uIGroupInvitePlate.Responded -= OnResponded;
		uIGroupInvitePlate.Responded += OnResponded;
		_platesByInviterId.Add(crumb.InviterId, uIGroupInvitePlate);
	}

	private void OnResponded(InvitedRelayCrumb crumb, bool accepted)
	{
		if (_platesByInviterId.TryGetValue(crumb.InviterId, out var value))
		{
			SmartPool.Despawn(value.gameObject);
			_platesByInviterId.Remove(crumb.InviterId);
		}
		_groupManager.RespondToInvite(crumb, accepted);
	}
}
