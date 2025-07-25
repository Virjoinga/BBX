using UnityEngine;

public static class LayerMaskExtensions
{
	public static int Flag(this LayerMask mask)
	{
		return 1 << mask.value;
	}

	public static bool ContainsLayer(this LayerMask mask, int layer)
	{
		return (int)mask == ((int)mask | (1 << layer));
	}
}
