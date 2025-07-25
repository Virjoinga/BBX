using UnityEngine;

[CreateAssetMenu(fileName = "LayerMaskConfig", menuName = "Data/Layer Mask Config")]
public class LayerMaskConfig : ScriptableObject
{
	public enum Group
	{
		Hitable = 0,
		Affectable = 1,
		Ground = 2
	}

	public const int ALWAYS_RENDER_LAYER = 19;

	public const int PLAYER_LAYER = 9;

	private static LayerMaskConfig _instanceCache;

	[SerializeField]
	[Tooltip("Layers that can be hit by raycasts and projectiles")]
	private LayerMask _hitableLayers = 0;

	[SerializeField]
	[Tooltip("Layers that can be affected by weapons and deployables")]
	private LayerMask _affectableLayers = 0;

	[SerializeField]
	[Tooltip("Layers that players collider with")]
	private LayerMask _groundLayers = 0;

	[SerializeField]
	[Tooltip("Layers that survival enemies are on")]
	private LayerMask _survivalEnemyLayers = 0;

	public static LayerMask HitableLayers => _instance._hitableLayers;

	public static LayerMask AffectableLayers => _instance._affectableLayers;

	public static LayerMask GroundLayers => _instance._groundLayers;

	public static LayerMask SurvivalEnemyLayers => _instance._survivalEnemyLayers;

	private static LayerMaskConfig _instance
	{
		get
		{
			if (_instanceCache == null)
			{
				_instanceCache = Resources.Load<LayerMaskConfig>("Config/LayerMaskConfig");
			}
			return _instanceCache;
		}
	}

	public static LayerMask GetLayerMask(Group group)
	{
		switch (group)
		{
		case Group.Affectable:
			return AffectableLayers;
		case Group.Ground:
			return GroundLayers;
		default:
			return HitableLayers;
		}
	}
}
