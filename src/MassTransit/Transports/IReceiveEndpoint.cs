// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// A service endpoint has a inbound transport that pushes messages to consumers
    /// </summary>
    public interface IReceiveEndpoint
    {
        Uri InputAddress { get; }

        /// <summary>
        /// The input pipe for the endpoint
        /// </summary>
        IConsumePipe ConsumePipe { get; }

        /// <summary>
        /// Starts recieving from the inbound transport, returning a Task that is completed
        /// once the transport is closed. The cancellationToken is used to stop the receive
        /// endpoint from receiving messages from the inbound transport.
        /// </summary>
        /// <param name="stopToken">The token which will be cancelled once the receive endpoint should shut down</param>
        /// <returns></returns>
        Task Start(CancellationToken stopToken);
    }


    /*
     * 
                        failedMessageException = null;

                        if (successfulMessageId != null)
                        {
                            _log.DebugFormat("Received Successfully: {0}", successfulMessageId);

                            _tracker.MessageWasReceivedSuccessfully(successfulMessageId);
                            successfulMessageId = null;
                        }


                        Action<IReceiveContext> receive;
                        try
                        {
                            acceptContext.SetEndpoint(this);

                            IMessageSerializer serializer;
                            if (!_supportedSerializers.TryGetSerializer(acceptContext.ContentType, out serializer))
                                throw new SerializationException(
                                    string.Format("The content type could not be deserialized: {0}",
                                        acceptContext.ContentType));

                            serializer.Deserialize(acceptContext);

                            receive = receiver(acceptContext);
                            if (receive == null)
                            {
                                Address.LogSkipped(acceptMessageId);

                                if (_tracker.IncrementRetryCount(acceptMessageId))
                                    return MoveMessageToErrorTransport;

                                return null;
                            }
                        }
                        catch (SerializationException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Unrecognized message " + Address + ":" + acceptMessageId, sex);

                            _tracker.IncrementRetryCount(acceptMessageId, sex);
                            return MoveMessageToErrorTransport;
                        }
                        catch (Exception ex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("An exception was thrown preparing the message consumers", ex);

                            if(_tracker.IncrementRetryCount(acceptMessageId, ex))
                            {
                                if (!_tracker.IsRetryEnabled)
                                {
                                    acceptContext.ExecuteFaultActions(acceptContext.GetFaultActions());
                                    return MoveMessageToErrorTransport;
                                }
                            }
                            return null;
                        }

                        return receiveContext =>
                            {
                                string receiveMessageId = receiveContext.OriginalMessageId ?? receiveContext.MessageId;
                                try
                                {
                                    receive(receiveContext);

                                    successfulMessageId = receiveMessageId;
                                }
                                catch (Exception ex)
                                {
                                    if (_log.IsErrorEnabled)
                                        _log.Error("An exception was thrown by a message consumer", ex);

                                    faultActions = receiveContext.GetFaultActions();
                                    if(_tracker.IncrementRetryCount(receiveMessageId, ex, faultActions))
                                    {
                                        if (!_tracker.IsRetryEnabled)
                                        {
                                            receiveContext.ExecuteFaultActions(faultActions);
                                            MoveMessageToErrorTransport(receiveContext);

                                            return;
                                        }
                                    }

                                    if(!receiveContext.IsTransactional)
                                    {
                                        SaveMessageToInboundTransport(receiveContext);
                                        return;
                                    }

                                    throw;
                                }
                            };
                    }, timeout);

                if (failedMessageException != null)
                {
                    if(_log.IsErrorEnabled)
                        _log.ErrorFormat("Throwing Original Exception: {0}", failedMessageException.GetType());

                    throw failedMessageException;
                }
            }
            catch (Exception ex)
            {
                if (successfulMessageId != null)
                {
                    _log.DebugFormat("Increment Retry Count: {0}", successfulMessageId);

                    _tracker.IncrementRetryCount(successfulMessageId, ex);
                    successfulMessageId = null;
                }
                throw;
            }
            finally
            {
                if (successfulMessageId != null)
                {
                    _log.DebugFormat("Received Successfully: {0}", successfulMessageId);

                    _tracker.MessageWasReceivedSuccessfully(successfulMessageId);
                    successfulMessageId = null;
                }
            }*/
}