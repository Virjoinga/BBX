using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HuggableOutfit : Outfit
{
	[Header("Huggable Heads")]
	[SerializeField]
	private GameObject _baseHead;

	[SerializeField]
	private GameObject _removableHead;

	[SerializeField]
	private Transform _removableHeadPositioner;

	private Hat _removableHat;

	private Coroutine _headRemovalRoutine;

	public Transform RemovableHeadPositioner => _removableHeadPositioner;

	protected override void Awake()
	{
		base.Awake();
		_removableHead.SetActive(value: false);
	}

	public void RemoveHead(float duration)
	{
		if (_headRemovalRoutine != null)
		{
			StopCoroutine(_headRemovalRoutine);
		}
		_headRemovalRoutine = StartCoroutine(RemoveHeadRoutine(duration));
	}

	private IEnumerator RemoveHeadRoutine(float duration)
	{
		RemoveHead();
		yield return new WaitForSeconds(duration + 0.25f);
		ReplaceHead();
	}

	public void CancelRemoveHead()
	{
		if (_headRemovalRoutine != null)
		{
			StopCoroutine(_headRemovalRoutine);
		}
		ReplaceHead();
	}

	public override void ReleaseEquippedItems()
	{
		base.ReleaseEquippedItems();
		if (_removableHat != null)
		{
			Addressables.ReleaseInstance(_removableHat.gameObject);
			_removableHat = null;
		}
	}

	public override void EquipHat(HatProfile profile)
	{
		base.EquipHat(profile);
		if (_removableHat != null)
		{
			Addressables.ReleaseInstance(_removableHat.gameObject);
			_removableHat = null;
		}
		if (profile != null && profile.HeroClass == _profile.HeroClass)
		{
			StartCoroutine(InstantiateRemovableHatRoutine(profile));
		}
	}

	private IEnumerator InstantiateRemovableHatRoutine(HatProfile profile)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(profile.Id, _removableHeadPositioner);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			_removableHat = handle.Result.GetComponent<Hat>();
			_removableHat.Profile = profile;
			_removableHat.name = profile.Id;
			_removableHat.gameObject.SetActive(value: false);
		}
	}

	public void RemoveHead()
	{
		_baseHead.SetActive(value: false);
		_removableHead.SetActive(value: true);
		if (base.Hat != null)
		{
			base.Hat.gameObject.SetActive(value: false);
		}
		if (_removableHat != null)
		{
			_removableHat.gameObject.SetActive(value: true);
		}
	}

	private void ReplaceHead()
	{
		_baseHead.SetActive(value: true);
		_removableHead.SetActive(value: false);
		if (base.Hat != null)
		{
			base.Hat.gameObject.SetActive(value: true);
		}
		if (_removableHat != null)
		{
			_removableHat.gameObject.SetActive(value: false);
		}
	}
}
