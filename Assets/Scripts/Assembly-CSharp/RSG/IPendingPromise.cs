namespace RSG
{
	public interface IPendingPromise<PromisedT> : IRejectable
	{
		int Id { get; }

		void Resolve(PromisedT value);

		void ReportProgress(float progress);
	}
	public interface IPendingPromise : IRejectable
	{
		int Id { get; }

		void Resolve();

		void ReportProgress(float progress);
	}
}
