using System.Threading.Tasks;

namespace PlayFab.Pipeline
{
	public interface IEventPipeline
	{
		Task StartAsync();

		bool IntakeEvent(IPlayFabEmitEventRequest request);

		Task<IPlayFabEmitEventResponse> IntakeEventAsync(IPlayFabEmitEventRequest request);
	}
}
