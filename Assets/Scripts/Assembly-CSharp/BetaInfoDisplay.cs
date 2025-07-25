using TMPro;
using UnityEngine;

public class BetaInfoDisplay : MonoBehaviourSingleton<BetaInfoDisplay>
{
	[SerializeField]
	private TextMeshProUGUI _versionText;

	[SerializeField]
	private TextMeshProUGUI _fpsText;

	private float _deltaTime;

	protected override void Awake()
	{
		base.Awake();
		_versionText.text = Application.version;
	}

	private void Update()
	{
		_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
		float num = 1f / _deltaTime;
		_fpsText.text = $"{num:0.} fps";
	}

	public void ToggleVisibleState(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
	}
}
