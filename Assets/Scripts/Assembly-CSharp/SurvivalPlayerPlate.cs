using TMPro;
using UnityEngine;

public class SurvivalPlayerPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _name;

	[SerializeField]
	private TextMeshProUGUI _points;

	private int _pointsValue;

	public string Name => _name.text;

	public int Points
	{
		get
		{
			return _pointsValue;
		}
		set
		{
			_pointsValue = value;
			_points.text = _pointsValue.ToString();
		}
	}

	public void Setup(string name, int points)
	{
		_name.text = name;
		Points = points;
	}
}
