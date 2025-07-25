using System.Collections;
using UnityEngine;

public class PlayerSpawnPoint : SpawnPoint
{
	[SerializeField]
	private TeamId _team = TeamId.Neutral;

	[SerializeField]
	private int _id;

	[SerializeField]
	protected float _sphereOfInfluence = 2f;

	[SerializeField]
	protected Color _color = Color.black;

	[SerializeField]
	private bool _primarySpawn = true;

	private float _coolDownTime = 5f;

	public string UniqueId => string.Concat(_team, _id);

	public TeamId Team => _team;

	public bool PrimarySpawn => _primarySpawn;

	public bool SpawnCooldownActive { get; private set; }

	public float SphereOfInfluence => _sphereOfInfluence;

	public void StartSpawnCooldown()
	{
		StopAllCoroutines();
		StartCoroutine(SpawnCooldownRoutine());
	}

	private IEnumerator SpawnCooldownRoutine()
	{
		SpawnCooldownActive = true;
		yield return new WaitForSeconds(_coolDownTime);
		SpawnCooldownActive = false;
	}

	public void ResetCooldown()
	{
		StopAllCoroutines();
		SpawnCooldownActive = false;
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawRay(base.transform.position, base.transform.forward);
		Gizmos.color = _color;
		if (_team != TeamId.Neutral)
		{
			switch (_team)
			{
			case TeamId.Team1:
				Gizmos.color = Color.blue;
				break;
			case TeamId.Team2:
				Gizmos.color = Color.red;
				break;
			default:
				Gizmos.color = Color.black;
				break;
			}
		}
		Gizmos.DrawWireSphere(base.transform.position, _sphereOfInfluence);
	}
}
