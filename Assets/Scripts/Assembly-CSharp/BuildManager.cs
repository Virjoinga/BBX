using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildManager : MonoBehaviour
{
	public GameObject WebGLCanvas;

	public GameObject WarningPanel;

	private void Awake()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			SceneManager.LoadScene(0);
		}
	}
}
