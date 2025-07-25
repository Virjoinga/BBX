using UnityEngine;
using UnityEngine.SceneManagement;

namespace BSCore
{
	public abstract class MouseLockToggle : MonoBehaviourSingleton<MouseLockToggle>
	{
		private const string MAIN_MENU_SCENE = "MainMenu";

		public bool MouseCanLock;

		private bool _initialLoad = true;

		public static bool MouseLockReleased => Cursor.lockState != CursorLockMode.Locked;

		private void Start()
		{
			MouseCanLock = false;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void PreventLock(bool prevent)
		{
			MouseCanLock = !prevent;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "MainMenu")
			{
				MouseCanLock = false;
				ReleaseCursor();
			}
		}

		protected override void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			ReleaseCursor();
			base.OnDestroy();
		}

		private void Update()
		{
			if (CheckReleaseMousePressed())
			{
				if (MouseLockReleased)
				{
					TryLockCursor();
				}
				else
				{
					ReleaseCursor();
				}
			}
			TryLockCursorFromMovementInput();
		}

		private void TryLockCursorFromMovementInput()
		{
			if (CheckForMovementInput())
			{
				TryLockCursor();
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				ReleaseCursor();
			}
			else if (hasFocus && !_initialLoad)
			{
				TryLockCursor();
			}
			_initialLoad = false;
		}

		public void ReleaseCursor()
		{
			if (Cursor.lockState != CursorLockMode.None)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		public void TryLockCursor()
		{
			if (MouseCanLock && !ActiveUI.Manager.IsActiveUIShown && Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		protected abstract bool CheckReleaseMousePressed();

		protected abstract bool CheckForMovementInput();
	}
}
