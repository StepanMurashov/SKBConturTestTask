using System;
using System.Threading;

namespace Sonic.Net.ThreadPoolTaskFramework
{
	#region Context Classes

	/// <summary>
	/// Interface that defines a type that is used to generate unique ids
	/// </summary>
	public interface IContextIdGenerator
	{
		/// <summary>
		/// Retun the next unique context id
		/// </summary>
		/// <returns>Unique context id</returns>
		object GetNextContextId();
		/// <summary>
		/// Reset the context id so that the next context id will
		/// start from the begining of its internal sequence.
		/// </summary>
		void Reset();
	}

	/// <summary>
	/// Interface that defines a type that is used to create a unique context.
	/// Context provide an environment for serialzed task execution. 
	/// </summary>
	public interface IContext
	{
		/// <summary>
		/// Unique id of the context
		/// </summary>
		object Id
		{
			get;
		}
		/// <summary>
		/// Lock the context
		/// </summary>
		void Lock();
		/// <summary>
		/// Unlock the context
		/// </summary>
		void UnLock();
	}

	/// <summary>
	/// Abstract Factory interface to create context types
	/// </summary>
	public interface IContextFactory
	{
		IContext CreateContext();
	}

	/// <summary>
	/// Monitor based implementation of IContext interface
	/// </summary>
	public class Context : IContext	
	{
		#region Public Constructor(s)

		public Context(object id)
		{
			_id = id;
		}

		#endregion

		#region IContext Public Methods

		public object Id
		{
			get
			{
				return _id;
			}
		}
		public void Lock()
		{
			Monitor.Enter(this);
		}
		public void UnLock()
		{
			Monitor.Exit(this);
		}

		#endregion

		#region Private Data Members
		
		private object _id;

		#endregion
	}

	/// <summary>
	/// Factory class for creating Monitor based Context objects
	/// </summary>
	public class ContextFactory : IContextFactory
	{
		#region Public Constructor(s)

		public ContextFactory(IContextIdGenerator ctxIdGen)
		{
			_ctxIdGen = ctxIdGen;
		}

		#endregion

		#region IContextFactory Members

		public IContext CreateContext()
		{
			return new Context(_ctxIdGen.GetNextContextId());
		}

		#endregion

		#region Private Data Members

		IContextIdGenerator _ctxIdGen;

		#endregion
	}

	/// <summary>
	/// Singleton instance implementation of Context Id generator
	/// </summary>
	public class ContextIdGenerator : IContextIdGenerator
	{
		#region IContextIdGenerator Members

		public object GetNextContextId()
		{
			return Interlocked.Increment(ref _ctxId);
		}

		public void Reset()
		{
			_ctxId = 0;
		}

		#endregion

		#region Public Static Methods

		public static IContextIdGenerator GetInstance()
		{
			return _ctxIdGen;
		}

		#endregion

		#region Public Static Data Members

		public static readonly ContextIdGenerator _ctxIdGen = new ContextIdGenerator();
		public static long _ctxId = 0;

		#endregion
	}



	#endregion
}
