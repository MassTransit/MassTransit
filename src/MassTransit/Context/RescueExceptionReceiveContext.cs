// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;


    public class RescueExceptionReceiveContext :
        ExceptionReceiveContext
    {
        readonly ReceiveContext _context;
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionReceiveContext(ReceiveContext context, Exception exception)
        {
            _context = context;
            _exception = exception;
        }

        Exception ExceptionReceiveContext.Exception => _exception;

        ExceptionInfo ExceptionReceiveContext.ExceptionInfo
        {
            get
            {
                if (_exceptionInfo != null)
                    return _exceptionInfo;

                _exceptionInfo = new FaultExceptionInfo(_exception);

                return _exceptionInfo;
            }
        }

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        Stream ReceiveContext.GetBody()
        {
            return _context.GetBody();
        }

        TimeSpan ReceiveContext.ElapsedTime => _context.ElapsedTime;
        Uri ReceiveContext.InputAddress => _context.InputAddress;
        ContentType ReceiveContext.ContentType => _context.ContentType;
        bool ReceiveContext.Redelivered => _context.Redelivered;
        Headers ReceiveContext.TransportHeaders => _context.TransportHeaders;
        Task ReceiveContext.CompleteTask => _context.CompleteTask;
        bool ReceiveContext.IsDelivered => _context.IsDelivered;
        bool ReceiveContext.IsFaulted => _context.IsFaulted;

        Task ReceiveContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        Task ReceiveContext.NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        void ReceiveContext.AddPendingTask(Task task)
        {
            _context.AddPendingTask(task);
        }
    }
}