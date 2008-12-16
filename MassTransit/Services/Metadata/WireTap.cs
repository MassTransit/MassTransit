namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using Pipeline;
    using Pipeline.Sinks;

    public class WireTap<TMessage> :
        MessageSinkBase<TMessage, TMessage> where TMessage : class
	{
		private readonly Func<TMessage, bool> _allow;
        private readonly Action<TMessage> _action;

		public WireTap(string description, Func<IMessageSink<TMessage>, IMessageSink<TMessage>> insertAfter, Func<TMessage, bool> allow, Action<TMessage> action) :
			base(null)
		{
			Description = description ?? string.Empty;

			_allow = allow;
		    _action = action;

			ReplaceOutputSink(insertAfter(this));
		}

		public string Description { get; private set; }

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			if (!_allow(message))
				yield break;

			foreach (Consumes<TMessage>.All consumer in _outputSink.ReadLock(x => x.Enumerate(message)))
			{
				yield return consumer;
			}
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _outputSink.ReadLock(x => x.Inspect(inspector));
		}
	}
}