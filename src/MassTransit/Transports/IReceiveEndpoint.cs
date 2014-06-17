namespace MassTransit.Transports
{
    /// <summary>
    /// A service endpoint has a inbound transport that pushes messages to consumers
    /// </summary>
    public interface IReceiveEndpoint
    {
        
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