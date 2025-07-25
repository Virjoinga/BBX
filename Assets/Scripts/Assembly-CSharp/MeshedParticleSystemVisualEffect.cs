using UnityEngine;

public class MeshedParticleSystemVisualEffect : BaseVisualEffect
{
	[SerializeField]
	private ParticleSystem[] _meshedParticleSystems;

	public override void Setup(Outfit outfit)
	{
		ParticleSystem[] meshedParticleSystems = _meshedParticleSystems;
		for (int i = 0; i < meshedParticleSystems.Length; i++)
		{
			ParticleSystem.ShapeModule shape = meshedParticleSystems[i].shape;
			shape.skinnedMeshRenderer = outfit.Mesh;
		}
	}
}
