using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	/// <summary>
	/// Class that provides a waitable Generic Task object to be used with ThreadPool
	/// </summary>
	public abstract class WaitableGenericTask : GenericTask
	{
		#region Public Constructor(s)
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public WaitableGenericTask()
		{
			// No-Op
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Wait till the current Geenric Task instance is executed by the
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
