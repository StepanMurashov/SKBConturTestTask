using System;

using Sonic.Net.DataStructures.LockFree;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Associates a TaskHandler with a set of Tasks. It also creates
	/// a new instance of a Task that uses the associated TaskHandler
	/// </summary>
	public abstract class ContextBoundGenericTaskFactory : TaskFactory
	{
		#region Public Constructor(s)
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public ContextBoundGenericTaskFactory()
		{
			// No-Op
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new Context Bound Task object .or. get an available Context Bound Task
		/// object from the object pool maintained by this instance of the
		/// Context Bound Task Factory.
		/// </summary>
		/// <param name="obj">User object that need to be associated with the new Context Bound Task instance</param>
		/// <returns>New Context Bound Task object</returns>
		public ContextBoundGenericTask NewContextBoundGenericTask(object id,object obj,IContext ctx)
		{
			ContextBoundGenericTask task = _objTaskPool.GetObject() as ContextBoundGenericTask;
			task.InitializeTask(id,obj,ctx,this);
			return task;
		}

		#endregion
	}
}
