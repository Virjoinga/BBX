using System;
using System.Collections.Generic;
using System.Linq;
using BSCore.Constants.Config;
using RSG;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class ClientBSCoreBootloader : MonoBehaviour
	{
		public class ServiceFailedException : Exception
		{
			public FailureReasons reason { get; private set; }

			public ServiceFailedException(FailureReasons reason)
			{
				this.reason = reason;
			}
		}

		private enum LoginMethod
		{
			GUID = 0,
			Steam = 1,
			Username = 2,
			DebugId = 3
		}

		private const string VERSION_OUTDATED_MESSAGE = "Game Update Required!\n\nPlease update to the latest version.";

		[Inject]
		protected LoginManager _loginManager;

		[Inject]
		protected UserManager _userManager;

		[Inject]
		protected ProfileManager _profileManager;

		[Inject]
		protected InventoryManager _inventoryManager;

		[Inject]
		protected ConfigManager _configManager;

		[Inject]
		protected GameConfigData _gameConfigData;

		[SerializeField]
		private LoginMethod _loginMethod;

		[SerializeField]
		private string _username = "skyvutest";

		[SerializeField]
		private string _password = "password";

		private DataStoreString _persistentGUID;

		private readonly List<Func<IPromise>> _beforeLoginActions = new List<Func<IPromise>>();

		private readonly List<Func<IPromise>> _afterLoginActions = new List<Func<IPromise>>();

		private readonly List<Func<IPromise>> _afterConfigFetchedActions = new List<Func<IPromise>>();

		private readonly List<Func<IPromise>> _afterUserDataFetchedActions = new List<Func<IPromise>>();

		private readonly List<Func<IPromise>> _afterUserDataVerifiedActions = new List<Func<IPromise>>();

		private readonly List<Func<IPromise>> _afterProfilesFetchedActions = new List<Func<IPromise>>();

		private readonly List<Action> _afterDoneActions = new List<Action>();

		protected virtual void Awake()
		{
			_persistentGUID = new DataStoreString("GUID", "");
			Debug.Log("[ClientBSCoreBootloader] GUID: " + _persistentGUID.Value);
		}

		protected virtual void Start()
		{
			StartBootloading();
		}

		public void BeforeLogin(Func<IPromise> action)
		{
			_beforeLoginActions.Add(action);
		}

		public void AfterLogin(Func<IPromise> action)
		{
			_afterLoginActions.Add(action);
		}

		public void AfterConfigDataFetched(Func<IPromise> action)
		{
			_afterConfigFetchedActions.Add(action);
		}

		public void AfterUserDataFetched(Func<IPromise> action)
		{
			_afterUserDataFetchedActions.Add(action);
		}

		public void AfterUserDataVerified(Func<IPromise> action)
		{
			_afterUserDataVerifiedActions.Add(action);
		}

		public void AfterProfileFetched(Func<IPromise> action)
		{
			_afterProfilesFetchedActions.Add(action);
		}

		public void AfterDone(Action action)
		{
			_afterDoneActions.Add(action);
		}

		private void StartBootloading()
		{
			Debug.Log("[ClientBSCoreBootloader] Bootloading...");
			Promise.All(_beforeLoginActions.Select((Func<IPromise> p) => p())).Then(delegate
			{
				Debug.Log("[ClientBSCoreBootloader] Pre-login complete...");
				Login();
			});
		}

		protected void Login(string email, string password)
		{
			Debug.Log("[ClientBSCoreBootloader] Logging in...");
			Action<FailureReasons> onFailureWrapper = delegate(FailureReasons reason)
			{
				OnLoginFailed(reason);
			};
			Action<PlayerProfile, bool> onSuccess = delegate(PlayerProfile player, bool isNewlyCreated)
			{
				_loginManager.LinkSeamlessAccount(delegate
				{
					FinishBootloading(player);
				}, onFailureWrapper);
				FinishBootloading(player);
			};
			_loginManager.Login(_username, _password, onSuccess, onFailureWrapper);
		}

		protected virtual void Login(bool createAccount = true)
		{
			Debug.Log("[ClientBSCoreBootloader] Logging in...");
			Action<PlayerProfile, bool> onSuccess = delegate(PlayerProfile player, bool isNewlyCreated)
			{
				FinishBootloading(player);
			};
			Action<FailureReasons> onFailure = delegate(FailureReasons reason)
			{
				OnLoginFailed(reason);
			};
			_loginManager.LoginWithSteam(createAccount, onSuccess, onFailure);
		}

		private void OnDebugLoginPopupCreated(GameObject uiGameobject, Action<string> loginAction)
		{
			if (!(uiGameobject == null))
			{
				uiGameobject.GetComponent<DebugLoginPopup>().Listen(loginAction);
			}
		}

		protected virtual void OnLoginFailed(FailureReasons reason)
		{
			Debug.LogError($"[ClientBSCoreBootloader] Login failed with reason: {reason}");
		}

		private void FinishBootloading(PlayerProfile player)
		{
			Debug.Log("[ClientBSCoreBootloader] Finishing bootloading...");
			Promise.All(_afterLoginActions.Select((Func<IPromise> p) => p())).Then((Func<IPromise>)AfterLoggedIn).Then((Func<IPromise>)ServerVersionCheck)
				.Then((Func<IPromise>)ServerLockedCheck)
				.Catch(OnBootloadingFailed)
				.Done(delegate
				{
					foreach (Action afterDoneAction in _afterDoneActions)
					{
						afterDoneAction();
					}
				});
		}

		protected virtual void OnBootloadingFailed(Exception error)
		{
			Debug.LogError($"[ClientBSCoreBootloader] Bootloading failed with error: {error}");
			UIPrefabManager.Instantiate(UIPrefabIds.GenericPopup, delegate(GameObject go)
			{
				OnBootloadingFailedPopupCreated(go, error.Message);
			}, interactive: true, 99);
			throw new Exception("Bootloading failed");
		}

		private void OnBootloadingFailedPopupCreated(GameObject uiGameobject, string errorMessage)
		{
			if (!(uiGameobject == null))
			{
				UIGenericPopup component = uiGameobject.GetComponent<UIGenericPopup>();
				GenericPopupDetails popupDetails = new GenericPopupDetails
				{
					Message = errorMessage,
					Button1Details = new GenericButtonDetails("Exit", OnBootloadingFailurePopupClose, Color.red),
					AllowCloseButtons = false
				};
				component.Show(popupDetails, OnBootloadingFailurePopupClose);
			}
		}

		private void OnBootloadingFailurePopupClose()
		{
			Application.Quit();
		}

		private IPromise AfterLoggedIn()
		{
			return Promise.All(FetchConfig(), FetchProfiles(), FetchUserData());
		}

		private IPromise FetchConfig()
		{
			Promise promise = new Promise();
			_configManager.Fetch(promise.Resolve, delegate(FailureReasons reason)
			{
				promise.Reject(new ServiceFailedException(reason));
			});
			return promise.ThenAll(() => _afterConfigFetchedActions.Select((Func<IPromise> p) => p()));
		}

		private IPromise ServerLockedCheck()
		{
			if (_userManager.CurrentUser.IsAdmin)
			{
				return Promise.Resolved();
			}
			ServerLockoutData serverLockoutData = _configManager.Get<ServerLockoutData>(DataKeys.ServerLockout);
			if (serverLockoutData.Locked)
			{
				return Promise.Rejected(new Exception(serverLockoutData.Message));
			}
			return Promise.Resolved();
		}

		private IPromise ServerVersionCheck()
		{
			string text = _configManager.Get(DataKeys.gameVersion, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return Promise.Rejected(new Exception("Game Update Required!\n\nPlease update to the latest version."));
			}
			if (_gameConfigData.GameVersionGreaterThanEqualsVersion(text))
			{
				return Promise.Resolved();
			}
			return Promise.Rejected(new Exception("Game Update Required!\n\nPlease update to the latest version."));
		}

		private IPromise FetchProfiles()
		{
			Promise promise = new Promise();
			_profileManager.Fetch(_gameConfigData.DefaultCatalog, delegate
			{
				promise.Resolve();
			}, delegate(FailureReasons reason)
			{
				promise.Reject(new ServiceFailedException(reason));
			});
			return promise.ThenAll(() => _afterProfilesFetchedActions.Select((Func<IPromise> p) => p())).Then((Func<IPromise>)FetchInventory);
		}

		private IPromise FetchUserData()
		{
			Promise promise = new Promise();
			_userManager.FetchUserData(promise.Resolve, delegate(FailureReasons reason)
			{
				promise.Reject(new ServiceFailedException(reason));
			});
			return promise.ThenAll(() => _afterUserDataFetchedActions.Select((Func<IPromise> p) => p())).ThenAll(() => _afterUserDataVerifiedActions.Select((Func<IPromise> p) => p()));
		}

		private IPromise FetchInventory()
		{
			Promise promise = new Promise();
			_inventoryManager.Fetch(delegate
			{
				promise.Resolve();
			}, delegate(FailureReasons reason)
			{
				promise.Reject(new ServiceFailedException(reason));
			});
			return promise;
		}
	}
}
