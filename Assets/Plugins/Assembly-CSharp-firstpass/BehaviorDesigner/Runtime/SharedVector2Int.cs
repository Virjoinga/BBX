using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector2Int : SharedVariable<Vector2Int>
	{
		public static implicit operator SharedVector2Int(Vector2Int value)
		{
			return new SharedVector2Int
			{
				mValue = value
			};
		}
	}
}
