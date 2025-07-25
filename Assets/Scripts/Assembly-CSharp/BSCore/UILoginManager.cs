using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BSCore
{
	public class UILoginManager : MonoBehaviour
	{
		[Inject]
		private LoginManager _loginManager;

		[SerializeField]
		private Button _newAccountButton;

		[SerializeField]
		private Button _loginButton;

		[SerializeField]
		private TMP_InputField _emailInput;

		[SerializeField]
		private TMP_InputField _passwordInput;

		private Action _createAccountCallback;

		private Action<string, string> _loginCallback;

		private void Start()
		{
			_newAccountButton.onClick.AddListener(NewAccountClicked);
			_loginButton.onClick.AddListener(OnLoginClicked);
			_loginManager.LoggedIn += OnLoggedIn;
		}

		private void OnDestroy()
		{
			_loginManager.LoggedIn -= OnLoggedIn;
		}

		public void Display(Action createAccountCallback, Action<string, string> loginCallback)
		{
			_createAccountCallback = createAccountCallback;
			_loginCallback = loginCallback;
		}

		private void NewAccountClicked()
		{
			_createAccountCallback();
		}

		private void OnLoginClicked()
		{
			_loginCallback(_emailInput.text, _passwordInput.text);
		}

		private void OnLoggedIn(PlayerProfile player, bool isNewlyCreated)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
