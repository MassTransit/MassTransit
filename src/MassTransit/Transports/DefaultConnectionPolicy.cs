// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Magnum.Extensions;

    public class DefaultConnectionPolicy :
        ConnectionPolicy
    {
        readonly ConnectionHandler _connectionHandler;
        readonly TimeSpan _reconnectDelay;
        readonly ILog _log = Logger.Get(typeof(DefaultConnectionPolicy));
        readonly ReaderWriterLockSlim _connectionLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public DefaultConnectionPolicy(ConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
            _reconnectDelay = 10.Seconds();
        }

        public void Execute(Action callback)
        {
            try
            {
                try
                {
                    // wait here so we can be sure that there is not a reconnect in progress
                    _connectionLock.EnterReadLock();
                    callback();
                }
                finally
                {
                    _connectionLock.ExitReadLock();
                }
            }
            catch (InvalidConnectionException ex)
            {
                _log.Warn("Invalid Connection when executing callback", ex.InnerException);

                Reconnect();

                if (_log.IsDebugEnabled)
                {
                    _log.Debug("Retrying callback after reconnect.");
                }

                try
                {
                    // wait here so we can be sure that there is not a reconnect in progress
                    _connectionLock.EnterReadLock();
                    callback();
                }
                finally
                {
                    _connectionLock.ExitReadLock();
                }
            }
        }

        void Reconnect()
        {
            if (_connectionLock.TryEnterWriteLock((int)_reconnectDelay.TotalMilliseconds/2))
            {
                try
                {
                    if (_log.IsDebugEnabled)
                    {
                        _log.Debug("Disconnecting connection handler.");
                    }
                    _connectionHandler.Disconnect();

                    if (_reconnectDelay > TimeSpan.Zero)
                        Thread.Sleep(_reconnectDelay);

                    if (_log.IsDebugEnabled)
                    {
                        _log.Debug("Re-connecting connection handler...");
                    }
                    _connectionHandler.Connect();
                }
                catch (Exception)
                {
                    _log.Warn("Failed to reconnect, deferring to connection policy for reconnection");
                    _connectionHandler.ForceReconnect(_reconnectDelay);
                }
                finally
                {
                    _connectionLock.ExitWriteLock();
                }
            }
            else
            {
                try
                {
                    _connectionLock.EnterReadLock();
                    if (_log.IsDebugEnabled)
                    {
                        _log.Debug("Waiting for reconnect in another thread.");
                    }
                }
                finally
                {
                    _connectionLock.ExitReadLock();
                }
            }
        }
    }
}