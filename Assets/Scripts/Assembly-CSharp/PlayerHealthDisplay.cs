using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerHealthDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private TextMeshProUGUI _healthText;

	[SerializeField]
	private Image _healthBar;

	[Inject]
	private void Constuct()
	{
		_signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
	}

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.LoadoutController != null && PlayerController.LocalPlayer.LoadoutController.OutfitProfile != null && PlayerController.LocalPlayer.LoadoutController.OutfitProfile.HeroClassProfile != null);
		float num = PlayerController.LocalPlayer.LoadoutController.TryGetModifiedHealth();
		OnPlayerHealthChanged(num, num);
	}

	private void OnPlayerHealthChangedSignal(PlayerHealthChangedSignal playerHealthChangedSignal)
	{
		OnPlayerHealthChanged(playerHealthChangedSignal.UpdatedHealth, playerHealthChangedSignal.MaxHealth);
	}

	private void OnPlayerHealthChanged(float updatedHealth, float maxHealth)
	{
		int num = Mathf.CeilToInt(updatedHealth);
		_healthText.text = $"{num}/{Mathf.RoundToInt(maxHealth)}";
		float t = updatedHealth / maxHealth;
		_healthBar.fillAmount = Mathf.Lerp(0f, 1f, t);
	}
}
