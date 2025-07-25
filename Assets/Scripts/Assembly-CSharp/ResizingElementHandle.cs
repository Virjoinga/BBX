using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ResizingElementHandle : MonoBehaviour
{
	[SerializeField]
	private RectTransform _objectToResize;

	[SerializeField]
	private Vector2 _sizeRange = new Vector2(0.5f, 3f);

	private EventTrigger _eventTrigger;

	private EventTrigger.Entry _pointerDownEntry;

	private EventTrigger.Entry _dragEntry;

	private EventTrigger.Entry _pointerUpEntry;

	private RectTransform _rectTransform;

	private Vector3 _offset;

	private bool _anchoredBottom;

	private bool _anchoredLeft;

	private Vector2 _defaultSize;

	private float _canvasScale = 1f;

	private void Awake()
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
		_eventTrigger.triggers.Add(_pointerDownEntry);
		_eventTrigger.triggers.Add(_dragEntry);
		_eventTrigger.triggers.Add(_pointerUpEntry);
		_rectTransform = GetComponent<RectTransform>();
	}

	private void Start()
	{
		_anchoredBottom = _objectToResize.pivot.y < 0.9f;
		_anchoredLeft = _objectToResize.pivot.x < 0.9f;
		_defaultSize = new Vector2(_objectToResize.rect.width, _objectToResize.rect.height);
		_canvasScale = 1f;
		Canvas[] componentsInParent = GetComponentsInParent<Canvas>();
		foreach (Canvas canvas in componentsInParent)
		{
			_canvasScale *= canvas.transform.localScale.x;
		}
	}

	private void OnPointerDown(BaseEventData data)
	{
		_offset = _rectTransform.position - Input.mousePosition;
		Cursor.visible = false;
	}

	private void OnDrag(BaseEventData data)
	{
		Vector3 vector = Input.mousePosition + _offset - _objectToResize.position;
		float num = Mathf.Abs(vector.x);
		float num2 = Mathf.Abs(vector.y);
		float a = num / _defaultSize.x;
		float b = num2 / _defaultSize.y;
		float num3 = Mathf.Min(a, b);
		_objectToResize.localScale = Vector3.one * Mathf.Clamp(num3 / _canvasScale, _sizeRange.x, _sizeRange.y);
	}

	private void OnPointerUp(BaseEventData data)
	{
		Cursor.visible = true;
	}
}
