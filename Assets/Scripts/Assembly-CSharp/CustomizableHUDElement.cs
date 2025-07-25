using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class CustomizableHUDElement : MonoBehaviour
{
	[SerializeField]
	private string _id = "";

	[SerializeField]
	private Transform _resizeHandle;

	[SerializeField]
	private GameObject _tooltip;

	[SerializeField]
	private GameObject _clickArea;

	private EventTrigger _eventTrigger;

	private EventTrigger.Entry _pointerDownEntry;

	private EventTrigger.Entry _dragEntry;

	private EventTrigger.Entry _pointerUpEntry;

	private RectTransform _rectTransform;

	private Vector3 _offset;

	private Vector3 _defaultPosition;

	private float _defaultScale;

	public string Id => _id;

	private void Awake()
	{
		_resizeHandle.gameObject.SetActive(value: false);
		_tooltip.SetActive(value: false);
		_clickArea.SetActive(value: false);
		_rectTransform = GetComponent<RectTransform>();
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
		_defaultPosition = _rectTransform.position;
		_defaultScale = _rectTransform.localScale.x;
	}

	private void OnEnable()
	{
		_eventTrigger.triggers.Add(_pointerDownEntry);
		_eventTrigger.triggers.Add(_dragEntry);
		_eventTrigger.triggers.Add(_pointerUpEntry);
	}

	private void OnDisable()
	{
		_eventTrigger.triggers.Remove(_pointerDownEntry);
		_eventTrigger.triggers.Remove(_dragEntry);
		_eventTrigger.triggers.Remove(_pointerUpEntry);
	}

	public void ToggleCustomization(bool isOn)
	{
		_resizeHandle.gameObject.SetActive(isOn);
		_tooltip.SetActive(isOn);
		_clickArea.SetActive(isOn);
		base.enabled = isOn;
	}

	public void ResetCustomization()
	{
		_rectTransform.position = _defaultPosition;
		_rectTransform.localScale = Vector3.one * _defaultScale;
	}

	public void ApplyCustomization(ElementCustomization customization)
	{
		_rectTransform.position = customization.position;
		_rectTransform.localScale = Vector3.one * customization.scale;
	}

	public ElementCustomization GetCustomization()
	{
		return new ElementCustomization
		{
			position = _rectTransform.position,
			scale = _rectTransform.localScale.x
		};
	}

	private void OnPointerDown(BaseEventData data)
	{
		_offset = Input.mousePosition - _rectTransform.position;
	}

	private void OnDrag(BaseEventData data)
	{
		_rectTransform.position = Input.mousePosition - _offset;
	}

	private void OnPointerUp(BaseEventData data)
	{
	}
}
