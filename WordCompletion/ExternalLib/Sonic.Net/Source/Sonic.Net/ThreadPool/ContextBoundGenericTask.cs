using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Class that provides a generic Task object to be used with ThreadPool
	/// </summary>
	public abstract class ContextBoundGenericTask : Task
	{
		#region Public Constructor(s)

		/// <summary>
		/// Default constructor
		/// </summary>
		public ContextBoundGenericTask()
		{
			// No-Op
		}

		#endregion

		#region Protected Methods
		
		internal virtual void InitializeTask(object id,object obj,IContext ctx,ContextBoundGenericTaskFactory tf)
		{
			_id = id;
			_object = obj;
			_ctx = ctx;
			_tf = tf;
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Context to which this task instance belongs to.
		/// </summary>
		public IContext Context
		{
			get
			{
				return _ctx;
			}
		}
		public ContextBoundGenericTaskFactory TaskFactory
		{
			get
			{
				return _tf;
			}
		}

		#endregion

		/// <summary>
		/// Get/Set the user object associated with this Task instance 
		/// </summary>
		public override object UserObject
		{
			get
			{
				return _object;
			}
			set
			{
				_object = value;
			}
		}

		#region ITask Public Methods

		public override void Done()
		{
			_tf.Done(this);
		}

		#endregion

		#region Private Data Members

		protected object _object;
		protected IContext _ctx;
		protected ContextBoundGenericTaskFactory _tf;

		#endregion
	}
}
