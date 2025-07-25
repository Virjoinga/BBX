using UnityEngine;

public class Hat : MonoBehaviour
{
	private HatProfile _profile;

	public HatProfile Profile
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

	private void SetProperties()
	{
	}
}
