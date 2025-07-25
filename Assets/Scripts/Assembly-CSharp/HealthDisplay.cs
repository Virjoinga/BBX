using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	[SerializeField]
	private Image _healthBar;

	[SerializeField]
	private float _changeSpeed = 1f;

	[SerializeField]
	private float _snapThreshold = 0.9f;

	[SerializeField]
	private float _fillAmount = 1f;

	[SerializeField]
	private float _interimFillAmount;

	private void Start()
	{
		_healthBar.fillAmount = _fillAmount;
	}

	public void UpdateHealthBar(float health, float maxHealth)
	{
		_fillAmount = health / maxHealth;
		if (Mathf.Abs(_fillAmount - _healthBar.fillAmount) > _snapThreshold)
		{
			_healthBar.fillAmount = _fillAmount;
		}
	}

	private void Update()
	{
		_interimFillAmount = Mathf.MoveTowards(_healthBar.fillAmount, _fillAmount, _changeSpeed * Time.deltaTime);
		_healthBar.fillAmount = _interimFillAmount;
	}
}
