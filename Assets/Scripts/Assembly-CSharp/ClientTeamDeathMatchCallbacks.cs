using Bolt;
using UdpKit;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour(BoltNetworkModes.Client, new string[] { "TeamDeathMatchGameMode" })]
public class ClientTeamDeathMatchCallbacks : GlobalEventListener
{
	public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
	{
		if (SceneManager.GetActiveScene().name != "MainMenu")
		{
			UIPrefabManager.Destroy(UIPrefabIds.TDMMatchHud);
			UIPrefabManager.Destroy(UIPrefabIds.DamageNumbers);
		}
	}
}
