using Bolt;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour(BoltNetworkModes.Client, new string[] { "SurvivalGameMode" })]
public class ClientSurvivalCallbacks : ClientBaseGameModeCallbacks
{
	protected override bool _matchIsActive => ClientBaseGameModeEntity<ISurvivalGameModeState>.MatchState == MatchState.Active;

	public override void BoltShutdownBegin(AddCallback registerDoneCallback)
	{
		base.BoltShutdownBegin(registerDoneCallback);
		if (SceneManager.GetActiveScene().name != "MainMenu")
		{
			UIPrefabManager.Destroy(UIPrefabIds.SurvivalDeathScreen);
		}
	}
}
