// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Initializers
{
    using System;
    using GreenPipes;


    /// <summary>
    /// The context of the message being initialized
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public interface InitializeMessageContext<out TMessage, out TInput> :
        InitializeMessageContext<TMessage>
        where TMessage : class
        where TInput : class
    {
        /// <summary>
        /// The input to the initializer
        /// </summary>
        TInput Input { get; }
    }


    /// <summary>
    /// The context of the message being initialized
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface InitializeMessageContext<out TMessage> :
        InitializeMessageContext
        where TMessage : class
    {
        /// <summary>
        /// The message being initialized
        /// </summary>
        TMessage Message { get; }

        InitializeMessageContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class;
    }


    public interface InitializeMessageContext :
        PipeContext
    {
        /// <summary>
        /// The message type being initialized
        /// </summary>
        Type MessageType { get; }
    }
}