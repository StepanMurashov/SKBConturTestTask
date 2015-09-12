using System;
using System.Threading;

namespace Sonic.Net.DataStructures.LockFree
{
	/// <summary>
	/// Factory class to create new instances of the Node type
	/// </summary>
	class NodePoolFactory : PoolableObjectFactory
	{
		/// <summary>
		/// Creates a new instance of poolable Node type
		/// </summary>
		/// <returns>New poolable Node object</returns>
		public override PoolableObject CreatePoolableObject()
		{
			return new Node();
		}
	}
	/// <summary>
	/// Internal class used by all other data structures
	/// </summary>
	class Node : PoolableObject
	{
		public Node()
		{
			Init(null);
		}
		public Node(object data)
		{
			Init(data);
		}
		public override void Initialize()
		{
			Init(null);
		}
		private void Init(object data)
		{
			Data = data;
			NextNode = null;
		}
		public object Data;
		public object NextNode;
	}
	/// <summary>
	/// Lock Free Queue
	/// </summary>
	public class Queue
	{
		/// <summary>
		/// Creates a new instance of Lock-Free Queue
		/// </summary>
		public Queue()
		{
			Init(0);
		}

		/// <summary>
		/// Creates a new instance of Lock-Free Queue with n-number of 
		/// pre-created nodes to hold objects queued on to this instance.
		/// </summary>
		public Queue(int nodeCount)
		{
			Init(nodeCount);
		}

		public void Enqueue(object data)
		{
			Node tempTail = null;
			Node tempTailNext = null;
			Node newNode = _nodePool.GetObject() as Node; //new Node(data);
			newNode.Data = data;
			do
			{
				tempTail = _tail as Node;
				tempTailNext = tempTail.NextNode as Node;
				if (tempTail == _tail)
				{
					if (tempTailNext == null)
					{
						// If the tail node we are referring to is really the last
						// node in the queue (i.e. its next node is null), then
						// try to point its next node to our new node
						//
						if (Interlocked.CompareExchange(ref tempTail.NextNode,newNode,tempTailNext) == tempTailNext)
							break;
					}
					else
					{
						// This condition occurs when we have failed to update
						// the tail's next node. And the next time we try to update
						// the next node, the next node is pointing to a new node
						// updated by other thread. But the other thread has not yet
						// re-pointed the tail to its new node.
						// So we try to re-point to the tail node to the next node of the
						// current tail
						//
						Interlocked.CompareExchange(ref _tail,tempTailNext,tempTail);
					}
				}
			} while (true);

			// If we were able to successfully change the next node of the current tail node
			// to point to our new node, then re-point the tail node also to our new node
			//
			Interlocked.CompareExchange(ref _tail,newNode,tempTail);
			Interlocked.Increment(ref _count);
		}
		public object Dequeue(ref bool empty)
		{
			object data = null;
			Node tempTail = null;
			Node tempHead = null;
			Node tempHeadNext = null;
			do
			{
				tempHead = _head as Node;
				tempTail = _tail as Node;
				tempHeadNext = tempHead.NextNode as Node;
				if (tempHead == _head)
				{
					// There may not be any elements in the queue
					//
					if (tempHead == tempTail)
					{
						if (tempHeadNext == null)
						{
							// If the queue is really empty come out of dequeue operation
							//
							empty = true;
							return null;
						}
						else
						{
							// Some other thread could be in the middle of the
							// enqueue operation. it could have changed the next node of the tail
							// to point to the new node.
							// So let us advance the tail node to point to the next node of the
							// current tail
							Interlocked.CompareExchange(ref _tail,tempHeadNext,tempTail);
						}
					}
					else
					{
						// Move head one element down. 
						// If succeeded Try to get the data from head and
						// break out of the loop.
						//
                        if (Interlocked.CompareExchange(ref _head, tempHeadNext, tempHead) == tempHead)
                        {
                            data = tempHeadNext.Data;
                            break;
                        }
					}
				}
			} while (true);
			Interlocked.Decrement(ref _count);
			tempHead.Data = null;
			_nodePool.AddToPool(tempHead);
			return data;
		}
		public void Clear()
		{
			Init(0);
		}
		public void Clear(int nodeCount)
		{
			Init(nodeCount);
		}
		public long Count
		{
			get
			{
				return _count;
			}
		}

		private void Init(int nodeCount)
		{
			_count = 0;
			if (_nodePool != null) 
				_nodePool.Clear(nodeCount);
			else
				_nodePool = new ObjectPool(new NodePoolFactory(),true,nodeCount);
			_head = _tail = _nodePool.GetObject();//new Node();
		}
		private object _head;
		private object _tail;
		private long _count = 0;
		private ObjectPool _nodePool = null;
	}
}
