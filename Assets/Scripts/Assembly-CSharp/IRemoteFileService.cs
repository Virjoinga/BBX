using System;

public interface IRemoteFileService
{
	void FetchFile(string fileId, Action<byte[]> onSuccess, Action<FailureReasons> onFailure);
}
