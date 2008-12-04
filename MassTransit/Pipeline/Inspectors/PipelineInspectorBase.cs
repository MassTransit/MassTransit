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
namespace MassTransit.Pipeline.Inspectors
{
	using System;
	using Util;

	/// <summary>
	/// Base class for pipeline inspectors
	/// </summary>
	public class PipelineInspectorBase :
		ReflectiveVisitorBase,
		IPipelineInspector
	{
		public PipelineInspectorBase()
			: base("Inspect")
		{
		}

		public bool Inspect(object obj)
		{
			return Visit(obj);
		}

		public bool Inspect(object obj, Func<bool> action)
		{
			return Visit(obj, action);
		}
	}
}