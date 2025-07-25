using UnityEngine;
using UnityEngine.EventSystems;

public class LoadoutPreviewSpinner : MonoBehaviour
{
	[SerializeField]
	private Transform _modelToSpin;

	[SerializeField]
	private float _spinMultiplier;

	[SerializeField]
	private EventTrigger _eventTrigger;

	private void Start()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener(InputDrag);
		_eventTrigger.triggers.Add(entry);
	}

	private void InputDrag(BaseEventData eventData)
	{
		float y = (0f - (eventData as PointerEventData).delta.x) * _spinMultiplier;
		_modelToSpin.localEulerAngles += new Vector3(0f, y, 0f);
	}

	public void SetRotation(float yRotation)
	{
		_modelToSpin.localEulerAngles = new Vector3(0f, yRotation, 0f);
	}
}
