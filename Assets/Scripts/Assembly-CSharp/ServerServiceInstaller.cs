using BSCore;
using UnityEngine;
using Zenject;

public class ServerServiceInstaller : MonoInstaller
{
	[SerializeField]
	private GameConfigData _gameConfig;

	[SerializeField]
	protected EmoticonData _emoticonData;

	public override void InstallBindings()
	{
	}
}
