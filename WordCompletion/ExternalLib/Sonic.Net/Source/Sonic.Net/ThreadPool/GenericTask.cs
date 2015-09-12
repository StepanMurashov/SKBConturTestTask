using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Class that provides a generic Task object to be used with ThreadPool
	/// </summary>
	public abstract class GenericTask : Task
	{
		#region Public Constructor(s)

		/// <summary>
		/// Default constructor
		/// </summary>
		public GenericTask()
		{
			// No-Op
		}

		#endregion

		#region Protected Methods
		
		internal virtual void InitializeTask(object id,object obj,GenericTaskFactory tf)
		{
			_id = id;
			_object = obj;
			_tf = tf;
		}
		
		#endregion

		#region Public Methods
		
		public GenericTaskFactory TaskFactory
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

		private object _object;
		private GenericTaskFactory _tf;

		#endregion
	}
}
