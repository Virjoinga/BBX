using UnityEngine;

[CreateAssetMenu]
public class RampAsset : ScriptableObject
{
	public Gradient gradient = new Gradient();

	public int size = 16;

	public bool up;

	public bool overwriteExisting = true;
}
