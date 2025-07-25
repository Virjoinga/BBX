using UnityEngine;

public class MatchMap : MonoBehaviour
{
	public static bool Loaded { get; private set; }

	private void Awake()
	{
		Loaded = true;
	}

	private void OnDestroy()
	{
		Loaded = false;
	}
}
