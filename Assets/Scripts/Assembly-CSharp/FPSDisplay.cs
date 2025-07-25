using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	[SerializeField]
	[Range(0.02f, 0.2f)]
	private float _fontSize = 0.02f;

	[SerializeField]
	private Color _fontColor = new Color(0f, 0f, 0.5f, 1f);

	[SerializeField]
	private Vector2 _rectOffset = Vector2.zero;

	[SerializeField]
	private bool _showMSEC;

	private float deltaTime;

	private void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI()
	{
		int width = Screen.width;
		int height = Screen.height;
		GUIStyle gUIStyle = new GUIStyle();
		float num = (float)height * _fontSize;
		Rect position = new Rect(_rectOffset.x, (float)height - num - _rectOffset.y, width, num);
		gUIStyle.alignment = TextAnchor.UpperRight;
		gUIStyle.fontSize = (int)num;
		gUIStyle.normal.textColor = _fontColor;
		float num2 = deltaTime * 1000f;
		float num3 = 1f / deltaTime;
		string text = $"{num3:0.} fps";
		if (_showMSEC)
		{
			text += $" | {num2:0.0} ms";
		}
		text += $" | {Application.version}";
		GUI.Label(position, text, gUIStyle);
	}
}
