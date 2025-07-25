using System.Collections.Generic;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace BSCore.Utils
{
	public static class PlayFabUtils
	{
		public static string ParseError(PlayFabError error)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"PlayFab error: {error.ErrorMessage}");
			stringBuilder.AppendLine($"Error code: {error.Error}");
			if (error.ErrorDetails != null)
			{
				stringBuilder.AppendLine("Error Details: ");
				foreach (KeyValuePair<string, List<string>> errorDetail in error.ErrorDetails)
				{
					stringBuilder.AppendLine($"{errorDetail.Key}: {ErrorValueDetails(errorDetail.Value)}");
				}
			}
			return stringBuilder.ToString();
		}

		public static string ParseError(ScriptExecutionError error)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"PlayFab error: {error.Message}");
			stringBuilder.AppendLine($"Error code: {error.Error}");
			stringBuilder.AppendLine(error.StackTrace);
			return stringBuilder.ToString();
		}

		public static string ErrorValueDetails(List<string> valueDetails)
		{
			string text = "";
			for (int i = 0; i < valueDetails.Count - 1; i++)
			{
				text = text + valueDetails[i] + ", ";
			}
			return text + valueDetails[valueDetails.Count - 1] + ".";
		}

		public static int GetIntFromUserRecord(string key, Dictionary<string, UserDataRecord> records)
		{
			if (records.ContainsKey(key))
			{
				return GetIntFromUserRecord(records[key]);
			}
			return 0;
		}

		public static int GetIntFromUserRecord(UserDataRecord record)
		{
			int result = 0;
			int.TryParse(record.Value, out result);
			return result;
		}

		public static float GetFloatFromUserRecord(string key, Dictionary<string, UserDataRecord> records)
		{
			if (records.ContainsKey(key))
			{
				return GetFloatFromUserRecord(records[key]);
			}
			return 0f;
		}

		public static float GetFloatFromUserRecord(UserDataRecord record)
		{
			float result = 0f;
			float.TryParse(record.Value, out result);
			return result;
		}

		public static bool GetBoolFromUserRecord(string key, Dictionary<string, UserDataRecord> records)
		{
			if (records.ContainsKey(key))
			{
				return GetBoolFromUserRecord(records[key]);
			}
			return false;
		}

		public static bool GetBoolFromUserRecord(UserDataRecord record)
		{
			bool result = false;
			int result2 = 0;
			if (!(bool.TryParse(record.Value, out result) && result))
			{
				if (int.TryParse(record.Value, out result2))
				{
					return result2 != 0;
				}
				return false;
			}
			return true;
		}

		public static FailureReasons ConvertToFailureReason(PlayFabErrorCode errorCode)
		{
			switch (errorCode)
			{
			case PlayFabErrorCode.AccountBanned:
				return FailureReasons.LoginAccountBanned;
			case PlayFabErrorCode.ServiceUnavailable:
				return FailureReasons.WebTimeout;
			case PlayFabErrorCode.ItemNotFound:
				return FailureReasons.ItemNotFound;
			case PlayFabErrorCode.WrongVirtualCurrency:
				return FailureReasons.PurchasingInvalidCurrencyType;
			case PlayFabErrorCode.WrongPrice:
				return FailureReasons.PurchasingWrongPrice;
			case PlayFabErrorCode.InsufficientFunds:
				return FailureReasons.PurchasingInsufficientFunds;
			case PlayFabErrorCode.CharacterNotFound:
				return FailureReasons.CharacterNotFound;
			case PlayFabErrorCode.InvalidSessionTicket:
				return FailureReasons.InvalidSessionTicket;
			case PlayFabErrorCode.NameNotAvailable:
				return FailureReasons.DisplayNameNotAvailable;
			case PlayFabErrorCode.AccountNotFound:
				return FailureReasons.AccountNotFound;
			case PlayFabErrorCode.InvalidEmailOrPassword:
				return FailureReasons.InvalidEmailOrPassword;
			case PlayFabErrorCode.AccountAlreadyLinked:
				return FailureReasons.AccountAlreadyLinked;
			case PlayFabErrorCode.LinkedAccountAlreadyClaimed:
				return FailureReasons.LinkedAccountAlreadyClaimed;
			case PlayFabErrorCode.EmailAddressNotAvailable:
				return FailureReasons.EmailAddressNotAvailable;
			case PlayFabErrorCode.LinkedIdentifierAlreadyClaimed:
				return FailureReasons.LinkedAccountAlreadyClaimed;
			case PlayFabErrorCode.UsersAlreadyFriends:
				return FailureReasons.AddFriendFailed;
			case PlayFabErrorCode.NoContactEmailAddressFound:
				return FailureReasons.NoContactEmailAddressFound;
			default:
				return FailureReasons.Unknown;
			}
		}

		public static FailureReasons ParseFailureReason(PlayFabError error)
		{
			FailureReasons failureReasons = ConvertToFailureReason(error.Error);
			Debug.LogErrorFormat("[PlayFabUtils] Converted PlayFab error: {0} to FailureReasons: {1}", error.Error, failureReasons);
			return failureReasons;
		}

		public static void SetPlayFabTitleId()
		{
			PlayFabSettings.TitleId = Resources.Load<GameConfigData>(string.Format("Data/{0}", "GameConfigData")).GetTitleId();
		}
	}
}
