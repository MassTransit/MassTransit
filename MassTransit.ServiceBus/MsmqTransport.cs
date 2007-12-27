using System;
using System.ComponentModel;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class MsmqTransport :
        ITransport, IDisposable
    {
		private static readonly ILog _log = LogManager.GetLogger("default");

		private readonly BinaryFormatter _formatter;
		private readonly string _queueName;

    	private readonly CustomBackgroundWorker _worker;
    	private readonly MessageQueue _queue;

    	public MsmqTransport(string queueName)
        {
            _queueName = queueName;
        	_queue = new MessageQueue(_queueName, QueueAccessMode.ReceiveAndAdmin);

			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			_queue.MessageReadPropertyFilter = mpf;

        	_formatter = new BinaryFormatter();

        	_worker = new CustomBackgroundWorker(Receive, true);
        }

    	private void Receive(object sender, DoWorkEventArgs e)
    	{
			if (sender is CustomBackgroundWorker)
			{
				CustomBackgroundWorker worker = sender as CustomBackgroundWorker;

				// startup code (if needed)

				_log.DebugFormat("Peeking at queue {0}", _queue.Path);

				IAsyncResult peekAsyncResult = _queue.BeginPeek(TimeSpan.FromSeconds(30));

				worker.WaitHandles.Add(peekAsyncResult.AsyncWaitHandle);

				int waitResult;
				while ((waitResult = WaitHandle.WaitAny(worker.WaitHandles.ToArray(), TimeSpan.FromMinutes(1), false)) != (int)CustomBackgroundWorker.WaitHandleIndex.Exit)
				{
					if (waitResult == WaitHandle.WaitTimeout)
					{
						// if we timed out, do we have an interval item to evaluate
					}
					else if (waitResult == (int)CustomBackgroundWorker.WaitHandleIndex.Trigger)
					{
						// we have a task pending that needs attention
					}
					else if (waitResult == (int)CustomBackgroundWorker.WaitHandleIndex.Trigger + 1)
					{
						// our peek trigger, we have some work to do now

						//_queue.EndPeek(peekAsyncResult);

						ReceiveFromQueue();

						peekAsyncResult = _queue.BeginPeek(TimeSpan.FromSeconds(30));

						worker.WaitHandles[(int) CustomBackgroundWorker.WaitHandleIndex.Trigger + 1] = peekAsyncResult.AsyncWaitHandle;
					}
				}
			}

			e.Result = true;
    	}

		private void ReceiveFromQueue()
		{
			try
			{
				_log.Debug("Receiving Message From Queue");

				Message msg = _queue.Receive(TimeSpan.Zero);

				Envelope e = new Envelope();

				e.Id = msg.Id;
				e.CorrelationId = (msg.CorrelationId == "00000000-0000-0000-0000-000000000000\\0" ? null : msg.CorrelationId);

				e.TimeToBeReceived = msg.TimeToBeReceived;
				e.Recoverable = msg.Recoverable;
				e.SentTime = msg.SentTime;
				e.ArrivedTime = msg.ArrivedTime;
				e.Label = msg.Label;

				if(msg.ResponseQueue != null)
					e.ReturnTo = (MessageQueueEndpoint)msg.ResponseQueue.Path;

				IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
				if(messages != null)
				{
					e.Messages = messages;
				}

				try
				{
					if (EnvelopeReceived != null)
						EnvelopeReceived(this, new EnvelopeReceivedEventArgs(e));
					
				}
				catch
				{
					// ignore any excpetions from the event
					
				}
			}
			catch (MessageQueueException ex)
			{
				if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return;

				throw;
			}
		}

    	public string Address
        {
            get { return _queueName; }
        }

        public void Send(IEnvelope envelope)
        {
            using ( MessageQueue q = new MessageQueue(_queueName, QueueAccessMode.Send))
            {
                Message msg = new Message();

                if(envelope.Messages != null && envelope.Messages.Length>0)
                {
                    SerializeMessages(msg.BodyStream, envelope.Messages);
                }

                msg.ResponseQueue = new MessageQueue(envelope.ReturnTo.Transport.Address);

                q.Send(msg);

                envelope.Id = msg.Id;
            }
        }

    	public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

    	private void SerializeMessages(Stream stream, IMessage[] messages)
        {
        	_formatter.Serialize(stream, messages);
        }

    	public void Dispose()
    	{
    		_worker.Stop();
    		_queue.Close();
    	}
    }
}