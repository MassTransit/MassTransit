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
namespace MassTransit.ApplicationInsights
{
	using System;
	using System.Threading.Tasks;
	using GreenPipes;
	using Microsoft.ApplicationInsights;
	using Microsoft.ApplicationInsights.DataContracts;

	public class ApplicationInsightsSendFilter<T> 
		: IFilter<T>
		where T : class, SendContext
	{
		const string StepName = "MassTransit:Send";
		const string DependencyType = "Queue";

		private readonly TelemetryClient _telemetryClient;
		private readonly string _telemetryHeaderRootKey;
		private readonly string _telemetryHeaderParentKey;

		public ApplicationInsightsSendFilter(TelemetryClient telemetryClient
			, string telemetryHeaderRootKey
			, string telemetryHeaderParentKey
			)
		{
			_telemetryClient = telemetryClient;
			_telemetryHeaderRootKey = telemetryHeaderRootKey;
			_telemetryHeaderParentKey = telemetryHeaderParentKey;
		}

		public void Probe(ProbeContext context)
		{
			context.CreateFilterScope("TelemetrySendFilter");
		}

		public async Task Send(T context, IPipe<T> next)
		{
			var requestTelemetry = new DependencyTelemetry()
			{
				Name = $"{StepName} {context.DestinationAddress}",
				Type = DependencyType,
				Data = $"{StepName} {context.DestinationAddress}"
			};

			using (var operation = _telemetryClient.StartOperation(requestTelemetry))
			{
				context.Headers.Set(_telemetryHeaderRootKey, operation.Telemetry.Context.Operation.Id);
				context.Headers.Set(_telemetryHeaderParentKey, operation.Telemetry.Id);

				try
				{
					await next.Send(context);

					operation.Telemetry.Success = true;
				}
				catch (Exception e)
				{
					//In the future: you can log additionaly message: context.Message
					_telemetryClient.TrackException(e, operation.Telemetry.Properties);

					operation.Telemetry.Success = false;
					throw;
				}
				finally
				{
					_telemetryClient.StopOperation(operation);
				}
			}
		}
	}
}