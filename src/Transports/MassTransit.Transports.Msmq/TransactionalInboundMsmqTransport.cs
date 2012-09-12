// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Transactions;
    using Logging;

    [DebuggerDisplay("IN:TX:{Address}")]
    public class TransactionalInboundMsmqTransport :
        InboundMsmqTransport
    {
        static readonly ILog _log = Logger.Get(typeof(TransactionalInboundMsmqTransport));
        readonly IsolationLevel _isolationLevel;
        readonly TimeSpan _transactionTimeout;

        public TransactionalInboundMsmqTransport(IMsmqEndpointAddress address,
            ConnectionHandler<MessageQueueConnection> connectionHandler,
            TimeSpan transactionTimeout,
            IsolationLevel isolationLevel)
            : base(address, connectionHandler)
        {
            _transactionTimeout = transactionTimeout;
            _isolationLevel = isolationLevel;
        }

        protected override void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout,
            Action<Message> receiveAction)
        {
            var options = new TransactionOptions
                {
                    IsolationLevel = _isolationLevel,
                    Timeout = _transactionTimeout,
                };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                using (Message message = enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.Automatic))
                {
                    receiveAction(message);
                }

                scope.Complete();
            }
        }
    }
}