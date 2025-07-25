using RSG;
using UnityEngine;

public class OfflineTestBootloader : ClientBootloader
{
	[SerializeField]
	private UITestLoadoutLoader _uiTestloadoutLoader;

	protected override void Start()
	{
		AfterProfileFetched(OnProfilesFetched);
		base.Start();
	}

	private IPromise OnProfilesFetched()
	{
		_uiTestloadoutLoader.OnProfilesFetched(_profileManager.AllProfiles);
		return Promise.Resolved();
	}

	protected override void OnDone()
	{
	}
}
