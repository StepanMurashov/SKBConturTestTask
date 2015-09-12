using System;

using Sonic.Net.DataStructures.LockFree;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Factory class used to create poolable Task objects.
	/// </summary>
	public abstract class TaskFactory : PoolableObjectFactory
	{
		#region Public Constructor(s)
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public TaskFactory()
		{
			_objTaskPool = new ObjectPool(this,true);
		}

		#endregion

        #region Public Methods

        /// <summary>
		/// Add the completed task object to the task object pool
		/// </summary>
		/// <param name="t">Task object that completed execution</param>
		public void Done(Task t)
		{
			_objTaskPool.AddToPool(t);
		}

		#endregion

		#region Private Data Members

		protected ObjectPool _objTaskPool;

		#endregion
	}
}
