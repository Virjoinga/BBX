using UnityEngine;

public class ResizeLightRanges : MonoBehaviour
{
	[SerializeField]
	private float _intensityMultiplier = 1.2f;

	[ContextMenu("Resize All Lights")]
	private void ResizeAllLights()
	{
		Light[] componentsInChildren = GetComponentsInChildren<Light>();
		float num = 0.016f;
		Light[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].range *= num;
		}
		Debug.Log($"[ResizeAllLights] Updated range of {componentsInChildren.Length} lights");
	}

	[ContextMenu("Increase Lights Intensity")]
	private void IncreaseLightsIntensity()
	{
		Light[] componentsInChildren = GetComponentsInChildren<Light>();
		Light[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].intensity *= _intensityMultiplier;
		}
		Debug.Log($"[ResizeAllLights] Updated intensity of {componentsInChildren.Length} lights");
	}
}
