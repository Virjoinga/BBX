using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
	[SerializeField]
	protected Image _progressBar;

	[SerializeField]
	private TextMeshProUGUI _progressPercent;

	[SerializeField]
	private TextMeshProUGUI _progressUpdate;

	[SerializeField]
	private float _barMovementSpeed = 1f;

	protected float _percentage;

	private void Awake()
	{
		_progressBar.fillAmount = 0f;
	}

	public void UpdateProgressPercent(float percentage, string updateMessage = "")
	{
		UpdateProgressPercent(percentage);
		_progressUpdate.text = updateMessage;
	}

	public void UpdateProgressPercent(float percentage)
	{
		_percentage = percentage;
		if (_percentage == 0f)
		{
			_progressBar.fillAmount = _percentage;
		}
	}

	public IEnumerator Finish()
	{
		_progressUpdate.text = "Finishing up...";
		_percentage = 1f;
		while (_progressBar.fillAmount < 1f)
		{
			yield return null;
		}
	}

	protected virtual void Update()
	{
		_progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _percentage, _barMovementSpeed);
		_progressPercent.text = string.Format("{0}%", (_progressBar.fillAmount * 100f).ToString("n0"));
	}
}
