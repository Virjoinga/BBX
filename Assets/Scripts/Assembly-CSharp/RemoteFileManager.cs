using System;
using Zenject;

public class RemoteFileManager
{
	private IRemoteFileService _remoteFileService;

	[Inject]
	public RemoteFileManager(IRemoteFileService remoteFileService)
	{
		_remoteFileService = remoteFileService;
	}

	public void FetchFile(string fileId, Action<byte[]> onSuccess, Action<FailureReasons> onFailure)
	{
		_remoteFileService.FetchFile(fileId, onSuccess, onFailure);
	}
}
