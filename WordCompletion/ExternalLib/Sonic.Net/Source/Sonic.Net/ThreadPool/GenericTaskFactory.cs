using System;

using Sonic.Net.DataStructures.LockFree;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Factory class for creating Generic Task objects. It mantains a pool
	/// of Generic Task objects.
	/// </summary>
	public abstract class GenericTaskFactory : TaskFactory
	{
		#region Public Constructor(s)
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public GenericTaskFactory()
		{
			// No-Op
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new Generic Task object .or. get an available Generic Task
		/// object from the object pool maintained by this instance of the
		/// Generic Task Factory.
		/// </summary>
		/// <param name="obj">User object that need to be associated with the new Generic Task instance</param>
		/// <returns>New Generic Task object</returns>
		public GenericTask NewGenericTask(object id,object obj)
		{
			GenericTask task = _objTaskPool.GetObject() as GenericTask;
			task.InitializeTask(id,obj,this);
			return task;
		}

		#endregion
	}
}
