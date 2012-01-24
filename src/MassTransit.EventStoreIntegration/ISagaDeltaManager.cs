namespace MassTransit.EventStoreIntegration
{
	using System.Collections.Generic;
	using Util;

	/// <summary>
	/// This instance has the moving state:
	/// 
	/// <list type="bullet">
	///		<value>Uncommitted events</value>
	///		<value><see cref="ClearUncommittedEvents"/> clears the instance list of uncommitted events.</value>
	/// </list>
	/// </summary>
	public interface ISagaDeltaManager
	{
		/// <summary>
		/// Gets the saga version.
		/// </summary>
		ulong Version { get; }

		/// <summary>
		/// Apply a state change to the saga
		/// </summary>
		/// <param name="delta"> </param>
		void ApplyStateDelta([NotNull] object delta);

		/// <summary>
		/// Gets the collection of events that haven't yet been committed.
		/// </summary>
		/// <returns>A collection of events</returns>
		[NotNull]
		IEnumerable<object> GetUncommittedEvents();

		/// <summary>
		/// Clears the collection of uncommitted events from this saga instance.
		/// </summary>
		void ClearUncommittedEvents();
	}
}