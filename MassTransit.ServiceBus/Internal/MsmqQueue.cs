namespace MassTransit.ServiceBus.Internal
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Messaging;

    /// <summary>
    /// Class to delegate to Msmq For Testing Purposes
    /// </summary>
    public class MsmqQueue : IMsmqQueue
    {
        private MessageQueue _queue;

        #region Constructors
        public MsmqQueue(string path)
        {
            _queue = new MessageQueue(path);
        }

        public MsmqQueue(string path, bool sharedModeDenyReceive)
        {
            _queue = new MessageQueue(path, sharedModeDenyReceive);
        }
        #endregion

        public IAsyncResult BeginPeek()
        {
            return _queue.BeginPeek();
        }

        public IAsyncResult BeginPeek(TimeSpan timeout)
        {
            return _queue.BeginPeek(timeout);
        }

        public IAsyncResult BeginPeek(TimeSpan timeout, object stateObject)
        {
            return _queue.BeginPeek(timeout, stateObject);
        }

        public IAsyncResult BeginPeek(TimeSpan timeout, object stateObject, AsyncCallback callback)
        {
            return _queue.BeginPeek(timeout, stateObject, callback);
        }

        public IAsyncResult BeginPeek(TimeSpan timeout, Cursor cursor, PeekAction action, object state, AsyncCallback callback)
        {
            return _queue.BeginPeek(timeout, cursor, action, state, callback);
        }

        public IAsyncResult BeginReceive()
        {
            return _queue.BeginReceive();
        }

        public IAsyncResult BeginReceive(TimeSpan timeout)
        {
            return _queue.BeginReceive(timeout);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, object stateObject)
        {
            return _queue.BeginReceive(timeout, stateObject);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, object stateObject, AsyncCallback callback)
        {
            return _queue.BeginReceive(timeout, stateObject, callback);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, Cursor cursor, object state, AsyncCallback callback)
        {
            return _queue.BeginReceive(timeout, cursor, state, callback);
        }

        public void Close()
        {
            _queue.Close();
        }

        public Cursor CreateCursor()
        {
            return _queue.CreateCursor();
        }

        public Message EndPeek(IAsyncResult asyncResult)
        {
            return _queue.EndPeek(asyncResult);
        }

        public Message EndReceive(IAsyncResult asyncResult)
        {
            return _queue.EndReceive(asyncResult);
        }

        public Message[] GetAllMessages()
        {
            return _queue.GetAllMessages();
        }

        public IEnumerator GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public MessageEnumerator GetMessageEnumerator()
        {
            return _queue.GetMessageEnumerator();
        }

        public MessageEnumerator GetMessageEnumerator2()
        {
            return _queue.GetMessageEnumerator2();
        }

        public Message Peek()
        {
            return _queue.Peek();
        }

        public Message Peek(TimeSpan timeout)
        {
            return _queue.Peek(timeout);
        }

        public Message Peek(TimeSpan timeout, Cursor cursor, PeekAction action)
        {
            return _queue.Peek(timeout, cursor, action);
        }

        public Message PeekById(string id)
        {
            return _queue.PeekById(id);
        }

        public Message PeekById(string id, TimeSpan timeout)
        {
            return _queue.PeekById(id, timeout);
        }

        public Message PeekByCorrelationId(string correlationId)
        {
            return _queue.PeekByCorrelationId(correlationId);
        }

        public Message PeekByCorrelationId(string correlationId, TimeSpan timeout)
        {
            return _queue.PeekByCorrelationId(correlationId, timeout);
        }

        public void Purge()
        {
            _queue.Purge();
        }

        public Message Receive()
        {
            return _queue.Receive();
        }

        public Message Receive(MessageQueueTransaction transaction)
        {
            return _queue.Receive(transaction);
        }

        public Message Receive(MessageQueueTransactionType transactionType)
        {
            return _queue.Receive(transactionType);
        }

        public Message Receive(TimeSpan timeout)
        {
            return _queue.Receive(timeout);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor)
        {
            return _queue.Receive(timeout, cursor);
        }

        public Message Receive(TimeSpan timeout, MessageQueueTransaction transaction)
        {
            return _queue.Receive(timeout, transaction);
        }

        public Message Receive(TimeSpan timeout, MessageQueueTransactionType transactionType)
        {
            return _queue.Receive(timeout, transactionType);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransaction transaction)
        {
            return _queue.Receive(timeout, cursor, transaction);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransactionType transactionType)
        {
            return _queue.Receive(timeout, cursor, transactionType);
        }

        public Message ReceiveById(string id)
        {
            return _queue.ReceiveById(id);
        }

        public Message ReceiveById(string id, MessageQueueTransaction transaction)
        {
            return _queue.ReceiveById(id, transaction);
        }

        public Message ReceiveById(string id, MessageQueueTransactionType transactionType)
        {
            return _queue.ReceiveById(id, transactionType);
        }

        public Message ReceiveById(string id, TimeSpan timeout)
        {
            return _queue.ReceiveById(id, timeout);
        }

        public Message ReceiveById(string id, TimeSpan timeout, MessageQueueTransaction transaction)
        {
            return _queue.ReceiveById(id, timeout, transaction);
        }

        public Message ReceiveById(string id, TimeSpan timeout, MessageQueueTransactionType transactionType)
        {
            return _queue.ReceiveById(id, timeout, transactionType);
        }

        public Message ReceiveByCorrelationId(string correlationId)
        {
            return _queue.ReceiveByCorrelationId(correlationId);
        }

        public Message ReceiveByCorrelationId(string correlationId, MessageQueueTransaction transaction)
        {
            return _queue.ReceiveByCorrelationId(correlationId, transaction);
        }

        public Message ReceiveByCorrelationId(string correlationId, MessageQueueTransactionType transactionType)
        {
            return _queue.ReceiveByCorrelationId(correlationId, transactionType);
        }

        public Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout)
        {
            return _queue.ReceiveByCorrelationId(correlationId, timeout);
        }

        public Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout, MessageQueueTransaction transaction)
        {
            return _queue.ReceiveByCorrelationId(correlationId, timeout, transaction);
        }

        public Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout, MessageQueueTransactionType transactionType)
        {
            return _queue.ReceiveByCorrelationId(correlationId, timeout, transactionType);
        }

        public Message ReceiveByLookupId(long lookupId)
        {
            return _queue.ReceiveByLookupId(lookupId);
        }

        public Message ReceiveByLookupId(MessageLookupAction action, long lookupId, MessageQueueTransactionType transactionType)
        {
            return _queue.ReceiveByLookupId(action, lookupId, transactionType);
        }

        public Message ReceiveByLookupId(MessageLookupAction action, long lookupId, MessageQueueTransaction transaction)
        {
            return _queue.ReceiveByLookupId(action, lookupId, transaction);
        }

        public Message PeekByLookupId(long lookupId)
        {
            return _queue.PeekByLookupId(lookupId);
        }

        public Message PeekByLookupId(MessageLookupAction action, long lookupId)
        {
            return _queue.PeekByLookupId(action, lookupId);
        }

        public void Refresh()
        {
            _queue.Refresh();
        }

        public void Send(object obj)
        {
            _queue.Send(obj);
        }

        public void Send(object obj, MessageQueueTransaction transaction)
        {
            _queue.Send(obj, transaction);
        }

        public void Send(object obj, MessageQueueTransactionType transactionType)
        {
            _queue.Send(obj, transactionType);
        }

        public void Send(object obj, string label)
        {
            _queue.Send(obj, label);
        }

        public void Send(object obj, string label, MessageQueueTransaction transaction)
        {
            _queue.Send(obj, label, transaction);
        }

        public void Send(object obj, string label, MessageQueueTransactionType transactionType)
        {
            _queue.Send(obj, label, transactionType);
        }

        public void ResetPermissions()
        {
            _queue.ResetPermissions();
        }

        public void SetPermissions(string user, MessageQueueAccessRights rights)
        {
            _queue.SetPermissions(user, rights);
        }

        public void SetPermissions(string user, MessageQueueAccessRights rights, AccessControlEntryType entryType)
        {
            _queue.SetPermissions(user, rights, entryType);
        }

        public void SetPermissions(MessageQueueAccessControlEntry ace)
        {
            _queue.SetPermissions(ace);
        }

        public void SetPermissions(AccessControlList dacl)
        {
            _queue.SetPermissions(dacl);
        }

        public QueueAccessMode AccessMode
        {
            get { return _queue.AccessMode; }
        }

        public bool Authenticate
        {
            get { return _queue.Authenticate; }
            set { _queue.Authenticate = value; }
        }

        public short BasePriority
        {
            get { return _queue.BasePriority; }
            set { _queue.BasePriority = value; }
        }

        public bool CanRead
        {
            get { return _queue.CanRead; }
        }

        public bool CanWrite
        {
            get { return _queue.CanWrite; }
        }

        public Guid Category
        {
            get { return _queue.Category; }
            set { _queue.Category = value; }
        }

        public DateTime CreateTime
        {
            get { return _queue.CreateTime; }
        }

        public DefaultPropertiesToSend DefaultPropertiesToSend
        {
            get { return _queue.DefaultPropertiesToSend; }
            set { _queue.DefaultPropertiesToSend = value; }
        }

        public bool DenySharedReceive
        {
            get { return _queue.DenySharedReceive; }
            set { _queue.DenySharedReceive = value; }
        }

        public EncryptionRequired EncryptionRequired
        {
            get { return _queue.EncryptionRequired; }
            set { _queue.EncryptionRequired = value; }
        }

        public string FormatName
        {
            get { return _queue.FormatName; }
        }

        public IMessageFormatter Formatter
        {
            get { return _queue.Formatter; }
            set { _queue.Formatter = value; }
        }

        public Guid Id
        {
            get { return _queue.Id; }
        }

        public string Label
        {
            get { return _queue.Label; }
            set { _queue.Label = value; }
        }

        public DateTime LastModifyTime
        {
            get { return _queue.LastModifyTime; }
        }

        public string MachineName
        {
            get { return _queue.MachineName; }
            set { _queue.MachineName = value; }
        }

        public long MaximumJournalSize
        {
            get { return _queue.MaximumJournalSize; }
            set { _queue.MaximumJournalSize = value; }
        }

        public long MaximumQueueSize
        {
            get { return _queue.MaximumQueueSize; }
            set { _queue.MaximumQueueSize = value; }
        }

        public MessagePropertyFilter MessageReadPropertyFilter
        {
            get { return _queue.MessageReadPropertyFilter; }
            set { _queue.MessageReadPropertyFilter = value; }
        }

        public string MulticastAddress
        {
            get { return _queue.MulticastAddress; }
            set { _queue.MulticastAddress = value; }
        }

        public string Path
        {
            get { return _queue.Path; }
            set { _queue.Path = value; }
        }

        public string QueueName
        {
            get { return _queue.QueueName; }
            set { _queue.QueueName = value; }
        }

        public IntPtr ReadHandle
        {
            get { return _queue.ReadHandle; }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get { return _queue.SynchronizingObject; }
            set { _queue.SynchronizingObject = value; }
        }

        public bool Transactional
        {
            get { return _queue.Transactional; }
        }

        public bool UseJournalQueue
        {
            get { return _queue.UseJournalQueue; }
            set { _queue.UseJournalQueue = value; }
        }

        public IntPtr WriteHandle
        {
            get { return _queue.WriteHandle; }
        }

        public event PeekCompletedEventHandler PeekCompleted
        {
            add { _queue.PeekCompleted += value; }
            remove { _queue.PeekCompleted -= value; }
        }

        public event ReceiveCompletedEventHandler ReceiveCompleted
        {
            add { _queue.ReceiveCompleted += value; }
            remove { _queue.ReceiveCompleted -= value; }
        }
    }
}