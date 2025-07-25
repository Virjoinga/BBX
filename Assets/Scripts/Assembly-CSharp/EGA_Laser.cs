using UnityEngine;

public class EGA_Laser : MonoBehaviour
{
	public GameObject HitEffect;

	public float HitOffset;

	public float MaxLength;

	private LineRenderer Laser;

	public float MainTextureLength = 1f;

	public float NoiseTextureLength = 1f;

	private Vector4 Length = new Vector4(1f, 1f, 1f, 1f);

	private Vector4 LaserSpeed = new Vector4(0f, 0f, 0f, 0f);

	private Vector4 LaserStartSpeed;

	private bool LaserSaver;

	private bool UpdateSaver;

	private ParticleSystem[] Effects;

	private ParticleSystem[] Hit;

	private void Start()
	{
		Laser = GetComponent<LineRenderer>();
		Effects = GetComponentsInChildren<ParticleSystem>();
		Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
		if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW"))
		{
			LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
		}
		LaserSpeed = LaserStartSpeed;
	}

	private void Update()
	{
		if (!(Laser != null) || UpdateSaver)
		{
			return;
		}
		Laser.SetPosition(0, base.transform.position);
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward), out var hitInfo, MaxLength))
		{
			Laser.SetPosition(1, hitInfo.point);
			HitEffect.transform.position = hitInfo.point + hitInfo.normal * HitOffset;
			HitEffect.transform.rotation = Quaternion.identity;
			ParticleSystem[] effects = Effects;
			foreach (ParticleSystem particleSystem in effects)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
			Length[0] = MainTextureLength * Vector3.Distance(base.transform.position, hitInfo.point);
			Length[2] = NoiseTextureLength * Vector3.Distance(base.transform.position, hitInfo.point);
			LaserSpeed[0] = LaserStartSpeed[0] * 4f / Vector3.Distance(base.transform.position, hitInfo.point);
			LaserSpeed[2] = LaserStartSpeed[2] * 4f / Vector3.Distance(base.transform.position, hitInfo.point);
		}
		else
		{
			Vector3 vector = base.transform.position + base.transform.forward * MaxLength;
			Laser.SetPosition(1, vector);
			HitEffect.transform.position = vector;
			ParticleSystem[] effects = Hit;
			foreach (ParticleSystem particleSystem2 in effects)
			{
				if (particleSystem2.isPlaying)
				{
					particleSystem2.Stop();
				}
			}
			Length[0] = MainTextureLength * Vector3.Distance(base.transform.position, vector);
			Length[2] = NoiseTextureLength * Vector3.Distance(base.transform.position, vector);
			LaserSpeed[0] = LaserStartSpeed[0] * 4f / Vector3.Distance(base.transform.position, vector);
			LaserSpeed[2] = LaserStartSpeed[2] * 4f / Vector3.Distance(base.transform.position, vector);
		}
		if (!Laser.enabled && !LaserSaver)
		{
			LaserSaver = true;
			Laser.enabled = true;
		}
	}

	public void DisablePrepare()
	{
		if (Laser != null)
		{
			Laser.enabled = false;
		}
		UpdateSaver = true;
		if (Effects == null)
		{
			return;
		}
		ParticleSystem[] effects = Effects;
		foreach (ParticleSystem particleSystem in effects)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
	}
}
