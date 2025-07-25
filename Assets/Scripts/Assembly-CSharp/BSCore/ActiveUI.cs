using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BSCore
{
	public class ActiveUI : MonoBehaviour
	{
		public class Manager : ITickable
		{
			public static bool Enabled = true;

			public static bool IsActiveUIShown => _isActiveUIShown;

			public static ActiveUI LastActiveUI => _lastActiveUI;

			public static List<ActiveUI> ShownActiveUI => new List<ActiveUI>(_shownActiveUIs);

			public static bool CloseOnEsc
			{
				get
				{
					if (IsActiveUIShown)
					{
						return LastActiveUI._closeOnEsc;
					}
					return false;
				}
			}

			public static bool CanOpenUI
			{
				get
				{
					if (IsActiveUIShown)
					{
						return !LastActiveUI._preventsOtherActiveUI;
					}
					return false;
				}
			}

			public static bool IsInputSupressed
			{
				get
				{
					if (IsActiveUIShown)
					{
						return LastActiveUI._unlocksCursorOnOpen;
					}
					return false;
				}
			}

			public static event Action<ActiveUI, bool> ActiveUIToggled
			{
				add
				{
					_activeUIToggled += value;
				}
				remove
				{
					_activeUIToggled -= value;
				}
			}

			public static void CloseAllActiveUI()
			{
				foreach (ActiveUI item in new List<ActiveUI>(_shownActiveUIs))
				{
					item.Hide();
				}
			}

			public void Tick()
			{
				if (!Enabled || !BSCoreInput.Enabled)
				{
					return;
				}
				bool buttonDown = BSCoreInput.GetButtonDown(Option.Clear, notOverUI: false);
				foreach (KeyValuePair<Option, List<ActiveUI>> item in _activeUIsByToggleKey)
				{
					bool buttonDown2 = BSCoreInput.GetButtonDown(item.Key, notOverUI: false);
					bool buttonUp = BSCoreInput.GetButtonUp(item.Key);
					foreach (ActiveUI item2 in item.Value)
					{
						if (buttonDown && item2._closeOnEsc && item2.IsActive)
						{
							item2.Hide(_shownActiveUIs.Count > 1);
						}
						else if (item2._openWhileHeld)
						{
							HandleHeldInput(item2, buttonDown2, buttonUp);
						}
						else if (buttonDown2)
						{
							HandlePressInput(item2);
						}
					}
				}
			}

			private void HandleHeldInput(ActiveUI ui, bool keyDown, bool keyUp)
			{
				if (keyDown && !ui.IsActive)
				{
					ui.Show();
				}
				else if (keyUp && ui.IsActive)
				{
					ui.Hide();
				}
			}

			private void HandlePressInput(ActiveUI ui)
			{
				if (!ui.IsActive)
				{
					if (IsActiveUIShown && LastActiveUI._preventsOtherActiveUI && !LastActiveUI._canSwapTo.Contains(ui))
					{
						return;
					}
					if (ui.ClosesOthersOnOpen)
					{
						foreach (ActiveUI item in new List<ActiveUI>(_shownActiveUIs))
						{
							item.Hide(ui._unlocksCursorOnOpen);
						}
					}
				}
				ui.Toggle();
			}
		}

		private static List<ActiveUI> _shownActiveUIs = new List<ActiveUI>();

		private static Dictionary<Option, List<ActiveUI>> _activeUIsByToggleKey = new Dictionary<Option, List<ActiveUI>>();

		[Header("Showing/Hiding")]
		[SerializeField]
		private Button[] _closeButtons;

		[SerializeField]
		private bool _closeOnEsc = true;

		[SerializeField]
		private float _fadeInDuration = 0.5f;

		[SerializeField]
		private float _fadeOutDuration = 0.5f;

		[SerializeField]
		private FadeableUI _fader;

		[SerializeField]
		private GameObject _container;

		[Header("Other ActiveUI Interaction")]
		[SerializeField]
		private bool _preventsOtherActiveUI;

		[SerializeField]
		private bool _closesOthersOnOpen;

		[SerializeField]
		private bool _unlocksCursorOnOpen = true;

		[SerializeField]
		private List<ActiveUI> _canSwapTo;

		[Header("Input-based Toggling")]
		[SerializeField]
		private bool _inputToggled;

		[SerializeField]
		private Option _toggleInput;

		[SerializeField]
		private bool _openWhileHeld;

		private static bool _isActiveUIShown => _shownActiveUIs.Count > 0;

		private static ActiveUI _lastActiveUI
		{
			get
			{
				if (!_isActiveUIShown)
				{
					return null;
				}
				return _shownActiveUIs.Last();
			}
		}

		public bool ClosesOthersOnOpen => _closesOthersOnOpen;

		public bool UnlocksCursorOnOpen => _unlocksCursorOnOpen;

		public bool IsActive { get; private set; }

		public bool PreventOpening { get; set; }

		private static event Action<ActiveUI, bool> _activeUIToggled;

		private event Action<bool> _localActiveUIToggled;

		public event Action<bool> LocalActiveUIToggled
		{
			add
			{
				_localActiveUIToggled += value;
			}
			remove
			{
				_localActiveUIToggled -= value;
			}
		}

		protected virtual void Awake()
		{
			if (_inputToggled)
			{
				if (!_activeUIsByToggleKey.TryGetValue(_toggleInput, out var value))
				{
					value = new List<ActiveUI>();
					_activeUIsByToggleKey.Add(_toggleInput, value);
				}
				value.Add(this);
			}
		}

		protected virtual void Start()
		{
			IsActive = _container.activeInHierarchy;
			Button[] closeButtons = _closeButtons;
			for (int i = 0; i < closeButtons.Length; i++)
			{
				closeButtons[i].onClick.AddListener(OnCloseClicked);
			}
		}

		protected virtual void OnDestroy()
		{
			if (_inputToggled && _activeUIsByToggleKey.ContainsKey(_toggleInput))
			{
				_activeUIsByToggleKey[_toggleInput].Remove(this);
			}
			if (IsActive)
			{
				CleanUpDestroyedUI();
			}
			_fader.Stop();
		}

		public virtual void Toggle()
		{
			if (IsActive)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		public virtual void Show(bool forceShow = false, bool ignoreMouseRelease = false)
		{
			if ((!forceShow && _isActiveUIShown && _lastActiveUI._preventsOtherActiveUI) || _shownActiveUIs.Contains(this) || PreventOpening)
			{
				return;
			}
			_fader.Stop();
			_container.SetActive(value: true);
			IsActive = true;
			if (_fader != null)
			{
				_fader.FadeIn(_fadeInDuration);
			}
			if (_isActiveUIShown && _closesOthersOnOpen)
			{
				foreach (ActiveUI item in _shownActiveUIs.ToList())
				{
					item.Hide();
				}
			}
			_shownActiveUIs.Add(this);
			if (_unlocksCursorOnOpen && !ignoreMouseRelease)
			{
				MonoBehaviourSingleton<MouseLockToggle>.Instance.ReleaseCursor();
			}
			ActiveUI._activeUIToggled?.Invoke(this, arg2: true);
			this._localActiveUIToggled?.Invoke(obj: true);
		}

		public virtual void Hide(bool ignoreMouseLock = false)
		{
			_fader.Stop();
			if (_fader != null)
			{
				_fader.FadeOut(_fadeOutDuration, delegate
				{
					_container.SetActive(value: false);
				});
			}
			else
			{
				_container.SetActive(value: false);
			}
			IsActive = false;
			_shownActiveUIs.Remove(this);
			if ((!_isActiveUIShown || !_lastActiveUI._unlocksCursorOnOpen) && !ignoreMouseLock)
			{
				MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
			}
			ActiveUI._activeUIToggled?.Invoke(this, arg2: false);
			this._localActiveUIToggled?.Invoke(obj: false);
		}

		private void CleanUpDestroyedUI()
		{
			_fader.Stop();
			IsActive = false;
			_shownActiveUIs.Remove(this);
			if ((!_isActiveUIShown || !_lastActiveUI._unlocksCursorOnOpen) && MonoBehaviourSingleton<MouseLockToggle>.IsInstantiated)
			{
				MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
			}
			ActiveUI._activeUIToggled?.Invoke(this, arg2: false);
		}

		protected virtual void OnCloseClicked()
		{
			Hide();
		}
	}
}
