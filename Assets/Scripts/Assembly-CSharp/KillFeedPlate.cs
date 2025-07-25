using System;
using System.Collections;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedPlate : FadeableUI
{
	[Serializable]
	public struct HeroClassToIcon
	{
		public HeroClass HeroClass;

		public Sprite Icon;
	}

	[SerializeField]
	private TextMeshProUGUI _attackerName;

	[SerializeField]
	private TextMeshProUGUI _victimName;

	[SerializeField]
	private Image _attackerHeroClassIcon;

	[SerializeField]
	private Image _victimHeroClassIcon;

	[SerializeField]
	private Image _weaponIcon;

	[SerializeField]
	private float _fadeInDuration = 0.2f;

	[SerializeField]
	private float _fadeOutDuration = 0.3f;

	[SerializeField]
	private float _duration = 3f;

	[SerializeField]
	private Color _localTeamColor;

	[SerializeField]
	private Color _enemyTeamColor;

	[SerializeField]
	private Sprite _defaultWeaponIcon;

	private void OnEnable()
	{
		FadeOut(0f);
	}

	public void Populate(ClientPlayerDiedSignal clientPlayerDiedSignal, ProfileManager profileManager)
	{
		StopAllCoroutines();
		base.transform.SetAsLastSibling();
		_attackerName.text = clientPlayerDiedSignal.AttackerName;
		bool flag = PlayerController.LocalPlayer.Team == (int)clientPlayerDiedSignal.AttackerTeam;
		_attackerName.color = (flag ? _localTeamColor : _enemyTeamColor);
		_victimName.text = clientPlayerDiedSignal.VictimName;
		bool flag2 = PlayerController.LocalPlayer.Team == (int)clientPlayerDiedSignal.VictimTeam;
		_victimName.color = (flag2 ? _localTeamColor : _enemyTeamColor);
		Sprite sprite = Resources.Load<Sprite>($"{clientPlayerDiedSignal.AttackerHeroClass}_HeroClassIcon");
		_attackerHeroClassIcon.sprite = sprite;
		Sprite sprite2 = Resources.Load<Sprite>($"{clientPlayerDiedSignal.VictimHeroClass}_HeroClassIcon");
		_victimHeroClassIcon.sprite = sprite2;
		if (!string.IsNullOrEmpty(clientPlayerDiedSignal.WeaponId))
		{
			WeaponProfile byId = profileManager.GetById<WeaponProfile>(clientPlayerDiedSignal.WeaponId);
			_weaponIcon.sprite = byId.Icon;
		}
		else
		{
			_weaponIcon.sprite = _defaultWeaponIcon;
		}
		StartCoroutine(ShowFadeEffect());
	}

	private IEnumerator ShowFadeEffect()
	{
		yield return FadeToRoutine(1f, _fadeInDuration, null);
		yield return new WaitForSeconds(_duration);
		yield return FadeToRoutine(0f, _fadeOutDuration, null);
		SmartPool.Despawn(base.gameObject);
	}
}
