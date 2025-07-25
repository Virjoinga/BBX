using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomizeAnimationStartTime : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Animator>().Play(0, -1, Random.value);
	}
}
