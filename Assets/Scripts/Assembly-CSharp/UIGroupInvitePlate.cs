using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupInvitePlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _displayName;

	[SerializeField]
	private Button _acceptButton;

	[SerializeField]
	private Button _rejectButton;

	[SerializeField]
	private TimerBar _timerBar;

	private InvitedRelayCrumb _crumb;

	private WaitForSeconds _timeoutTimer = new WaitForSeconds(60f);

	private event Action<InvitedRelayCrumb, bool> _responded;

	public event Action<InvitedRelayCrumb, bool> Responded
	{
		add
		{
			_responded += value;
		}
		remove
		{
			_responded -= value;
		}
	}

	private void Start()
	{
		_acceptButton.onClick.AddListener(delegate
		{
			this._responded(_crumb, arg2: true);
		});
		_rejectButton.onClick.AddListener(delegate
		{
			this._responded(_crumb, arg2: false);
		});
	}

	private void OnEnable()
	{
		StartCoroutine(RejectAfterTimeout());
	}

	public void Populate(InvitedRelayCrumb crumb)
	{
		_crumb = crumb;
		_displayName.text = crumb.InviterName;
		_timerBar.StartTimer(60f);
	}

	private IEnumerator RejectAfterTimeout()
	{
		yield return _timeoutTimer;
		this._responded(_crumb, arg2: false);
	}
}
