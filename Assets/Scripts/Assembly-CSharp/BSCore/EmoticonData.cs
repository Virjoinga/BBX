using System;
using System.Linq;
using UnityEngine;

namespace BSCore
{
	[CreateAssetMenu(fileName = "EmoticonData", menuName = "Data/EmoticonData")]
	public class EmoticonData : ScriptableObject
	{
		[Serializable]
		public struct EmoticonTypeToSprite
		{
			public EmoticonType Emoticon;

			public Sprite Icon;
		}

		[SerializeField]
		private EmoticonTypeToSprite[] _emoticonTypesToSprites;

		public Sprite GetSpriteForEmoticon(EmoticonType emoticon)
		{
			EmoticonTypeToSprite emoticonTypeToSprite = _emoticonTypesToSprites.FirstOrDefault((EmoticonTypeToSprite x) => x.Emoticon == emoticon);
			if (emoticonTypeToSprite.Icon == null)
			{
				Debug.LogError($"[EmoticonData] No Sprite found for emoticon: {emoticon}");
			}
			return emoticonTypeToSprite.Icon;
		}
	}
}
