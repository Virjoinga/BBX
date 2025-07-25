using Bolt;
using UnityEngine;

public class BarrensBossStatue : EntityEventListener<IBossStatue>
{
	[SerializeField]
	private GameObject[] _modelOptions;

	[SerializeField]
	private float _minX;

	[SerializeField]
	private float _maxX;

	[SerializeField]
	private float _minZ;

	[SerializeField]
	private float _maxZ;

	public override void Attached()
	{
		if (base.entity.isOwner)
		{
			SetRandomPosition();
			base.state.Model = Random.Range(0, _modelOptions.Length);
		}
		base.state.SetTransforms(base.state.Transform, base.transform);
		base.state.AddCallback("Model", OnModelUpdated);
		OnModelUpdated();
	}

	public override void Detached()
	{
		base.state.RemoveAllCallbacks();
	}

	private void SetRandomPosition()
	{
		float x = Random.Range(_minX, _maxX);
		float z = Random.Range(_minZ, _maxZ);
		base.transform.position = new Vector3(x, base.transform.position.y, z);
	}

	private void OnModelUpdated()
	{
		for (int i = 0; i < _modelOptions.Length; i++)
		{
			_modelOptions[i].SetActive(i == base.state.Model);
		}
	}
}
