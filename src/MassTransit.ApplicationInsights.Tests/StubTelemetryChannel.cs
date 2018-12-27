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
namespace MassTransit.ApplicationInsights.Tests
{
	using System;
	using Microsoft.ApplicationInsights.Channel;

	/// <summary>
	/// A stub of <see cref="ITelemetryChannel"/>.
	/// </summary>
	public sealed class StubTelemetryChannel : ITelemetryChannel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StubTelemetryChannel"/> class.
		/// </summary>
		public StubTelemetryChannel()
		{
			this.OnSend = telemetry => { };
			this.OnFlush = () => { };
			this.OnDispose = () => { };
		}

		/// <summary>
		/// Gets or sets a value indicating whether this channel is in developer mode.
		/// </summary>
		public bool? DeveloperMode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the channel's URI. To this URI the telemetry is expected to be sent.
		/// </summary>
		public string EndpointAddress { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to throw an error.
		/// </summary>
		public bool ThrowError { get; set; }

		/// <summary>
		/// Gets or sets the callback invoked by the <see cref="Send"/> method.
		/// </summary>
		public Action<ITelemetry> OnSend { get; set; }

		public Action OnFlush { get; set; }

		public Action OnDispose { get; set; }

		/// <summary>
		/// Implements the <see cref="ITelemetryChannel.Send"/> method by invoking the <see cref="OnSend"/> callback.
		/// </summary>
		public void Send(ITelemetry item)
		{
			if (this.ThrowError)
			{
				throw new Exception("test error");
			}

			this.OnSend(item);
		}

		/// <summary>
		/// Implements the <see cref="IDisposable.Dispose"/> method.
		/// </summary>
		public void Dispose()
		{
			this.OnDispose();
		}

		/// <summary>
		/// Implements  the <see cref="ITelemetryChannel.Flush" /> method.
		/// </summary>
		public void Flush()
		{
			this.OnFlush();
		}
	}
}
