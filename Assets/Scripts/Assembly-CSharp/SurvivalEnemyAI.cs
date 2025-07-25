using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class SurvivalEnemyAI : MonoBehaviour
{
	[SerializeField]
	private NavMeshAgent _navMeshAgent;

	[SerializeField]
	private NavMeshObstacle _navMeshObstacle;

	[SerializeField]
	private BehaviorTree _behaviorTree;

	[SerializeField]
	private Animator _animator;

	private Vector3 _lastValidPosition;

	public float Acceleration
	{
		get
		{
			return _navMeshAgent.acceleration;
		}
		set
		{
			_navMeshAgent.acceleration = value;
		}
	}

	public float CurrentSpeed => _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;

	public bool IsPaused { get; set; }

	public float MaxSpeed { get; set; } = 6f;

	public float AngularSpeed { get; set; } = 360f;

	private void Start()
	{
		_lastValidPosition = base.transform.position;
	}

	public void Disable()
	{
		Object.Destroy(_navMeshAgent);
		Object.Destroy(_navMeshObstacle);
		Object.Destroy(_behaviorTree);
		Object.Destroy(this);
	}

	public void SetTarget(GameObject attacker)
	{
		_behaviorTree.SendEvent("Attacked", (object)attacker);
	}

	private void FixedUpdate()
	{
		_animator.SetFloat("Speed", CurrentSpeed);
		if (_navMeshAgent.enabled)
		{
			if (_navMeshAgent.isOnNavMesh)
			{
				_lastValidPosition = base.transform.position;
				return;
			}
			Debug.LogError("[SurvivalEnemyAI] AI Off Nav Mesh! Resetting to last valid position");
			base.transform.position = _lastValidPosition;
		}
	}
}
