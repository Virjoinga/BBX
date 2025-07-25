using UnityEngine;
using UnityEngine.Events;

public struct GenericButtonDetails
{
	public string Text;

	public UnityAction Action;

	public Color32 Color;

	public bool IsInitialized;

	public GenericButtonDetails(string text, UnityAction action, Color32 color)
	{
		Text = text;
		Action = action;
		Color = color;
		IsInitialized = true;
	}
}
