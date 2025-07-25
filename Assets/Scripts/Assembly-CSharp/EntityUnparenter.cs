using System.Collections;
using UnityEngine;

public class EntityUnparenter : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return null;
		base.transform.SetParent(null, worldPositionStays: true);
	}
}
