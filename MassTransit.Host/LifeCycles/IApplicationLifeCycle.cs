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
namespace MassTransit.Host.LifeCycles
{
	using System;
	using Actions;
	using Castle.Windsor;

	public interface IApplicationLifeCycle :
		IDisposable
	{
		/// <summary>
		/// The default action to be used by the host (console or gui for the most part)
		/// </summary>
		NamedAction DefaultAction { get; }

		IWindsorContainer Container { get; }

		void Initialize();
		void Start();
		void Stop();

		//To be fired after disose is complete
		event Action<IApplicationLifeCycle> Completed;
	}
}