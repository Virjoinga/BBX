using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class KillStreakDisplay : MonoBehaviour
{
	[Serializable]
	private struct StreakToColor
	{
		public int Streak;

		public Color Color;
	}

	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private TextMeshProUGUI _killstreakText;

	[SerializeField]
	private float _scaleInOutTime = 0.25f;

	[SerializeField]
	private float _bounceTime = 0.25f;

	[SerializeField]
	private int _bounceCount = 5;

	[SerializeField]
	private List<StreakToColor> _killStreakColors;

	private int _killStreak;

	private Tweener _tweener;

	private void Start()
	{
		_killstreakText.transform.DOScale(0f, 0f);
		_signalBus.Subscribe<PlayerEliminatedSignal>(OnPlayerEliminated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<PlayerEliminatedSignal>(OnPlayerEliminated);
	}

	private void OnPlayerEliminated(PlayerEliminatedSignal signal)
	{
		if (signal.LocalPlayerIsVictim)
		{
			_killStreak = 0;
		}
		else if (signal.LocalPlayerPerformedFinalBlow || signal.LocalPlayerAssisted)
		{
			_killStreak++;
			TryShowKillstreakMessage();
		}
	}

	private void TryShowKillstreakMessage()
	{
		switch (_killStreak)
		{
		case 3:
			ShowMessage("Beartacular Streak! (3)");
			break;
		case 5:
			ShowMessage("Pawsitively Amazing Streak! (5)");
			break;
		case 7:
			ShowMessage("Barbearic Streak! (7)");
			break;
		case 10:
			ShowMessage("Un-Bearleavable Streak! (10)");
			break;
		}
	}

	private void ShowMessage(string message)
	{
		if (_tweener != null)
		{
			_tweener.Kill();
			_killstreakText.transform.DOScale(0f, 0f);
		}
		_killstreakText.text = message;
		foreach (StreakToColor killStreakColor in _killStreakColors)
		{
			if (killStreakColor.Streak == _killStreak)
			{
				_killstreakText.color = killStreakColor.Color;
			}
		}
		_tweener = _killstreakText.transform.DOScale(1f, _scaleInOutTime).OnComplete(StartBounce);
	}

	private void StartBounce()
	{
		_tweener = _killstreakText.transform.DOScale(0.77f, _bounceTime).SetLoops(_bounceCount, LoopType.Yoyo).OnComplete(delegate
		{
			_killstreakText.transform.DOScale(0f, _scaleInOutTime);
		});
	}

	private void OnDisable()
	{
		if (_tweener != null)
		{
			_tweener.Kill();
			_killstreakText.transform.DOScale(0f, 0f);
		}
	}
}
