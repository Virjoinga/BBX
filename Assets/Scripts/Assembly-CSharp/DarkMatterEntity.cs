using UnityEngine;

public class DarkMatterEntity : SpawnedEntityApplyEffectToDamageablesInRange<IDarkMatter>
{
	[SerializeField]
	private ParticleSystem _damagingParticleEffect;

	[SerializeField]
	private ParticleSystem _friendlyParticleEffect;

	protected override void OnRemoteOnlyAttached()
	{
		base.state.AddCallback("OwnerTeam", OnOwnerTeamUpdated);
		base.state.AddCallback("Scale", OnScaleUpdated);
	}

	private void OnOwnerTeamUpdated()
	{
		if (PlayerController.LocalPlayer.entity != base.state.Owner && PlayerController.LocalPlayer.Team == base.state.OwnerTeam)
		{
			_friendlyParticleEffect.Play();
		}
		else
		{
			_damagingParticleEffect.Play();
		}
	}

	private void OnScaleUpdated()
	{
		base.transform.localScale = Vector3.one * base.state.Scale;
	}
}
