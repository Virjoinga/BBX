using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using DG.Tweening;
using UnityEngine;

public class BRZoneEntity : EntityEventListener<IBRZone>
{
	private const float CLOSER_MOVE_TIME = 10f;

	private const float ZONE_SCALE_IN_TIME = 5f;

	private const float CLOSER_SCALE_IN_TIME = 0.7f;

	[SerializeField]
	private int _id;

	[SerializeField]
	private List<BRZoneEntity> _neighbors = new List<BRZoneEntity>();

	[SerializeField]
	private Transform _closerTransform;

	[SerializeField]
	private SphereCollider _damageCollider;

	[SerializeField]
	private GameObject _smokeVFX;

	[SerializeField]
	private Transform _closerEndPosition;

	[SerializeField]
	private float _damageColliderMaxSize = 15f;

	public int Id => base.state.Id;

	public BRZoneStatus Status => (BRZoneStatus)base.state.Status;

	public float DamageColliderMaxSize => _damageColliderMaxSize * _damageCollider.radius;

	public List<BRZoneEntity> Neighbors => new List<BRZoneEntity>(_neighbors);

	public override void Attached()
	{
		if (base.entity.isOwner)
		{
			base.state.Id = _id;
			base.state.Status = 0;
		}
		base.state.SetTransforms(base.state.CloserTransform, _closerTransform);
		base.state.AddCallback("Status", OnStatusUpdated);
	}

	public override void Detached()
	{
		base.state.RemoveAllCallbacks();
	}

	private void OnStatusUpdated()
	{
		Debug.Log($"Status Changed to {(BRZoneStatus)base.state.Status}");
		if (base.state.Status == 1)
		{
			_closerTransform.localScale = Vector3.zero;
			_closerTransform.gameObject.SetActive(value: true);
			_closerTransform.DOScale(Vector3.one, 0.7f);
			if (base.entity.isOwner)
			{
				_closerTransform.DOMove(_closerEndPosition.position, 10f);
			}
			else if (MonoBehaviourSingleton<BRMinimapZoneController>.IsInstantiated)
			{
				MonoBehaviourSingleton<BRMinimapZoneController>.Instance.ClosingZone(base.state.Id);
			}
		}
		else if (base.state.Status == 2)
		{
			_damageCollider.transform.localScale = Vector3.zero;
			_damageCollider.gameObject.SetActive(value: true);
			_damageCollider.transform.DOScale(_damageColliderMaxSize, 5f).OnComplete(OnZoneCloseCompleted);
			if (!base.entity.isOwner && MonoBehaviourSingleton<BRMinimapZoneController>.IsInstantiated)
			{
				MonoBehaviourSingleton<BRMinimapZoneController>.Instance.ClosedZone(base.state.Id);
			}
		}
	}

	private void OnZoneCloseCompleted()
	{
		_closerTransform.DOScale(Vector3.zero, 0.7f).OnComplete(delegate
		{
			_closerTransform.gameObject.SetActive(value: false);
		});
		_ = base.entity.isOwner;
	}

	public bool IsConnectedToZone(int otherZoneId, List<int> checkedZoneIds, List<int> openZoneIds)
	{
		List<BRZoneEntity> list = _neighbors.Where((BRZoneEntity x) => openZoneIds.Contains(x.Id) && !checkedZoneIds.Contains(x.Id)).ToList();
		if (list.Count <= 0)
		{
			return false;
		}
		foreach (BRZoneEntity item in list)
		{
			if (item.Id == otherZoneId)
			{
				return true;
			}
			checkedZoneIds.Add(item.Id);
			if (item.IsConnectedToZone(otherZoneId, checkedZoneIds, openZoneIds))
			{
				return true;
			}
		}
		return false;
	}

	public void CloseZone(float closeTime)
	{
		if (base.entity.IsOwner())
		{
			Debug.Log($"Closing Zone {_id}");
			StartCoroutine(CloseZoneRoutine(closeTime));
		}
	}

	private IEnumerator CloseZoneRoutine(float closeTime)
	{
		base.state.Status = 1;
		yield return new WaitForSeconds(closeTime);
		base.state.Status = 2;
	}
}
