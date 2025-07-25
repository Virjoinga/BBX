using System;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class SteamAbstractionLayer
	{
		public class OverlayStateChangedSignal
		{
			public bool isActive;

			public OverlayStateChangedSignal(bool isActive)
			{
				this.isActive = isActive;
			}
		}

		private SignalBus _signalbus;

		private Callback<GameOverlayActivated_t> _gameOverlayActivated;

		private Callback<MicroTxnAuthorizationResponse_t> _microTxnAuthCallback;

		public bool Initialized => SteamManager.Initialized;

		private event Action<bool> _purchaseAuthorizeResponse;

		public event Action<bool> PurchaseAuthorizeResponse
		{
			add
			{
				_purchaseAuthorizeResponse += value;
			}
			remove
			{
				_purchaseAuthorizeResponse -= value;
			}
		}

		public SteamAbstractionLayer(SignalBus signalbus)
		{
			_signalbus = signalbus;
		}

		public void Initilize(Action onInitialized)
		{
			DelayedAction.RunWhen(() => SteamManager.Initialized, delegate
			{
				SteamInitialized();
				onInitialized?.Invoke();
			});
		}

		public string GetDisplayName()
		{
			if (!SteamManager.Initialized)
			{
				return "";
			}
			return SteamFriends.GetPersonaName();
		}

		public string GetSteamId()
		{
			if (!SteamManager.Initialized)
			{
				return "";
			}
			return SteamUser.GetSteamID().ToString();
		}

		public string GetAuthSessionTicket()
		{
			byte[] array = new byte[1024];
			SteamUser.GetAuthSessionTicket(array, array.Length, out var pcbTicket);
			Array.Resize(ref array, (int)pcbTicket);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array.Take((int)pcbTicket).ToArray();
			foreach (byte b in array2)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		private void SteamInitialized()
		{
			Debug.LogFormat("Steam initialized for user {0} with ID {1}", GetDisplayName(), GetSteamId());
			_gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			_microTxnAuthCallback = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
		}

		private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
		{
			bool isActive = pCallback.m_bActive != 0;
			_signalbus.Fire(new OverlayStateChangedSignal(isActive));
		}

		private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t response)
		{
			bool obj = response.m_bAuthorized != 0;
			this._purchaseAuthorizeResponse?.Invoke(obj);
		}
	}
}
