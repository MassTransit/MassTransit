// Copyright 2007-2011 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using System.Threading;
    using System.Transactions;
    using Exceptions;
    using Internal;
    using log4net;
    using Magnum.Extensions;

    [DebuggerDisplay("OUT:TX:{Address}")]
    public class TransactionalOutboundMsmqTransport :
        AbstractOutboundMsmqTransport
    {
        public TransactionalOutboundMsmqTransport(IEndpointAddress address) : base(address)
        {
        }

        protected override void SendMessage(MessageQueue queue, Message message)
        {
            MessageQueueTransactionType tt = (Transaction.Current != null)
                                                 ? MessageQueueTransactionType.Automatic
                                                 : MessageQueueTransactionType.Single;

            queue.Send(message, tt);
        }
    }

    [DebuggerDisplay("OUT:{Address}")]
    public class NonTransactionalOutboundMsmqTransport :
        AbstractOutboundMsmqTransport
    {
        public NonTransactionalOutboundMsmqTransport(IEndpointAddress address) :
            base(address)
        {
        }
    }

    public abstract class AbstractOutboundMsmqTransport :
        IOutboundTransport
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (AbstractOutboundMsmqTransport));
        bool _disposed;

        protected AbstractOutboundMsmqTransport(IEndpointAddress address)
        {
            Address = address;
        }

        protected string FormatName { get; set; }
        public IEndpointAddress Address { get; set; }

        public void Send(Action<ISendingContext> sender)
        {
            if (_disposed) throw NewDisposedException();

            using (var message = new Message())
            {
                var cxt = new MsmqSendingContext(message);
                sender(cxt);

                try
                {
                    using (var queue = new MessageQueue(FormatName, QueueAccessMode.Send))
                    {
                        SendMessage(queue, message);
                    }
                }
                catch (MessageQueueException ex)
                {
                    HandleOutboundMessageQueueException(ex, 2.Seconds());
                }
            }
        }

        protected virtual void SendMessage(MessageQueue queue, Message message)
        {
            queue.Send(message, MessageQueueTransactionType.None);
        }

        protected ObjectDisposedException NewDisposedException()
        {
            return new ObjectDisposedException("The transport has already been disposed: '{0}'".FormatWith(Address));
        }

        protected void HandleOutboundMessageQueueException(MessageQueueException ex, TimeSpan timeout)
        {
            switch (ex.MessageQueueErrorCode)
            {
                case MessageQueueErrorCode.IOTimeout:
                    break;

                case MessageQueueErrorCode.ServiceNotAvailable:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queuing service is not available, pausing for timeout period", ex);

                    Thread.Sleep(timeout);
                    break;

                case MessageQueueErrorCode.QueueNotAvailable:
                case MessageQueueErrorCode.AccessDenied:
                case MessageQueueErrorCode.QueueDeleted:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queue was not available: " + FormatName, ex);

                    Thread.Sleep(timeout);
                    break;

                case MessageQueueErrorCode.QueueNotFound:
                case MessageQueueErrorCode.IllegalFormatName:
                case MessageQueueErrorCode.MachineNotFound:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queue was not found or is improperly named: " + FormatName, ex);

                    Thread.Sleep(timeout);
                    break;

                case MessageQueueErrorCode.MessageAlreadyReceived:
                    // we are competing with another consumer, no reason to report an error since
                    // the message has already been handled.
                    if (_log.IsDebugEnabled)
                        _log.Debug(
                            "The message was removed from the queue before it could be received. This could be the result of another service reading from the same queue.");
                    break;

                case MessageQueueErrorCode.InvalidHandle:
                case MessageQueueErrorCode.StaleHandle:
                    if (_log.IsErrorEnabled)
                        _log.Error(
                            "The message queue handle is stale or no longer valid due to a restart of the message queuing service: " +
                            FormatName, ex);


                    Thread.Sleep(timeout);
                    break;

                default:
                    if (_log.IsErrorEnabled)
                        _log.Error("There was a problem communicating with the message queue: " + FormatName, ex);
                    break;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Address = null;
            }

            _disposed = true;
        }

        ~AbstractOutboundMsmqTransport()
        {
            Dispose(false);
        }
    }


    [DebuggerDisplay("IN:TX:{Address}")]
    public class TransactionalInboundMsmqTransport :
        AbstractInboundMsmqTransport
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (TransactionalInboundMsmqTransport));

        public TransactionalInboundMsmqTransport(IEndpointAddress address) :
            base(address)
        {
        }

        public override void Receive(Func<Message, Action<Message>> receiver, TimeSpan timeout)
        {
            try
            {
                Connect();

                var options = new TransactionOptions
                                  {
                                      IsolationLevel = IsolationLevel.Serializable,
                                      Timeout = 30.Seconds(),
                                  };

                using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    if (EnumerateQueue(receiver, timeout))
                        scope.Complete();
                }
            }
            catch (MessageQueueException ex)
            {
                HandleInboundMessageQueueException(ex, timeout);
            }
        }

        protected override void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout,
                                               Action<Func<Message>> receiveAction)
        {
            receiveAction(() =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Removing message {0} from queue {1}", enumerator.Current.Id, Address);

                return enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.Automatic);
            });
        }
    }

    [DebuggerDisplay("IN:{Address}")]
    public class NonTransactionalInboundMsmqTransport :
        AbstractInboundMsmqTransport
    {
        public NonTransactionalInboundMsmqTransport(IEndpointAddress address) :
            base(address)
        {
        }
    }


    public abstract class AbstractInboundMsmqTransport :
        IInboundTransport
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (AbstractInboundMsmqTransport));
        bool _disposed;
        MessageQueue _queue;

        protected AbstractInboundMsmqTransport(IEndpointAddress address)
        {
            Address = address;
        }

        protected string FormatName { get; set; }
        public IEndpointAddress Address { get; set; }

        public void Receive(Func<IReceivingContext, Action<IReceivingContext>> receiver)
        {
            if (_disposed) throw NewDisposedException();

            Receive(receiver, TimeSpan.Zero);
        }

        public void Receive(Func<IReceivingContext, Action<IReceivingContext>> receiver, TimeSpan timeout)
        {
            try
            {
                Connect();

                EnumerateQueue(receiver, timeout);
            }
            catch (MessageQueueException ex)
            {
                HandleInboundMessageQueueException(ex, timeout);
            }
        }


        public virtual void Receive(Func<Message, Action<Message>> receiver, TimeSpan timeout)
        {
            try
            {
                Connect();

                EnumerateQueue(receiver, timeout);
            }
            catch (MessageQueueException ex)
            {
                HandleInboundMessageQueueException(ex, timeout);
            }
        }

        protected virtual bool EnumerateQueue(Func<Message, Action<Message>> receiver, TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            bool received = false;

            using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Enumerating endpoint: {0} ({1}ms)", Address, timeout);

                while (enumerator.MoveNext(timeout))
                {
                    Message current = enumerator.Current;
                    if (current == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Current message was null while enumerating endpoint");

                        continue;
                    }

                    Action<Message> receive = receiver(current);
                    if (receive == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SKIP:{0}:{1}", Address, current.Id);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, current.Id);

                        continue;
                    }

                    ReceiveMessage(enumerator, timeout, receiveCurrent =>
                    {
                        using (Message message = receiveCurrent())
                        {
                            if (message == null)
                                throw new TransportException(Address.Uri,
                                                             "Unable to remove message from queue: " + current.Id);

                            if (message.Id != current.Id)
                                throw new TransportException(Address.Uri,
                                                             string.Format(
                                                                 "Received message does not match current message: ({0} != {1})",
                                                                 message.Id, current.Id));

                            receive(message);

                            received = true;
                        }
                    });
                }
            }

            return received;
        }


        protected virtual bool EnumerateQueue(Func<IReceivingContext, Action<IReceivingContext>> receiver,
                                              TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            bool received = false;

            using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Enumerating endpoint: {0} ({1}ms)", Address, timeout);

                while (enumerator.MoveNext(timeout))
                {
                    Message current = enumerator.Current;
                    if (current == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Current message was null while enumerating endpoint");

                        continue;
                    }

                    var cxt = new MsmqReceivingContext(current);
                    Action<IReceivingContext> receive = receiver(cxt);
                    if (receive == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SKIP:{0}:{1}", Address, current.Id);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, current.Id);

                        continue;
                    }

                    ReceiveMessage(enumerator, timeout, receiveCurrent =>
                    {
                        using (Message message = receiveCurrent())
                        {
                            if (message == null)
                                throw new TransportException(Address.Uri,
                                                             "Unable to remove message from queue: " + current.Id);

                            if (message.Id != current.Id)
                                throw new TransportException(Address.Uri,
                                                             string.Format(
                                                                 "Received message does not match current message: ({0} != {1})",
                                                                 message.Id, current.Id));
                            var cxt2 = new MsmqReceivingContext(message);
                            receive(cxt2);

                            received = true;
                        }
                    });
                }
            }

            return received;
        }

        protected virtual void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout,
                                              Action<Func<Message>> receiveAction)
        {
            receiveAction(() => enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.None));
        }


        protected void Connect()
        {
            if (_queue != null)
                return;
            _queue = new MessageQueue(FormatName, QueueAccessMode.Receive);
        }

        protected void Disconnect()
        {
            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
            }
        }

        protected void Reconnect()
        {
            Disconnect();
            Connect();
        }

        protected ObjectDisposedException NewDisposedException()
        {
            return new ObjectDisposedException("The transport has already been disposed: '{0}'".FormatWith(Address));
        }

        protected void HandleInboundMessageQueueException(MessageQueueException ex, TimeSpan timeout)
        {
            switch (ex.MessageQueueErrorCode)
            {
                case MessageQueueErrorCode.IOTimeout:
                    break;

                case MessageQueueErrorCode.ServiceNotAvailable:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queuing service is not available, pausing for timeout period", ex);

                    Thread.Sleep(timeout);
                    Reconnect();
                    break;

                case MessageQueueErrorCode.QueueNotAvailable:
                case MessageQueueErrorCode.AccessDenied:
                case MessageQueueErrorCode.QueueDeleted:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queue was not available: " + FormatName, ex);

                    Thread.Sleep(timeout);
                    Reconnect();
                    break;

                case MessageQueueErrorCode.QueueNotFound:
                case MessageQueueErrorCode.IllegalFormatName:
                case MessageQueueErrorCode.MachineNotFound:
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queue was not found or is improperly named: " + FormatName, ex);

                    Thread.Sleep(timeout);
                    Reconnect();
                    break;

                case MessageQueueErrorCode.MessageAlreadyReceived:
                    // we are competing with another consumer, no reason to report an error since
                    // the message has already been handled.
                    if (_log.IsDebugEnabled)
                        _log.Debug(
                            "The message was removed from the queue before it could be received. This could be the result of another service reading from the same queue.");
                    break;

                case MessageQueueErrorCode.InvalidHandle:
                case MessageQueueErrorCode.StaleHandle:
                    if (_log.IsErrorEnabled)
                        _log.Error(
                            "The message queue handle is stale or no longer valid due to a restart of the message queuing service: " +
                            FormatName, ex);

                    Reconnect();

                    Thread.Sleep(timeout);
                    break;

                default:
                    if (_log.IsErrorEnabled)
                        _log.Error("There was a problem communicating with the message queue: " + FormatName, ex);
                    break;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Disconnect();

                Address = null;
            }

            _disposed = true;
        }

        ~AbstractInboundMsmqTransport()
        {
            Dispose(false);
        }
    }
}