using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

using Sonic.Net;
using Sonic.Net.Utilities;
using Sonic.Net.DataStructures.LockFree;
using Sonic.Net.ThreadPoolTaskFramework;

namespace Sonic.Net.ConsoleDemo
{
    #region .Net Thread Pool Callback
    /// <summary>
    /// .Net Thread Pool callback provider
    /// </summary>
    class DotNetThreadPoolHandler
    {
        public static void ThreadPoolCallback(object obj)
        {
            if (Interlocked.Increment(ref ManagedIOCPConsoleDemo._i) >= 100)
            {
                Interlocked.Exchange(ref ManagedIOCPConsoleDemo._i, (int)0);
                ManagedIOCPConsoleDemo._sw.Stop();

                ManagedIOCPConsoleDemo._j++;
                Console.WriteLine("Iteration {0} -- Total Time : {1}", ManagedIOCPConsoleDemo._j, ManagedIOCPConsoleDemo._sw.Milliseconds);
                
				ManagedIOCPConsoleDemo._sw.Start();

                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCallback));
                ManagedIOCPConsoleDemo.ReadData();
            }
            else
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCallback));
                ManagedIOCPConsoleDemo.ReadData();
            }
        }
    }
    #endregion

    #region ManagedIOCP Thread Pool Task
    /// <summary>
    /// ManagedIOCP Thread Pool Task
    /// </summary>
	class MyGenericTask : WaitableContextBoundGenericTask
	{
		public override void Execute(ThreadPool tp)
		{
            //Context.Lock();

            if (Interlocked.Increment(ref ManagedIOCPConsoleDemo._i) >= 100)
			{
                Interlocked.Exchange(ref ManagedIOCPConsoleDemo._i, (int)0);
                ManagedIOCPConsoleDemo._sw.Stop();
                
                Console.WriteLine("Active Threads : {0}", tp.ActiveThreads);
                ManagedIOCPConsoleDemo._j++;
                Console.WriteLine("Iteration {0} -- Total Time : {1}", ManagedIOCPConsoleDemo._j, ManagedIOCPConsoleDemo._sw.Milliseconds);
                
				ManagedIOCPConsoleDemo._sw.Start();

                MyGenericTask gt = this.TaskFactory.NewContextBoundGenericTask(null, null,this.Context) as MyGenericTask;
                tp.Dispatch(gt);
                ManagedIOCPConsoleDemo.ReadData();
			}
			else
			{
                MyGenericTask gt = this.TaskFactory.NewContextBoundGenericTask(null, null, this.Context) as MyGenericTask;
                tp.Dispatch(gt);
                ManagedIOCPConsoleDemo.ReadData();
			}

            //Context.UnLock();
		}
	}
    /// <summary>
    /// ManagedIOCP Thread Pool Task Factory
    /// </summary>
	class MyGenericTaskFactory : ContextBoundGenericTaskFactory
	{
		public override PoolableObject CreatePoolableObject()
		{
			return new MyGenericTask();
		}

    }
    #endregion

    #region ManagedIOCP/.Net Thread Pool Console Demo
    /// <summary>
	/// Summary description for Class1.
	/// </summary>
	class ManagedIOCPConsoleDemo
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            //StartDotNetThreadPool();
            StartManagedIOCPThreadPool();
		}
        
        /// <summary>
        /// Start ManagedIOCP Thread Pool Demo
        /// </summary>
        static void StartManagedIOCPThreadPool()
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadLine();
            Console.WriteLine("Started ManagedIOCP Thread Pool with [4] concurrency limit...");
            ManagedIOCPConsoleDemo._sw.Start();
            _tp = new ThreadPool(25, 10);
            object ctxId = ContextIdGenerator.GetInstance().GetNextContextId();
            Context ctx = new Context(ctxId);
            for (int i = 1; i <= 1000; i++)
                _tp.Dispatch(_tf.NewContextBoundGenericTask(null, null,ctx));
            Thread th = new Thread(new ThreadStart(ManagedIOCPBurstThread));
            th.Start();
            Console.ReadLine();
            _tp.Close();
        }
        static void ManagedIOCPBurstThread()
        {
            try
            {
                object ctxId = ContextIdGenerator.GetInstance().GetNextContextId();
                Context ctx = new Context(ctxId);
                while (true)
                {
                    for(int i = 1; i <= 100; i++)
                        _tp.Dispatch(_tf.NewContextBoundGenericTask(null, null,ctx));
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Start .Net Thread Pool Demo
        /// </summary>
        static void StartDotNetThreadPool()
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadLine();
            Console.WriteLine("Started .Net Thread Pool...");
            ManagedIOCPConsoleDemo._sw.Start();
			// Not supported in .Net v1.1
            // System.Threading.ThreadPool.SetMaxThreads(4, 25);
            for(int i = 1; i <= 1000; i++)
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DotNetThreadPoolHandler.ThreadPoolCallback));
            Thread th = new Thread(new ThreadStart
                (DotNetThreadPoolBurstThread));
            th.Start();
            Console.ReadLine();
        }
        static void DotNetThreadPoolBurstThread()
        {
            try
            {
                while (true)
                {
                    for (int i = 1; i <= 25; i++)
                        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DotNetThreadPoolHandler.ThreadPoolCallback));
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
            }
        }


        public static void ReadData()
        {
			StreamReader sr = File.OpenText(@"C:\aditya\downloads\lgslides.pdf");
			string st = sr.ReadToEnd();
			st = null;
			sr.Close();
			Thread.Sleep(100);
        }

        static MyGenericTaskFactory _tf = new MyGenericTaskFactory();
        public static ThreadPool _tp;
		// Stop watch is not supposed to be used across threads.
		// I'm using it here as we calculate the completed time of an iteration
		// most probably in one thread. So I did not bother about using a global static 
		// StopWatch object.
		//
		public static StopWatch _sw = new StopWatch();
		public static int _i = 0;
        public static long _j = 0;
    }
    #endregion
}
