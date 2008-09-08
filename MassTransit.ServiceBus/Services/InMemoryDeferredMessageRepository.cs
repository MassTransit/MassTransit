namespace MassTransit.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Messages;

	public class InMemoryDeferredMessageRepository :
		IDeferredMessageRepository
	{
		private Dictionary<Guid, DeferMessage> _messages = new Dictionary<Guid, DeferMessage>();

		public void Add(Guid id, DeferMessage message)
		{
			lock(_messages)
				_messages.Add(id, message);
		}

		public DeferMessage Get(Guid id)
		{
			lock(_messages)
				if (_messages.ContainsKey(id))
					return _messages[id];

			return null;
		}

		public bool Contains(Guid id)
		{
			lock (_messages)
				return _messages.ContainsKey(id);
		}

		public void Remove(Guid id)
		{
			lock (_messages)
				_messages.Remove(id);
		}

		public void Dispose()
		{
			_messages.Clear();
		}
	}
}