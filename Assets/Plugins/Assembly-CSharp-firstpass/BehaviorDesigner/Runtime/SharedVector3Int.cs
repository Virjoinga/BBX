using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector3Int : SharedVariable<Vector3Int>
	{
		public static implicit operator SharedVector3Int(Vector3Int value)
		{
			return new SharedVector3Int
			{
				mValue = value
			};
		}
	}
}
