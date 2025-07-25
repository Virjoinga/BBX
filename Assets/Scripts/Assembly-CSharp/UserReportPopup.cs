using System;
using System.Collections;
using System.Text;
using BSCore;
using TMPro;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UserReportPopup : MonoBehaviour
{
	[Inject]
	private UserManager _userManager;

	[SerializeField]
	private TMP_InputField _summaryInput;

	[SerializeField]
	private TMP_InputField _descriptionInput;

	[SerializeField]
	private TMP_Dropdown _categoryDropdown;

	[SerializeField]
	private TextMeshProUGUI _errorText;

	[SerializeField]
	private TextMeshProUGUI _progressText;

	[SerializeField]
	private TextMeshProUGUI _submittingText;

	[SerializeField]
	private Image _thumbnailViewer;

	[SerializeField]
	private GameObject _submittingOverlay;

	[SerializeField]
	private Button _submitButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private Button _doneButton;

	private UnityUserReportingUpdater _unityUserReportingUpdater = new UnityUserReportingUpdater();

	private bool _isSubmitting;

	private bool _takingScreenshot;

	public UserReport CurrentUserReport { get; private set; }

	private IEnumerator Start()
	{
		UnityUserReporting.Configure(new UserReportingClientConfiguration());
		string endpoint = $"https://userreporting.cloud.unity3d.com/api/userreporting/projects/{UnityUserReporting.CurrentClient.ProjectIdentifier}/ping";
		UnityUserReporting.CurrentClient.Platform.Post(endpoint, "application/json", Encoding.UTF8.GetBytes("\"Ping\""), delegate
		{
		}, delegate
		{
		});
		UnityUserReporting.CurrentClient.IsSelfReporting = true;
		_thumbnailViewer.enabled = false;
		_takingScreenshot = ConnectionManager.IsConnected;
		if (_takingScreenshot)
		{
			UIPrefabManager.ShowHideInteractiveCanvas(shouldShow: false);
			yield return new WaitForFixedUpdate();
		}
		CreateUserReport();
		_submitButton.onClick.AddListener(SubmitUserReport);
		_cancelButton.onClick.AddListener(ClosePopup);
		_doneButton.onClick.AddListener(ClosePopup);
		yield return new WaitForSeconds(5f);
		UIPrefabManager.ShowHideInteractiveCanvas(shouldShow: true);
	}

	private void Update()
	{
		_unityUserReportingUpdater.Reset();
		StartCoroutine(_unityUserReportingUpdater);
	}

	private void CreateUserReport()
	{
		if (_takingScreenshot)
		{
			UnityUserReporting.CurrentClient.TakeScreenshot(2048, 2048, delegate
			{
			});
			UnityUserReporting.CurrentClient.TakeScreenshot(512, 512, delegate
			{
			});
		}
		UnityUserReporting.CurrentClient.CreateUserReport(delegate(UserReport report)
		{
			if (string.IsNullOrEmpty(report.ProjectIdentifier))
			{
				Debug.LogError("The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
			}
			string arg = "Unknown";
			string text = "0.0";
			foreach (UserReportNamedValue deviceMetadatum in report.DeviceMetadata)
			{
				if (deviceMetadatum.Name == "Platform")
				{
					arg = deviceMetadatum.Value;
				}
				if (deviceMetadatum.Name == "Version")
				{
					text = deviceMetadatum.Value;
				}
			}
			report.Dimensions.Add(new UserReportNamedValue("Platform.Version", $"{arg}.{text}"));
			report.Dimensions.Add(new UserReportNamedValue("Version", text));
			CurrentUserReport = report;
			if (_takingScreenshot)
			{
				SetThumbnail(report);
				UIPrefabManager.ShowHideInteractiveCanvas(shouldShow: true);
				_takingScreenshot = false;
			}
			else
			{
				_thumbnailViewer.enabled = false;
			}
		});
	}

	private void SubmitUserReport()
	{
		if (_isSubmitting || CurrentUserReport == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(_summaryInput.text) || string.IsNullOrEmpty(_descriptionInput.text))
		{
			_errorText.text = "Please provide a Summary and Description";
			return;
		}
		_submittingOverlay.SetActive(value: true);
		_errorText.text = string.Empty;
		_progressText.text = "0%";
		_isSubmitting = true;
		CurrentUserReport.Summary = _summaryInput.text;
		string text = _categoryDropdown.options[_categoryDropdown.value].text;
		CurrentUserReport.Dimensions.Add(new UserReportNamedValue("Category", text));
		CurrentUserReport.Fields.Add(new UserReportNamedValue("Category", text));
		CurrentUserReport.Fields.Add(new UserReportNamedValue("Description", _descriptionInput.text));
		PlayerProfile currentUser = _userManager.CurrentUser;
		CurrentUserReport.Fields.Add(new UserReportNamedValue("PlayfabId", currentUser.Id));
		CurrentUserReport.Fields.Add(new UserReportNamedValue("DisplayName", currentUser.DisplayName));
		CurrentUserReport.Fields.Add(new UserReportNamedValue("HeroClass", currentUser.LoadoutManager.EquippedHeroClass.ToString()));
		CurrentUserReport.Fields.Add(new UserReportNamedValue("EquippedLoadout", currentUser.LoadoutManager.GetEquippedLoadout().ToJson()));
		string value = "NA";
		if (!string.IsNullOrEmpty(ClientTeamDeathMatchGameModeEntity.SERVERID))
		{
			value = ClientTeamDeathMatchGameModeEntity.SERVERID;
		}
		CurrentUserReport.Fields.Add(new UserReportNamedValue("ServerId", value));
		if (BoltNetwork.IsConnected && PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.state != null)
		{
			string value2 = PlayerController.LocalPlayer.state.DebugState();
			CurrentUserReport.Fields.Add(new UserReportNamedValue("PlayerState", value2));
		}
		if (BoltNetwork.IsConnected && ClientTeamDeathMatchGameModeEntity.HasTDMGameMode && ClientTeamDeathMatchGameModeEntity.TDMGameMode.state != null)
		{
			string value3 = ClientTeamDeathMatchGameModeEntity.TDMGameMode.state.DebugState();
			CurrentUserReport.Fields.Add(new UserReportNamedValue("TDMMatchState", value3));
		}
		UnityUserReporting.CurrentClient.SendUserReport(CurrentUserReport, delegate(float uploadProgress, float downloadProgress)
		{
			string text2 = $"{uploadProgress:P}";
			_progressText.text = text2;
		}, delegate(bool success, UserReport report)
		{
			if (!success)
			{
				_errorText.text = "Failed to Send Report. Please Try Again";
				_submittingOverlay.SetActive(value: false);
			}
			else
			{
				CurrentUserReport = null;
				_submittingText.text = "Thank you for the Report!";
				_doneButton.gameObject.SetActive(value: true);
			}
			_isSubmitting = false;
		});
	}

	private void SetThumbnail(UserReport userReport)
	{
		if (userReport != null && _thumbnailViewer != null)
		{
			byte[] data = Convert.FromBase64String(userReport.Thumbnail.DataBase64);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			_thumbnailViewer.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			_thumbnailViewer.preserveAspect = true;
			_thumbnailViewer.enabled = true;
		}
	}

	private void OnGUI()
	{
		if (_takingScreenshot)
		{
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.alignment = TextAnchor.UpperCenter;
			gUIStyle.fontSize = 50;
			gUIStyle.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width / 2 - 500, Screen.height - 100, 1000f, 100f), "Capturing Screenshot", gUIStyle);
		}
	}

	private void ClosePopup()
	{
		UIPrefabManager.Destroy(UIPrefabIds.UserReportPopup);
	}
}
