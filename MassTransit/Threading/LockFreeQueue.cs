// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Threading
{
	using System.Threading;

	/// <summary>
	/// A lock-free queue
	/// Based on text read at: http://bytes.com/forum/thread247361.html
	/// William Stacey (MVP)
	/// </summary>
	public class LockFreeQueue<T>
	{
		private Node _head;
		private Node _tail;

		public LockFreeQueue()
		{
			Node node = new Node(default(T));

			_head = node;
			_tail = node;
		}

		public bool Dequeue(out T value)
		{
			Node head;
			Node tail;
			Node next;

			while (true)
			{
				head = _head;
				tail = _tail;
				next = head.Next;

				// Are head, tail, and next consistent?
				if (ReferenceEquals(_head, head))
				{
					// is tail falling behind
					if (ReferenceEquals(head.Next, tail.Next))
					{
						// is the queue empty?
						if (ReferenceEquals(next, null))
						{
							value = default(T);
							return false;
						}

						Interlocked.CompareExchange(ref _tail, next.Next, tail);
					}
					else // No need to deal with tail
					{
						// read value before CAS otherwise another deque might try to free the next node
						value = next.Value;

						// try to swing the head to the next node
						if (Interlocked.CompareExchange(ref _head, next, head) == head)
						{
							return true;
						}
					}
				}
			}
		}

		public void Enqueue(T value)
		{
			Node valueNode = new Node(value);

			while (true)
			{
				Node tail = _tail;
				Node next = tail.Next;

				// are tail and next consistent
				if (ReferenceEquals(tail, _tail))
				{
					// was tail pointing to the last node?
					if (ReferenceEquals(next, null))
					{
						if (ReferenceEquals(Interlocked.CompareExchange(ref tail.Next, valueNode, next), next))
						{
							Interlocked.CompareExchange(ref _tail, valueNode, tail);
							break;
						}
					}
					else // tail was not pointing to last node
					{
						// try to swing Tail to the next node
						Interlocked.CompareExchange(ref _tail, next, tail);
					}
				}
			}
		}

		private class Node
		{
			internal readonly T Value;
			internal Node Next;

			internal Node(T value)
			{
				Value = value;
			}
		}
	}
}