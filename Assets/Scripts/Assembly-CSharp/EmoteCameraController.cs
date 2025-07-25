using System.Collections;
using Cinemachine;
using UnityEngine;

public class EmoteCameraController : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera _emoteCamera;

	public IEnumerator EmoteRoutine(float length)
	{
		_emoteCamera.Priority = 99;
		yield return new WaitForSeconds(length);
		_emoteCamera.Priority = 0;
	}
}
