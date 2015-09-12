using System;

using Sonic.Net.DataStructures.LockFree;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Base class for all Task objects that can be processed by ThreadPool.
	/// Objects of this class are poolable.
	/// </summary>
	public abstract class Task : PoolableObject, ITask
	{
		#region Public Constructor(s)

		/// <summary>
		/// Default constructor
		/// </summary>
		public Task()
		{
		}

		#endregion

		#region ITask Public Methods
		
		public abstract void Execute(ThreadPool tp);
		public abstract void Done();
		
		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Unique identification of the task object
		/// </summary>
		public object Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// User object if any that is associated with this task object
		/// </summary>
		public abstract object UserObject
		{
			get;set;
		}

		#endregion

		#region Protected Data Members

		protected object _id;
		protected bool _active = true;

		#endregion
	}
}
