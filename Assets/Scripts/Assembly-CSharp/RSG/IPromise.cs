using System;
using System.Collections.Generic;

namespace RSG
{
	public interface IPromise<PromisedT>
	{
		int Id { get; }

		IPromise<PromisedT> WithName(string name);

		void Done(Action<PromisedT> onResolved, Action<Exception> onRejected);

		void Done(Action<PromisedT> onResolved);

		void Done();

		IPromise Catch(Action<Exception> onRejected);

		IPromise<PromisedT> Catch(Func<Exception, PromisedT> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved);

		IPromise Then(Func<PromisedT, IPromise> onResolved);

		IPromise Then(Action<PromisedT> onResolved);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved, Func<Exception, IPromise<ConvertedT>> onRejected);

		IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected);

		IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved, Func<Exception, IPromise<ConvertedT>> onRejected, Action<float> onProgress);

		IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress);

		IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected, Action<float> onProgress);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform);

		IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain);

		IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain);

		IPromise<PromisedT> Finally(Action onComplete);

		IPromise ContinueWith(Func<IPromise> onResolved);

		IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete);

		IPromise<PromisedT> Progress(Action<float> onProgress);
	}
	public interface IPromise
	{
		int Id { get; }

		IPromise WithName(string name);

		void Done(Action onResolved, Action<Exception> onRejected);

		void Done(Action onResolved);

		void Done();

		IPromise Catch(Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved);

		IPromise Then(Func<IPromise> onResolved);

		IPromise Then(Action onResolved);

		IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved, Func<Exception, IPromise<ConvertedT>> onRejected);

		IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected);

		IPromise Then(Action onResolved, Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved, Func<Exception, IPromise<ConvertedT>> onRejected, Action<float> onProgress);

		IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress);

		IPromise Then(Action onResolved, Action<Exception> onRejected, Action<float> onProgress);

		IPromise ThenAll(Func<IEnumerable<IPromise>> chain);

		IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenSequence(Func<IEnumerable<Func<IPromise>>> chain);

		IPromise ThenRace(Func<IEnumerable<IPromise>> chain);

		IPromise<ConvertedT> ThenRace<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise Finally(Action onComplete);

		IPromise ContinueWith(Func<IPromise> onResolved);

		IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete);

		IPromise Progress(Action<float> onProgress);
	}
}
