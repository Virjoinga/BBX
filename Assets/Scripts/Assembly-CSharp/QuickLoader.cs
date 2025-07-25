using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickLoader : MonoBehaviour
{
	private IEnumerator Start()
	{
		Debug.Log("[QuickLoader] Scene loaded...");
		yield return null;
		SceneManager.LoadScene("ClientBootloader");
	}
}
