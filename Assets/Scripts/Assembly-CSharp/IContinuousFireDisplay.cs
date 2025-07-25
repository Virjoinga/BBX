public interface IContinuousFireDisplay
{
	void Toggle(bool isOn);

	void DisplayFor(float duration);

	void CancelDisplayFor();
}
