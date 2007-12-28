using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
	public class MsmqTransport :
		ITransport, IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly BinaryFormatter _formatter;
		private readonly string _queueName;

		private readonly CustomBackgroundWorker _worker;
		private readonly MessageQueue _queue;

		private object _eventLock = new object();

		private Queue<Envelope> _envelopes = new Queue<Envelope>();

		private IAsyncResult _peekAsyncResult;

		public MsmqTransport(string queueName)
		{
			_queue = new MessageQueue(queueName, QueueAccessMode.ReceiveAndAdmin);

			_queueName = NormalizeQueueName(_queue);

			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			_queue.MessageReadPropertyFilter = mpf;

			_formatter = new BinaryFormatter();

			_worker = new CustomBackgroundWorker(Receive, true);
		}

		private static string NormalizeQueueName(MessageQueue queue)
		{
			string machineName = queue.MachineName;
			if (machineName == "." || string.Compare(machineName, "localhost", true) == 0)
			{
				queue.MachineName = Environment.MachineName;
			}

			return queue.Path;
		}

		private void Receive(object sender, DoWorkEventArgs e)
		{
			if (sender is CustomBackgroundWorker)
			{
				CustomBackgroundWorker worker = sender as CustomBackgroundWorker;

				// startup code (if needed)

				_log.DebugFormat("Queue: {0} Peek Waiting for {1}", _queue.Path, GetHashCode());

				_queue.ReceiveCompleted += Queue_ReceiveCompleted;

				_peekAsyncResult = _queue.BeginReceive();

				int waitResult;
				while ((waitResult = WaitHandle.WaitAny(worker.WaitHandles.ToArray(), Timeout.Infinite, false)) != (int) CustomBackgroundWorker.WaitHandleIndex.Exit)
				{
					if (waitResult == WaitHandle.WaitTimeout)
					{
						// if we timed out, do we have an interval item to evaluate
					}
					else if (waitResult == (int) CustomBackgroundWorker.WaitHandleIndex.Trigger)
					{
						try
						{
							lock (_eventLock)
							{
								Envelope envelope;
								lock(_envelopes)
									envelope = _envelopes.Dequeue();

								NotifyHandlers(new EnvelopeReceivedEventArgs(envelope));

								//if (EnvelopeReceived != null)
								//    EnvelopeReceived(this, new EnvelopeReceivedEventArgs(e));
								//else
								//{
								//    _log.DebugFormat("Envelope dropped: {0}", e.Id);
								//}
							}
						}
						catch (Exception ex)
						{
							_log.Error("Envelope Exception", ex);
						}
					}
				}
			}

			e.Result = true;
		}

		private void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArgs)
		{
			try
			{
				Message msg = eventArgs.Message;

				_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

				Envelope e = new Envelope();

				e.Id = msg.Id;
				e.CorrelationId = (msg.CorrelationId == "00000000-0000-0000-0000-000000000000\\0" ? null : msg.CorrelationId);

				e.TimeToBeReceived = msg.TimeToBeReceived;
				e.Recoverable = msg.Recoverable;
				e.SentTime = msg.SentTime;
				e.ArrivedTime = msg.ArrivedTime;
				e.Label = msg.Label;

				if (msg.ResponseQueue != null)
					e.ReturnTo = (MessageQueueEndpoint) msg.ResponseQueue.Path;

				IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
				if (messages != null)
				{
					e.Messages = messages;
				}

				try
				{
					lock (_eventLock)
					{
						lock (_envelopes)
							_envelopes.Enqueue(e);
						_worker.Trigger();

						//NotifyHandlers(new EnvelopeReceivedEventArgs(e));

						//if (EnvelopeReceived != null)
						//    EnvelopeReceived(this, new EnvelopeReceivedEventArgs(e));
						//else
						//{
						//    _log.DebugFormat("Envelope dropped: {0}", e.Id);
						//}
					}
				}
				catch (Exception ex)
				{
					_log.Error("Exception from Envelope Received: ", ex);
				}
			}
			catch (MessageQueueException ex)
			{
				_log.Error("Exception in Queue_ReceiveCompleted: ", ex);

				if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return;
			}

			_peekAsyncResult = _queue.BeginReceive();
		}

		public string Address
		{
			get { return _queueName; }
		}

		public void Send(IEnvelope envelope)
		{
			using (MessageQueue q = new MessageQueue(_queueName, QueueAccessMode.Send))
			{
				Message msg = new Message();

				if (envelope.Messages != null && envelope.Messages.Length > 0)
				{
					SerializeMessages(msg.BodyStream, envelope.Messages);
				}

				msg.ResponseQueue = new MessageQueue(envelope.ReturnTo.Transport.Address);

				q.Send(msg);

				envelope.Id = msg.Id;

				_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id, envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
			}
		}

		private EventHandler<EnvelopeReceivedEventArgs> _onEnvelopeReceived;

		public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived
		{
			add
			{
				lock(_eventLock)
					_onEnvelopeReceived = (EventHandler<EnvelopeReceivedEventArgs>) Delegate.Combine(_onEnvelopeReceived, value);
			}
			remove
			{
				lock(_eventLock)
					_onEnvelopeReceived = (EventHandler<EnvelopeReceivedEventArgs>)Delegate.Remove(_onEnvelopeReceived, value);
			}
		}

		private void NotifyHandlers(EnvelopeReceivedEventArgs e)
		{
			if (_onEnvelopeReceived != null)
			{
				_log.DebugFormat("Delivering Envelope {0} by {1}", e.Envelope.Id, GetHashCode());
				_onEnvelopeReceived(this, e);
			}
			else
				_log.DebugFormat("Envelope {0} dropped by {1}", e.Envelope.Id, GetHashCode());
		}

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