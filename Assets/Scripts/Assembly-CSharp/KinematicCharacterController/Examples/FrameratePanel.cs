using System;
using UnityEngine;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
	public class FrameratePanel : MonoBehaviour
	{
		public float PollingRate = 1f;

		public Text PhysicsRate;

		public Text PhysicsFPS;

		public Text AvgFPS;

		public Text AvgFPSMin;

		public Text AvgFPSMax;

		public Action<float> OnPhysicsFPSReady;

		public string[] FramerateStrings = new string[0];

		private bool _isFixedUpdateThisFrame;

		private bool _wasFixedUpdateLastFrame;

		private int _physFramesCount;

		private float _physFramesDeltaSum;

		private int _framesCount;

		private float _framesDeltaSum;

		private float _minDeltaTimeForAvg = float.PositiveInfinity;

		private float _maxDeltaTimeForAvg = float.NegativeInfinity;

		private float _timeOfLastPoll;

		private void FixedUpdate()
		{
			_isFixedUpdateThisFrame = true;
		}

		private void Update()
		{
			_framesCount++;
			_framesDeltaSum += Time.deltaTime;
			if (Time.deltaTime < _minDeltaTimeForAvg)
			{
				_minDeltaTimeForAvg = Time.deltaTime;
			}
			if (Time.deltaTime > _maxDeltaTimeForAvg)
			{
				_maxDeltaTimeForAvg = Time.deltaTime;
			}
			if (_wasFixedUpdateLastFrame)
			{
				_wasFixedUpdateLastFrame = false;
				_physFramesCount++;
				_physFramesDeltaSum += Time.deltaTime;
			}
			if (_isFixedUpdateThisFrame)
			{
				_wasFixedUpdateLastFrame = true;
				_isFixedUpdateThisFrame = false;
			}
			if (Time.unscaledTime - _timeOfLastPoll > PollingRate)
			{
				float num = 1f / (_physFramesDeltaSum / (float)_physFramesCount);
				AvgFPS.text = GetNumberString(Mathf.RoundToInt(1f / (_framesDeltaSum / (float)_framesCount)));
				AvgFPSMin.text = GetNumberString(Mathf.RoundToInt(1f / _maxDeltaTimeForAvg));
				AvgFPSMax.text = GetNumberString(Mathf.RoundToInt(1f / _minDeltaTimeForAvg));
				PhysicsFPS.text = GetNumberString(Mathf.RoundToInt(num));
				if (OnPhysicsFPSReady != null)
				{
					OnPhysicsFPSReady(num);
				}
				_physFramesDeltaSum = 0f;
				_physFramesCount = 0;
				_framesDeltaSum = 0f;
				_framesCount = 0;
				_minDeltaTimeForAvg = float.PositiveInfinity;
				_maxDeltaTimeForAvg = float.NegativeInfinity;
				_timeOfLastPoll = Time.unscaledTime;
			}
			PhysicsRate.text = GetNumberString(Mathf.RoundToInt(1f / Time.fixedDeltaTime));
		}

		public string GetNumberString(int fps)
		{
			if (fps < FramerateStrings.Length - 1 && fps >= 0)
			{
				return FramerateStrings[fps];
			}
			return FramerateStrings[FramerateStrings.Length - 1];
		}
	}
}
