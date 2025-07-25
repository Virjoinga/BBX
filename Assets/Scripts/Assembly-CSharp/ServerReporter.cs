using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;

public static class ServerReporter
{
	private class WeaponIdToFails
	{
		public string WeaponId;

		public Dictionary<ServerValidationFailureReason, int> FailReasonCounts;

		public WeaponIdToFails(string weaponId)
		{
			WeaponId = weaponId;
			FailReasonCounts = new Dictionary<ServerValidationFailureReason, int>();
		}
	}

	private static Dictionary<string, List<WeaponIdToFails>> _trackedValidationFailures = new Dictionary<string, List<WeaponIdToFails>>();

	public static UnityUserReportingUpdater UnityUserReportingUpdater { get; private set; }

	public static void TrackValidationFailure(string displayName, string weaponId, ServerValidationFailureReason serverValidationFailureReason)
	{
		if (!_trackedValidationFailures.ContainsKey(displayName))
		{
			_trackedValidationFailures.Add(displayName, new List<WeaponIdToFails>());
		}
		WeaponIdToFails weaponIdToFails = _trackedValidationFailures[displayName].FirstOrDefault((WeaponIdToFails x) => x.WeaponId == weaponId);
		if (weaponIdToFails == null)
		{
			weaponIdToFails = new WeaponIdToFails(weaponId);
			_trackedValidationFailures[displayName].Add(weaponIdToFails);
		}
		if (!weaponIdToFails.FailReasonCounts.ContainsKey(serverValidationFailureReason))
		{
			weaponIdToFails.FailReasonCounts.Add(serverValidationFailureReason, 0);
		}
		weaponIdToFails.FailReasonCounts[serverValidationFailureReason]++;
	}

	private static void DebugLogFailures()
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, List<WeaponIdToFails>> trackedValidationFailure in _trackedValidationFailures)
		{
			stringBuilder.AppendLine(trackedValidationFailure.Key + ":");
			foreach (WeaponIdToFails item in trackedValidationFailure.Value)
			{
				stringBuilder.AppendLine("=" + item.WeaponId);
				foreach (KeyValuePair<ServerValidationFailureReason, int> failReasonCount in item.FailReasonCounts)
				{
					stringBuilder.AppendLine($"--{failReasonCount.Key}: {failReasonCount.Value}");
					num += failReasonCount.Value;
				}
			}
		}
		Debug.LogError($"VALIdation Failed!! Total: {num}\n {stringBuilder.ToString()}");
	}

	public static void Init()
	{
		UnityUserReportingUpdater = new UnityUserReportingUpdater();
		UnityUserReporting.Configure(new UserReportingClientConfiguration());
		string endpoint = $"https://userreporting.cloud.unity3d.com/api/userreporting/projects/{UnityUserReporting.CurrentClient.ProjectIdentifier}/ping";
		UnityUserReporting.CurrentClient.Platform.Post(endpoint, "application/json", Encoding.UTF8.GetBytes("\"Ping\""), delegate
		{
		}, delegate
		{
		});
		UnityUserReporting.CurrentClient.IsSelfReporting = true;
	}

	public static void TrySendServerReport()
	{
		CreateReport();
	}

	private static void CreateReport()
	{
		Debug.Log("[ServerReporter] Creating Server Report");
		int totalValidationFailures = 0;
		StringBuilder sb = new StringBuilder();
		foreach (KeyValuePair<string, List<WeaponIdToFails>> trackedValidationFailure in _trackedValidationFailures)
		{
			sb.AppendLine(trackedValidationFailure.Key + ":");
			foreach (WeaponIdToFails item in trackedValidationFailure.Value)
			{
				sb.AppendLine("=" + item.WeaponId);
				foreach (KeyValuePair<ServerValidationFailureReason, int> failReasonCount in item.FailReasonCounts)
				{
					sb.AppendLine($"--{failReasonCount.Key}: {failReasonCount.Value}");
					totalValidationFailures += failReasonCount.Value;
				}
			}
		}
		UnityUserReporting.CurrentClient.CreateUserReport(delegate(UserReport report)
		{
			if (string.IsNullOrEmpty(report.ProjectIdentifier))
			{
				Debug.LogError("[ServerReporter] The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
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
			string text2 = "NA";
			if (!string.IsNullOrEmpty(PlayfabServerManagement.ServerId))
			{
				text2 = PlayfabServerManagement.ServerId;
			}
			report.Summary = $"Server {text2} Reported {totalValidationFailures} Failed Validations";
			report.Dimensions.Add(new UserReportNamedValue("Category", "ServerReport"));
			report.Fields.Add(new UserReportNamedValue("Category", "ServerReport"));
			report.Fields.Add(new UserReportNamedValue("ValidationFailures", sb.ToString()));
			report.Fields.Add(new UserReportNamedValue("ServerId", text2));
			report.Fields.Add(new UserReportNamedValue("SessionId", PlayfabServerManagement.SessionId));
			report.Fields.Add(new UserReportNamedValue("ExpectedPlayers", PlayfabServerManagement.ExpectedPlayerCount.ToString()));
			report.Fields.Add(new UserReportNamedValue("ConnectedPlayers", PlayfabServerManagement.ConnectedPlayerCount.ToString()));
			SubmitReport(report);
		});
	}

	private static void SubmitReport(UserReport userReport)
	{
		Debug.Log("[ServerReporter] Submitting User Report");
		UnityUserReporting.CurrentClient.SendUserReport(userReport, delegate(float uploadProgress, float downloadProgress)
		{
			Debug.Log("[ServerReporter] Uploading Report " + $"{uploadProgress:P}");
		}, delegate(bool success, UserReport sentReport)
		{
			if (!success)
			{
				Debug.Log("[ServerReporter] Failed to Send Report.");
			}
			else
			{
				Debug.Log("[ServerReporter] Report Sent.");
			}
		});
	}
}
