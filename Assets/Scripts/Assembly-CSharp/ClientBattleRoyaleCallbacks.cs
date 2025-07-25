using System.Collections;
using Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour(BoltNetworkModes.Client, new string[] { "BattleRoyaleGameMode" })]
public class ClientBattleRoyaleCallbacks : ClientBaseGameModeCallbacks
{
	public static bool IsSpectating;

	public static bool CancelSpectateDelay;

	private bool _isSpectating;

	private BoltEntity _spectatingTarget;

	private bool _hasSpectatingTarget
	{
		get
		{
			if (_spectatingTarget != null)
			{
				return _spectatingTarget.gameObject != null;
			}
			return false;
		}
	}

	protected override bool _matchIsActive => ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.MatchState == MatchState.Active;

	private void Awake()
	{
		IsSpectating = false;
	}

	private void Update()
	{
		if (IsSpectating && !_hasSpectatingTarget)
		{
			PlayerController[] array = Object.FindObjectsOfType<PlayerController>();
			foreach (PlayerController playerController in array)
			{
				if (playerController.state.Damageable.Health > 0f)
				{
					_spectatingTarget = playerController.entity;
					SetTarget();
					break;
				}
			}
		}
		else if (IsSpectating && !_isSpectating)
		{
			SetTarget();
		}
	}

	public override void OnEvent(DamagableDamaged damagedEvent)
	{
		if (PlayerController.HasLocalPlayer && damagedEvent.Died && damagedEvent.Victim == PlayerController.LocalPlayer.entity)
		{
			StartSpectating(damagedEvent);
		}
	}

	private void StartSpectating(DamagableDamaged playerDamagedEvent)
	{
		BoltEntity spectatingTarget = _spectatingTarget;
		if ((PlayerController.HasLocalPlayer && playerDamagedEvent.Victim == PlayerController.LocalPlayer.entity) || playerDamagedEvent.Victim == _spectatingTarget || (IsSpectating && !_hasSpectatingTarget))
		{
			if (PlayerController.HasLocalPlayer && playerDamagedEvent.Victim == PlayerController.LocalPlayer.entity)
			{
				StartCoroutine(SpectateAfterSeconds(5f));
			}
			_spectatingTarget = playerDamagedEvent.Attacker;
		}
		if (IsSpectating && spectatingTarget != _spectatingTarget)
		{
			SetTarget();
		}
	}

	private IEnumerator SpectateAfterSeconds(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (!CancelSpectateDelay)
		{
			IsSpectating = true;
		}
		CancelSpectateDelay = false;
	}

	private void SetTarget()
	{
		if (MonoBehaviourSingleton<OTSCamera>.IsInstantiated && _hasSpectatingTarget)
		{
			_isSpectating = true;
		}
	}

	public override void BoltShutdownBegin(AddCallback registerDoneCallback)
	{
		base.BoltShutdownBegin(registerDoneCallback);
		if (SceneManager.GetActiveScene().name != "MainMenu")
		{
			UIPrefabManager.Destroy(UIPrefabIds.BRDeathScreen);
			UIPrefabManager.Destroy(UIPrefabIds.BRVictoryScreen);
			UIPrefabManager.Destroy(UIPrefabIds.BRMatchHud);
			UIPrefabManager.Destroy(UIPrefabIds.BRSecondLifeHud);
		}
	}
}
