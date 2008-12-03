// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Pipeline
{
	using System;

	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Subscribe a component type to the pipeline that is resolved from the container for each message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <returns></returns>
		public static Func<bool> Subscribe<TComponent>(this InboundPipeline pipeline) where TComponent : class
		{
			return pipeline.Configure(x => x.Subscribe<TComponent>());
		}

		/// <summary>
		/// Subscribe a component to the pipeline that handles every message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <param name="instance">The instance that will handle the messages</param>
		/// <returns></returns>
		public static Func<bool> Subscribe<TComponent>(this InboundPipeline pipeline, TComponent instance)
			where TComponent : class
		{
			return pipeline.Configure(x => x.Subscribe(instance));
		}
	}
}