using RSG;
using UnityEngine;

public class MenuPelicantController : MonoBehaviour
{
	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private Material _blueMaterial;

	[SerializeField]
	private Material _redMaterial;

	private Tuple<float, float> _minMaxNextFlightTime = new Tuple<float, float>(10f, 30f);

	private Tuple<float, float> _minMaxXPosition = new Tuple<float, float>(-40f, -50f);

	private Tuple<float, float> _minMaxYPosition = new Tuple<float, float>(2.5f, 7.5f);

	private Tuple<float, float> _minMaxZRotation = new Tuple<float, float>(-25f, 7f);

	private Tuple<float, float> _minMaxShipSpeed = new Tuple<float, float>(3f, 3f);

	private readonly float _flightTime = 9f;

	private bool _isFlying;

	private float _nextFlightTimer;

	private float _currentFlightTimer;

	private float _timeToNextFlight;

	private float _shipSpeed = 3f;

	private void Start()
	{
		_timeToNextFlight = Random.Range(_minMaxNextFlightTime.Item1, _minMaxNextFlightTime.Item2);
	}

	private void Update()
	{
		if (!_isFlying)
		{
			_nextFlightTimer += Time.deltaTime;
			if (_nextFlightTimer >= _timeToNextFlight)
			{
				_shipSpeed = Random.Range(_minMaxShipSpeed.Item1, _minMaxShipSpeed.Item2);
				float x = Random.Range(_minMaxXPosition.Item1, _minMaxXPosition.Item2);
				float y = Random.Range(_minMaxYPosition.Item1, _minMaxYPosition.Item2);
				float z = Random.Range(_minMaxZRotation.Item1, _minMaxZRotation.Item2);
				float z2 = -12f;
				float y2 = 0f;
				if (Random.value > 0.5f)
				{
					z2 = 12f;
					y2 = 180f;
				}
				base.transform.position = new Vector3(x, y, z2);
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, y2, z);
				_renderer.material = ((Random.value > 0.5f) ? _blueMaterial : _redMaterial);
				_currentFlightTimer = 0f;
				_isFlying = true;
			}
		}
		else
		{
			Vector3 b = base.transform.position + base.transform.forward;
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * _shipSpeed);
			_currentFlightTimer += Time.deltaTime;
			if (_currentFlightTimer >= _flightTime)
			{
				_timeToNextFlight = Random.Range(_minMaxNextFlightTime.Item1, _minMaxNextFlightTime.Item2);
				_nextFlightTimer = 0f;
				_isFlying = false;
			}
		}
	}
}
