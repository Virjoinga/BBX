using System;
using System.Collections.Generic;

namespace RSG
{
	public class PromiseTimer : IPromiseTimer
	{
		private float curTime;

		private int curFrame;

		private readonly LinkedList<PredicateWait> waiting = new LinkedList<PredicateWait>();

		public IPromise WaitFor(float seconds)
		{
			return WaitUntil((TimeData t) => t.elapsedTime >= seconds);
		}

		public IPromise WaitWhile(Func<TimeData, bool> predicate)
		{
			return WaitUntil((TimeData t) => !predicate(t));
		}

		public IPromise WaitUntil(Func<TimeData, bool> predicate)
		{
			Promise promise = new Promise();
			PredicateWait value = new PredicateWait
			{
				timeStarted = curTime,
				pendingPromise = promise,
				timeData = default(TimeData),
				predicate = predicate,
				frameStarted = curFrame
			};
			waiting.AddLast(value);
			return promise;
		}

		public bool Cancel(IPromise promise)
		{
			LinkedListNode<PredicateWait> linkedListNode = FindInWaiting(promise);
			if (linkedListNode == null)
			{
				return false;
			}
			linkedListNode.Value.pendingPromise.Reject(new PromiseCancelledException("Promise was cancelled by user."));
			waiting.Remove(linkedListNode);
			return true;
		}

		private LinkedListNode<PredicateWait> FindInWaiting(IPromise promise)
		{
			for (LinkedListNode<PredicateWait> linkedListNode = waiting.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.pendingPromise.Id.Equals(promise.Id))
				{
					return linkedListNode;
				}
			}
			return null;
		}

		public void Update(float deltaTime)
		{
			curTime += deltaTime;
			curFrame++;
			LinkedListNode<PredicateWait> linkedListNode = waiting.First;
			while (linkedListNode != null)
			{
				PredicateWait value = linkedListNode.Value;
				float num = curTime - value.timeStarted;
				value.timeData.deltaTime = num - value.timeData.elapsedTime;
				value.timeData.elapsedTime = num;
				int elapsedUpdates = curFrame - value.frameStarted;
				value.timeData.elapsedUpdates = elapsedUpdates;
				bool flag;
				try
				{
					flag = value.predicate(value.timeData);
				}
				catch (Exception ex)
				{
					value.pendingPromise.Reject(ex);
					linkedListNode = RemoveNode(linkedListNode);
					continue;
				}
				if (flag)
				{
					value.pendingPromise.Resolve();
					linkedListNode = RemoveNode(linkedListNode);
				}
				else
				{
					linkedListNode = linkedListNode.Next;
				}
			}
		}

		private LinkedListNode<PredicateWait> RemoveNode(LinkedListNode<PredicateWait> node)
		{
			LinkedListNode<PredicateWait> node2 = node;
			node = node.Next;
			waiting.Remove(node2);
			return node;
		}
	}
}
