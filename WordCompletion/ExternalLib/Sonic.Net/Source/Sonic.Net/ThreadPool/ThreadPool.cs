using System;
using System.Collections;
using System.Threading;

namespace Sonic.Net
{
	/// <summary>
	/// Interface used by ThreadPool class for executing
	/// Tasks dispatched to it
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Executes the corresponding task 
		/// </summary>
		/// <param name="tp">ThreadPool onto which this Task is dispatched</param>
		void Execute(ThreadPool tp);
		/// <summary>
		/// Specifies to the ThreadPool whether to Execute this task based on whether
		/// this Task is Active or not. This allows for cancellation of Tasks after
		/// they are dispatched to ThreadPool for execution
		/// </summary>
		bool Active {get;set;}
		/// <summary>
		/// Indicates the task that its execution has been completed.
		/// </summary>
		void Done();
	}

	/// <summary>
	/// Class for dispatching objects to a Managed IOCP based ThreadPool
	/// for processing.
	/// </summary>
	public class ThreadPool
	{
		public delegate void ThreadPoolThreadExceptionHandler(Exception ex);
		/// <summary>
		/// Creates an instance of the ThreadPool with specified number
		/// of Maximum Threads allowed in the pool (maxThreads) and the
		/// Maximum concurrent threads allowed to be active in parallel
		/// </summary>
		/// <param name="maxThreads">Maximum Threads allowed in this ThreadPool</param>
		/// <param name="concurrentThreads">Maximum concurrent threads allowed to be active in parallel
		/// in this ThreadPool</param>
		public ThreadPool(short maxThreads, short concurrentThreads)
		{
			Init(maxThreads,concurrentThreads,null);
		}
		public ThreadPool(short maxThreads, short concurrentThreads, ThreadPoolThreadExceptionHandler tpThEx)
		{
			Init(maxThreads,concurrentThreads,tpThEx);
		}
		/// <summary>
		/// Get/Set the Maximum concurrent threads allowed to be active in parallel
		/// in this ThreadPool
		/// </summary>
		public short ConcurrentThreads
		{
			get
			{
				return _concurrentThreads;
			}
			set
			{
				_concurrentThreads = value;
			}
		}
		/// <summary>
		/// Get/Set the Maximum Threads allowed in this ThreadPool
		/// </summary>
		public short MaxThreads
		{
			get
			{
				return _maxThreads;
			}
			set
			{
				_maxThreads = value;
			}
		}
        public int ActiveThreads
        {
            get
            {
                return _mIOCP.ActiveThreads;
            }
        }
		/// <summary>
		/// Dispatch a task object to be processed by threads in this thread pool.
		/// </summary>
		/// <param name="task"></param>
		public void Dispatch(ITask task)
		{
			_mIOCP.Dispatch(task);
		}
		/// <summary>
		/// Close the processing of objects by this Thread Pool. Effectively
		/// makes this Thread Pool instance invalid.
		/// </summary>
		public void Close()
		{
			_mIOCP.Close();
		}

		private void ThreadFunc()
		{
			IOCPHandle hIOCP = _mIOCP.Register();
			_ev.Set();
			try
			{
				while(true)
				{
					ITask task = hIOCP.Wait() as ITask;
					if (task.Active == true) 
					{
						task.Execute(this);
						task.Done();
					}
				}
			}
			catch(Exception ex)
			{
				// Raise an event to the registered even handlers 
				// for ThreadExceptions
				//
				if (_tpThreadExceptionHandler != null) _tpThreadExceptionHandler(ex);
			}
			_mIOCP.UnRegister();
		}
		private void Init(short maxThreads, short concurrentThreads, ThreadPoolThreadExceptionHandler tpThEx)
		{
			_maxThreads = maxThreads;
			_concurrentThreads = concurrentThreads;
			_tpThreadExceptionHandler = tpThEx;
			_mIOCP = new ManagedIOCP(_concurrentThreads);
			for(short index = 1; index <= _maxThreads; index++)
			{
				Thread th = new Thread(new ThreadStart(this.ThreadFunc));
				th.Start();
				_ev.WaitOne();
			}
		}

		private short _maxThreads;
		private short _concurrentThreads;
		private ThreadPool.ThreadPoolThreadExceptionHandler _tpThreadExceptionHandler;
		private ManagedIOCP _mIOCP;
		private AutoResetEvent _ev = new AutoResetEvent(false);
	}
}
