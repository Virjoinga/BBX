using System;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointUI : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private GameObject _highlight;

	public string SpawnID { get; private set; }

	private event Action<SpawnPointUI> _spawnPointSelected;

	public event Action<SpawnPointUI> SpawnPointSelected
	{
		add
		{
			_spawnPointSelected += value;
		}
		remove
		{
			_spawnPointSelected -= value;
		}
	}

	private void Start()
	{
		_button.onClick.AddListener(delegate
		{
			this._spawnPointSelected?.Invoke(this);
		});
	}

	public void Populate(string spawnId, Color color)
	{
		SpawnID = spawnId;
		_button.image.color = color;
		_highlight.SetActive(value: false);
	}

	public void SetHighlight(bool isHighlighted)
	{
		_highlight.SetActive(isHighlighted);
	}
}
