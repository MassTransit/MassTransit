using System;

namespace MassTransit.ServiceBus
{
	public class EnvelopeReceivedEventArgs :
		EventArgs
	{
        private IEnvelope _envelope;

		public EnvelopeReceivedEventArgs(IEnvelope e)
		{
			_envelope = e;
		}

        public IEnvelope Envelope
        {
            get { return _envelope; }
        }
	}
}