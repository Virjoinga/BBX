using System.Collections;
using UnityEngine;

public class NecromancerController : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Vector2 _minMaxTimeBetweenAttacks;

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _spawnClip;

	[SerializeField]
	private AudioClip _laughClip;

	private float _nextTimeToAttack;

	private float _timer;

	private void OnEnable()
	{
		_nextTimeToAttack = Random.Range(_minMaxTimeBetweenAttacks.x, _minMaxTimeBetweenAttacks.y);
		StartCoroutine(PlaySpawnSFX());
	}

	private IEnumerator PlaySpawnSFX()
	{
		_audioSource.PlayOneShot(_spawnClip);
		yield return new WaitForSeconds(_spawnClip.length);
		_audioSource.PlayOneShot(_laughClip, 0.4f);
	}

	private void Update()
	{
		_timer += Time.deltaTime;
		if (_timer >= _nextTimeToAttack)
		{
			int value = Random.Range(1, 5);
			_animator.SetInteger("RandAttackIndex", value);
			_animator.SetTrigger("PlayAttack");
			_nextTimeToAttack = Random.Range(_minMaxTimeBetweenAttacks.x, _minMaxTimeBetweenAttacks.y);
			_timer = 0f;
		}
	}
}
