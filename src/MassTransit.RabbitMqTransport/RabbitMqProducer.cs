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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Transports;


    public class RabbitMqProducer 
    {
//        readonly Cache<ulong, TaskCompletionSource<bool>> _confirms;
        static readonly ILog _log = Logger.Get<RabbitMqProducer>();
        readonly bool _bindToQueue;
        readonly object _lock = new object();
        IModel _channel;
        bool _immediate;
        bool _mandatory;

        public RabbitMqProducer( bool bindToQueue)
        {
            _bindToQueue = bindToQueue;
  //          _confirms = new ConcurrentCache<ulong, TaskCompletionSource<bool>>();
        }


        void BindEvents(IModel channel)
        {
            channel.BasicAcks += HandleAck;
            channel.BasicNacks += HandleNack;
            channel.BasicReturn += HandleReturn;
            channel.FlowControl += HandleFlowControl;
            channel.ModelShutdown += HandleModelShutdown;
            channel.ConfirmSelect();
        }



        void FailPendingConfirms()
        {
            try
            {

//                _confirms.Each((id, task) => task.TrySetException(exception));
            }
            catch (Exception ex)
            {
                _log.Error("Exception while failing pending confirms", ex);
            }

  //          _confirms.Clear();
        }

        public IBasicProperties CreateProperties()
        {
            lock (_lock)
            {

                return _channel.CreateBasicProperties();
            }
        }

        public void Publish(string exchangeName, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {

                _channel.BasicPublish(exchangeName, "", properties, body);
            }
        }

        public Task PublishAsync(string exchangeName, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {

                ulong deliveryTag = _channel.NextPublishSeqNo;

                var task = new TaskCompletionSource<bool>();
    //            _confirms.Add(deliveryTag, task);

                try
                {
                    _channel.BasicPublish(exchangeName, "", _mandatory, _immediate, properties, body);
                }
                catch
                {
      //              _confirms.Remove(deliveryTag);                    
                    throw;
                }

                return task.Task;
            }
        }

        void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            try
            {
                FailPendingConfirms();
            }
            catch (Exception ex)
            {
                _log.Error("Fail pending confirms failed during model shutdown", ex);
            }
        }

        void HandleFlowControl(IModel sender, FlowControlEventArgs args)
        {
        }

        void HandleReturn(IModel model, BasicReturnEventArgs args)
        {
        }

        void HandleNack(IModel model, BasicNackEventArgs args)
        {
            IEnumerable<ulong> ids = Enumerable.Repeat(args.DeliveryTag, 1);
        //    if (args.Multiple)
          //      ids = _confirms.GetAllKeys().Where(x => x <= args.DeliveryTag);

            var exception = new InvalidOperationException("Publish was nacked by the broker");

            foreach (ulong id in ids)
            {
            //    _confirms[id].TrySetException(exception);
              //  _confirms.Remove(id);
            }
        }

        void HandleAck(IModel model, BasicAckEventArgs args)
        {
            IEnumerable<ulong> ids = Enumerable.Repeat(args.DeliveryTag, 1);
           // if (args.Multiple)
                //ids = _confirms.GetAllKeys().Where(x => x <= args.DeliveryTag);

            foreach (ulong id in ids)
            {
             //   _confirms[id].TrySetResult(true);
               // _confirms.Remove(id);
            }
        }
    }
}