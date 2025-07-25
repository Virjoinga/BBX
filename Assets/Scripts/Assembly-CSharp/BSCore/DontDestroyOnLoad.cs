using UnityEngine;

namespace BSCore
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
