using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab.Pipeline;

namespace PlayFab
{
	public interface IPlayFabEventRouter
	{
		IDictionary<EventPipelineKey, IEventPipeline> Pipelines { get; }

		Task AddAndStartPipeline(EventPipelineKey eventPipelineKey, IEventPipeline eventPipeline);

		IEnumerable<Task<IPlayFabEmitEventResponse>> RouteEvent(IPlayFabEmitEventRequest request);
	}
}
