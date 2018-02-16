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
namespace MassTransit
{
    using System;
    using GreenPipes;


    public interface IReceivePipelineConfigurator
    {
        /// <summary>
        /// Configure the Receive pipeline
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureReceive(Action<IReceivePipeConfigurator> callback);

        /// <summary>
        /// Configure the dead letter pipeline, which is called if the message is not consumed
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback);

        /// <summary>
        /// Configure the exception pipeline, which is called if there are uncaught consumer exceptions
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback);
    }
}