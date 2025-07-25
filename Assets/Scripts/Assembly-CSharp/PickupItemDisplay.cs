using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class PickupItemDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private Image _itemIcon;

	[SerializeField]
	private TextMeshProUGUI _itemNameText;

	[SerializeField]
	private WeaponSlots _weaponSlots;

	private RectTransform _rectTransform;

	private EventTrigger _eventTrigger;

	private EventTrigger.Entry _pointerDownEntry;

	private EventTrigger.Entry _dragEntry;

	private EventTrigger.Entry _pointerUpEntry;

	private Vector3 _originalPosition;

	private Vector3 _offset;

	private WeaponSlot _lastSlot;

	private ProfileWithHeroClass _itemProfile;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_originalPosition = _rectTransform.anchoredPosition;
		SetupEventTriggers();
		_container.SetActive(value: false);
	}

	private void OnEnable()
	{
		_signalBus.Subscribe<InRangeOfPickupSignal>(InRangeOfPickup);
		_signalBus.Subscribe<OutOfRangeOfPickupSignal>(OutOfRangeOfPickup);
		_eventTrigger.triggers.Add(_pointerDownEntry);
		_eventTrigger.triggers.Add(_dragEntry);
		_eventTrigger.triggers.Add(_pointerUpEntry);
	}

	private void OnDisable()
	{
		_signalBus.Unsubscribe<InRangeOfPickupSignal>(InRangeOfPickup);
		_signalBus.Unsubscribe<OutOfRangeOfPickupSignal>(OutOfRangeOfPickup);
		_eventTrigger.triggers.Remove(_pointerDownEntry);
		_eventTrigger.triggers.Remove(_dragEntry);
		_eventTrigger.triggers.Remove(_pointerUpEntry);
	}

	private void SetupEventTriggers()
	{
		_eventTrigger = GetComponent<EventTrigger>();
		_pointerDownEntry = new EventTrigger.Entry();
		_pointerDownEntry.eventID = EventTriggerType.PointerDown;
		_pointerDownEntry.callback.AddListener(OnPointerDown);
		_dragEntry = new EventTrigger.Entry();
		_dragEntry.eventID = EventTriggerType.Drag;
		_dragEntry.callback.AddListener(OnDrag);
		_pointerUpEntry = new EventTrigger.Entry();
		_pointerUpEntry.eventID = EventTriggerType.PointerUp;
		_pointerUpEntry.callback.AddListener(OnPointerUp);
	}

	private void OnPointerDown(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		_offset = _rectTransform.position - (Vector3)pointerEventData.position;
		_rectTransform.anchoredPosition = _originalPosition;
	}

	private void OnDrag(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		_rectTransform.position = (Vector3)pointerEventData.position + _offset;
		if (_weaponSlots.PointerIsOver(pointerEventData, out var slot))
		{
			if (_lastSlot != slot)
			{
				if (_lastSlot != null)
				{
					_lastSlot.SetHighlight(isOn: false);
				}
				slot.SetHighlight(slot.Index != 0 || _itemProfile.ItemType == ItemType.meleeWeapon);
				_lastSlot = slot;
			}
		}
		else
		{
			if (_lastSlot != null)
			{
				_lastSlot.SetHighlight(isOn: false);
			}
			_lastSlot = null;
		}
	}

	private void OnPointerUp(BaseEventData eventData)
	{
		_rectTransform.anchoredPosition = _originalPosition;
		PointerEventData eventData2 = eventData as PointerEventData;
		if (_weaponSlots.PointerIsOver(eventData2, out var slot))
		{
			int index = slot.Index;
			if (index != 0 || _itemProfile.ItemType == ItemType.meleeWeapon)
			{
				_signalBus.Fire(new TryPickupItemSignal
				{
					Slot = index
				});
			}
		}
		_weaponSlots.ClearHighlights();
	}

	private void InRangeOfPickup(InRangeOfPickupSignal inRangeOfPickupSignal)
	{
		_itemProfile = inRangeOfPickupSignal.PickupProfile;
		_itemIcon.sprite = _itemProfile.Icon;
		_itemNameText.text = _itemProfile.Name;
		_container.SetActive(value: true);
	}

	private void OutOfRangeOfPickup()
	{
		_container.SetActive(value: false);
	}

	private void PickupItemClicked(int slotType)
	{
		_signalBus.Fire(new TryPickupItemSignal
		{
			Slot = slotType
		});
		_container.SetActive(value: false);
	}
}
