namespace MassTransit.ServiceBus.Internal
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Messaging;

    /// <summary>
    /// Testable Shim Infront of MSMQ
    /// </summary>
    public interface IMsmqQueue
    {
        IAsyncResult BeginPeek();
        IAsyncResult BeginPeek(TimeSpan timeout);
        IAsyncResult BeginPeek(TimeSpan timeout, object stateObject);
        IAsyncResult BeginPeek(TimeSpan timeout, object stateObject, AsyncCallback callback);
        IAsyncResult BeginPeek(TimeSpan timeout, Cursor cursor, PeekAction action, object state, AsyncCallback callback);
        IAsyncResult BeginReceive();
        IAsyncResult BeginReceive(TimeSpan timeout);
        IAsyncResult BeginReceive(TimeSpan timeout, object stateObject);
        IAsyncResult BeginReceive(TimeSpan timeout, object stateObject, AsyncCallback callback);
        IAsyncResult BeginReceive(TimeSpan timeout, Cursor cursor, object state, AsyncCallback callback);
        void Close();
        Cursor CreateCursor();
        Message EndPeek(IAsyncResult asyncResult);
        Message EndReceive(IAsyncResult asyncResult);
        Message[] GetAllMessages();
        IEnumerator GetEnumerator();
        MessageEnumerator GetMessageEnumerator();
        MessageEnumerator GetMessageEnumerator2();
        Message Peek();
        Message Peek(TimeSpan timeout);
        Message Peek(TimeSpan timeout, Cursor cursor, PeekAction action);
        Message PeekById(string id);
        Message PeekById(string id, TimeSpan timeout);
        Message PeekByCorrelationId(string correlationId);
        Message PeekByCorrelationId(string correlationId, TimeSpan timeout);
        void Purge();
        Message Receive();
        Message Receive(MessageQueueTransaction transaction);
        Message Receive(MessageQueueTransactionType transactionType);
        Message Receive(TimeSpan timeout);
        Message Receive(TimeSpan timeout, Cursor cursor);
        Message Receive(TimeSpan timeout, MessageQueueTransaction transaction);
        Message Receive(TimeSpan timeout, MessageQueueTransactionType transactionType);
        Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransaction transaction);
        Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransactionType transactionType);
        Message ReceiveById(string id);
        Message ReceiveById(string id, MessageQueueTransaction transaction);
        Message ReceiveById(string id, MessageQueueTransactionType transactionType);
        Message ReceiveById(string id, TimeSpan timeout);
        Message ReceiveById(string id, TimeSpan timeout, MessageQueueTransaction transaction);
        Message ReceiveById(string id, TimeSpan timeout, MessageQueueTransactionType transactionType);
        Message ReceiveByCorrelationId(string correlationId);
        Message ReceiveByCorrelationId(string correlationId, MessageQueueTransaction transaction);
        Message ReceiveByCorrelationId(string correlationId, MessageQueueTransactionType transactionType);
        Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout);
        Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout, MessageQueueTransaction transaction);

        Message ReceiveByCorrelationId(string correlationId, TimeSpan timeout,
                                       MessageQueueTransactionType transactionType);

        Message ReceiveByLookupId(long lookupId);
        Message ReceiveByLookupId(MessageLookupAction action, long lookupId, MessageQueueTransactionType transactionType);
        Message ReceiveByLookupId(MessageLookupAction action, long lookupId, MessageQueueTransaction transaction);
        Message PeekByLookupId(long lookupId);
        Message PeekByLookupId(MessageLookupAction action, long lookupId);
        void Refresh();
        void Send(object obj);
        void Send(object obj, MessageQueueTransaction transaction);
        void Send(object obj, MessageQueueTransactionType transactionType);
        void Send(object obj, string label);
        void Send(object obj, string label, MessageQueueTransaction transaction);
        void Send(object obj, string label, MessageQueueTransactionType transactionType);
        void ResetPermissions();
        void SetPermissions(string user, MessageQueueAccessRights rights);
        void SetPermissions(string user, MessageQueueAccessRights rights, AccessControlEntryType entryType);
        void SetPermissions(MessageQueueAccessControlEntry ace);
        void SetPermissions(AccessControlList dacl);

        QueueAccessMode AccessMode { get; }

        bool Authenticate { get; set; }

        short BasePriority { get; set; }

        bool CanRead { get; }

        bool CanWrite { get; }

        Guid Category { get; set; }

        DateTime CreateTime { get; }

        DefaultPropertiesToSend DefaultPropertiesToSend { get; set; }

        bool DenySharedReceive { get; set; }

        EncryptionRequired EncryptionRequired { get; set; }

        string FormatName { get; }

        IMessageFormatter Formatter { get; set; }

        Guid Id { get; }

        string Label { get; set; }

        DateTime LastModifyTime { get; }

        string MachineName { get; set; }

        long MaximumJournalSize { get; set; }

        long MaximumQueueSize { get; set; }

        MessagePropertyFilter MessageReadPropertyFilter { get; set; }

        string MulticastAddress { get; set; }

        string Path { get; set; }

        string QueueName { get; set; }

        IntPtr ReadHandle { get; }

        ISynchronizeInvoke SynchronizingObject { get; set; }

        bool Transactional { get; }

        bool UseJournalQueue { get; set; }

        IntPtr WriteHandle { get; }

        event PeekCompletedEventHandler PeekCompleted;

        event ReceiveCompletedEventHandler ReceiveCompleted;
    }
}