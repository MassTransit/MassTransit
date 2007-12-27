using System;

namespace MassTransit.ServiceBus
{
	public class EnvelopeReceivedEventArgs :
		EventArgs
	{
		public EnvelopeReceivedEventArgs(IEnvelope e)
		{
			_envelope = e;
		}

		private IEnvelope _envelope;

        public IEnvelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }
	}
}