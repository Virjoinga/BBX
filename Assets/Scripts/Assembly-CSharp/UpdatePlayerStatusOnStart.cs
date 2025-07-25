using NodeClient;
using UnityEngine;
using Zenject;

public class UpdatePlayerStatusOnStart : MonoBehaviour
{
	[SerializeField]
	private PlayerStatus _status;

	private void Start()
	{
		ProjectContext.Instance.Container.Resolve<SocketClient>().UpdateStatus(_status);
	}
}
