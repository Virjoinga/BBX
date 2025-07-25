using UnityEngine;

public class StretchLineRendererBetweenNeckAndHead : StretchLineRenderBetweenTransforms
{
	protected override void Start()
	{
		HuggableOutfit componentInParent = GetComponentInParent<HuggableOutfit>();
		_transforms = new Transform[2] { base.transform, componentInParent.RemovableHeadPositioner };
		base.Start();
	}
}
