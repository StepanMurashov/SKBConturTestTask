using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Class that provides a ContextBound Task object to be used with ThreadPool
	/// </summary>
	public class WaitableContextBoundTask : Task
	{
		#region Public Constructor(s)

		/// <summary>
		/// Creates a new instance of the Task
		/// </summary>
		/// <param name="obj">User object to be used by this Task during its execution</param>
		/// <param name="hTask">TaskHandler used to execute this Task</param>
		/// <param name="tf">TaskFactory object from which this Task is created</param>
		public WaitableContextBoundTask(IContext ctx,object id,object obj,ContextBoundTaskHandler hTask,ContextBoundTaskFactory tf)
			: base(id)
		{
			_ctx = ctx;
			_object = obj;
			_hTask = hTask;
			_tf = tf;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Get/Set the TaskHandler used by this Task during its execution
		/// </summary>
		public ContextBoundTaskHandler TaskHandler
		{
			get
			{
				return _hTask;
			}
			set
			{
				_hTask = value;
			}
		}
		
		public ContextBoundTaskFactory TaskFactory
		{
			get
			{
				return _tf;
			}
		}

		public bool Wait(int waitMilli)
		{
			return _ev.WaitOne(waitMilli,false);
		}

		#endregion

		#region Task Class Public Overrides

		/// <summary>
		/// Get/Set the user object used by this Task during its execution
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

		#endregion

		#region ITask Public Methods

		public override void Execute(ThreadPool tp)
		{
			try
			{
				_ctx.Lock();
				_hTask(_ctx,this,tp);
				_ev.Set();
			}
			catch(Exception e)
			{
				throw e;
			}
			finally
			{
				_ctx.UnLock();
			}
		}

		public override void Done()
		{
			// <TODO>
		}

		#endregion

		#region Private Data Members

		private IContext _ctx;
		private object _object;
		private ContextBoundTaskHandler _hTask;
		private ContextBoundTaskFactory _tf;
		private AutoResetEvent _ev = new AutoResetEvent(false);

		#endregion
	}
}
