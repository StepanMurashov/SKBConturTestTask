using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Class that provides a generic Task object to be used with ThreadPool
	/// </summary>
	public abstract class WaitableContextBoundGenericTask : ContextBoundGenericTask
	{
		#region Public Constructor(s)
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public WaitableContextBoundGenericTask()
		{
			// No-Op
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Wait till the current Context Bound Generic Task instance is executed by the
		/// Thread Pool. User can specify a time-out for the wait operation. 
		/// </summary>
		/// <param name="msWaitTime">Tme-Out for wait in milliseconds</param>
		/// <returns>True/False of the wait operation</returns>		
		public bool Wait(int msWaitTime)
		{
			return _ev.WaitOne(msWaitTime,false);
		}

		#endregion

		#region Private Data Members

		protected AutoResetEvent _ev = new AutoResetEvent(false);

		#endregion
	}
}
