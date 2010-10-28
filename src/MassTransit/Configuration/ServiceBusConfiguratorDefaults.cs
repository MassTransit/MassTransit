// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
	using System;
	using Exceptions;
	using Magnum.Extensions;

    public class ServiceBusConfiguratorDefaults :
		IServiceBusConfiguratorDefaults
	{
		public ServiceBusConfiguratorDefaults()
		{
			AutoStart = true;
			ReceiveTimeout = 3.Seconds();
			ConcurrentReceiverLimit = 1;
			ConcurrentConsumerLimit = Environment.ProcessorCount*4;
		}

		protected IObjectBuilder ObjectBuilder { get; private set; }
        protected IEndpointFactory EndpointFactory { get; private set; }
		protected TimeSpan ReceiveTimeout { get; private set; }
		protected int ConcurrentReceiverLimit { get; private set; }
		protected int ConcurrentConsumerLimit { get; private set; }
		protected bool AutoStart { get; private set; }
		protected bool AutoSubscribe { get; private set; }
		protected bool PurgeBeforeStart{get;private set;}
		protected Uri ErrorUri { get; private set; }

		public void SetObjectBuilder(IObjectBuilder objectBuilder)
		{
			ObjectBuilder = objectBuilder;
		}

        public void SetEndpointFactory(IEndpointFactory endpointFactory)
        {
            EndpointFactory = endpointFactory;
        }
		public void SendErrorsTo(string uriString)
		{
			try
			{
				ErrorUri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The Uri for the error endpoint is invalid: " + uriString, ex);
			}
		}

		public void SendErrorsTo(Uri uri)
		{
			ErrorUri = uri;
		}


		public void EnableAutoSubscribe()
		{
			AutoSubscribe = true;
		}

		public void DisableAutoStart()
		{
			AutoStart = false;
		}

		public void PurgeBeforeStarting()
		{
			PurgeBeforeStart = true;
		}

		public void SetReceiveTimeout(TimeSpan receiveTimeout)
		{
			ReceiveTimeout = receiveTimeout;
		}

		public void SetConcurrentConsumerLimit(int concurrentConsumerLimit)
		{
			ConcurrentConsumerLimit = concurrentConsumerLimit;
		}

		public void SetConcurrentReceiverLimit(int concurrentReceiverLimit)
		{
			ConcurrentReceiverLimit = concurrentReceiverLimit;
		}

		public void ApplyTo(IServiceBusConfigurator configurator)
		{
			if (ObjectBuilder != null)
				configurator.SetObjectBuilder(ObjectBuilder);

            if(EndpointFactory != null)
                configurator.SetEndpointFactory(EndpointFactory);

			if (ErrorUri != null)
				configurator.SendErrorsTo(ErrorUri);

			configurator.SetReceiveTimeout(ReceiveTimeout);
			configurator.SetConcurrentConsumerLimit(ConcurrentConsumerLimit);
			configurator.SetConcurrentReceiverLimit(ConcurrentReceiverLimit);

			if (!AutoStart)
				configurator.DisableAutoStart();
			if (AutoSubscribe)
				configurator.EnableAutoSubscribe();
		}
	}
}