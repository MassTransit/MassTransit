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
namespace MassTransit.Context
{
    using System;
    using System.Web;

    /// <summary>
    /// The default context provider using thread local storage
    /// </summary>
    public static class ContextStorage
    {
        static ContextStorageProvider _provider;

        public static IConsumeContext CurrentConsumeContext
        {
            get { return Provider.ConsumeContext; }
            set
            {
                if (value == null || value.GetType() == typeof (InvalidConsumeContext))
                    Provider.ConsumeContext = null;
                else
                    Provider.ConsumeContext = value;
            }
        }

        static ContextStorageProvider Provider
        {
            get { return _provider ?? (_provider = GetDefaultProvider()); }
        }

        public static IConsumeContext<T> MessageContext<T>()
            where T : class
        {
            var context = CurrentConsumeContext as IConsumeContext<T>;
            if (context == null)
                throw new InvalidOperationException("The specified consumer context type was not found");

            return context;
        }

        public static OldSendContext<T> CreateSendContext<T>(T message)
            where T : class
        {
            var context = new OldSendContext<T>(message);

            SetReceiveContextForSend(context);

            return context;
        }


        internal static void SetReceiveContextForSend<T>(ISendContext<T> context)
            where T : class
        {
            IConsumeContext currentConsumeContext = CurrentConsumeContext;
            if (currentConsumeContext != null)
            {
                var receiveContext = currentConsumeContext as IReceiveContext;
                if (receiveContext != null)
                    context.SetReceiveContext(receiveContext);
                else if (currentConsumeContext.BaseContext != null)
                {
                    context.SetReceiveContext(currentConsumeContext.BaseContext);
                }
            }
        }
        public static IConsumeContext Context()
        {
            IConsumeContext context = CurrentConsumeContext;
            if (context == null)
                throw new InvalidOperationException("No consumer context was found");

            return context;
        }

        public static void Context(Action<IConsumeContext> contextCallback)
        {
            IConsumeContext context = CurrentConsumeContext;
            if (context == null)
                throw new InvalidOperationException("No consumer context was found");

            contextCallback(context);
        }

        public static TResult Context<TResult>(Func<IConsumeContext, TResult> contextCallback)
        {
            IConsumeContext context = CurrentConsumeContext;
            if (context == null)
                throw new InvalidOperationException("No consumer context was found");

            return contextCallback(context);
        }

        static ContextStorageProvider GetDefaultProvider()
        {
            return new ThreadStaticContextStorageProvider();
        }
    }
}