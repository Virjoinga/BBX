using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BSCore
{
	[CreateAssetMenu(fileName = "GameConfigData", menuName = "Data/Config Data")]
	public class GameConfigData : ScriptableObject
	{
		private const string QA_TAG = "qa";

		[Header("PlayFab")]
		[SerializeField]
		private string _devTitleId;

		[SerializeField]
		private string _releaseTitleId;

		[SerializeField]
		private string _defaultCatalog = "Items";

		[Header("MatchMaking")]
		[SerializeField]
		private string _devMatchMakingServer;

		[SerializeField]
		private string _releaseMatchMakingServer;

		[Header("Chat")]
		[SerializeField]
		private string _chatServerAddress;

		[SerializeField]
		[ColorUsage(false)]
		private Color _groupMessageColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _systemMessageColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _publicMessageColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _myMessageNameColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _adminMessageNameColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _otherMessageNameColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _privateMessageColor = Color.white;

		[Header("Quality Settings")]
		[SerializeField]
		private UniversalRenderPipelineAsset _shadowsDisabledAsset;

		[SerializeField]
		private UniversalRenderPipelineAsset _shadowsLowAsset;

		[SerializeField]
		private UniversalRenderPipelineAsset _shadowsMedAsset;

		[SerializeField]
		private UniversalRenderPipelineAsset _shadowsHighAsset;

		[SerializeField]
		[ColorUsage(false)]
		private Color _teammateColor = Color.white;

		[SerializeField]
		[ColorUsage(false)]
		private Color _enemyColor = Color.white;

		public UniversalRenderPipelineAsset ShadowsDisabledAsset => _shadowsDisabledAsset;

		public UniversalRenderPipelineAsset ShadowsLowAsset => _shadowsLowAsset;

		public UniversalRenderPipelineAsset ShadowsMedAsset => _shadowsMedAsset;

		public UniversalRenderPipelineAsset ShadowsHighAsset => _shadowsHighAsset;

		public string DefaultCatalog => _defaultCatalog;

		public string GameVersion => Application.version;

		public string ChatServerAddress => _chatServerAddress;

		public Color GroupMessageColor => _groupMessageColor;

		public Color SystemMessageColor => _systemMessageColor;

		public Color PublicMessageColor => _publicMessageColor;

		public Color MyMessageNameColor => _myMessageNameColor;

		public Color AdminMessageNameColor => _adminMessageNameColor;

		public Color OtherMessageNameColor => _otherMessageNameColor;

		public Color PrivateMessageColor => _privateMessageColor;

		public Color TeammateColor => _teammateColor;

		public Color EnemyColor => _enemyColor;

		public string GetTitleId()
		{
			return _releaseTitleId;
		}

		public string GetMatchMakingServer()
		{
			return _releaseMatchMakingServer;
		}

		public bool GameVersionGreaterThanEqualsVersion(string version)
		{
			int[] array = (from p in GameVersion.Replace("qa", "").Split('.')
				select int.Parse(p)).ToArray();
			int[] array2 = (from p in version.Replace("qa", "").Split('.')
				select int.Parse(p)).ToArray();
			if (array.Length != 3 || array2.Length != 3 || array.Length != array2.Length)
			{
				throw new ArgumentException("Version number incompatibility: " + version + " and " + GameVersion);
			}
			int num = array[0] * 10000 + array[1] * 100 + array[2];
			int num2 = array2[0] * 10000 + array2[1] * 100 + array2[2];
			return num >= num2;
		}
	}
}
