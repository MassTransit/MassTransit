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
namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;

	/// <summary>
	/// A saga repository is used by the service bus to dispatch messages to sagas
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public interface ISagaRepository<T> :
		IDisposable
        where T : class
    {
		/// <summary>
		/// Initiates a new saga, creating a new instance of the saga in the repository and
		/// throwing an exception if the saga already exists
		/// </summary>
		/// <param name="id">The identifier for the saga</param>
    	IEnumerator<T> InitiateNewSaga(Guid id);

		/// <summary>
		/// Orchestrates an existing saga, loading the existing instance of the saga and
		/// throwing an exception if the saga does not exist
		/// </summary>
		/// <param name="id">The identifier for the saga</param>
    	IEnumerator<T> OrchestrateExistingSaga(Guid id);
    }
}