using UnityEngine;

public class Backpack : MonoBehaviour
{
	private BackpackProfile _profile;

	public BackpackProfile Profile
	{
		get
		{
			return _profile;
		}
		set
		{
			_profile = value;
			SetProperties();
		}
	}

	public BackpackAnimationController AnimationController { get; private set; }

	private void Awake()
	{
		AnimationController = GetComponent<BackpackAnimationController>();
	}

	private void SetProperties()
	{
	}
}
