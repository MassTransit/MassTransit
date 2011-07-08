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
namespace MassTransit.Configurators
{
	/// <summary>
	/// Reports information about the configuration before configuring
	/// so that corrections can be made without allocating resources, etc.
	/// </summary>
	public interface ValidationResult
	{
		/// <summary>
		/// The disposition of the result, any Failure items will prevent
		/// the configuration from completing.
		/// </summary>
		ValidationResultDisposition Disposition { get; }

		/// <summary>
		/// The message associated with the result
		/// </summary>
		string Message { get; }

		/// <summary>
		/// The key associated with the result (chained if configurators are nested)
		/// </summary>
		string Key { get; }

		/// <summary>
		/// The value associated with the result
		/// </summary>
		string Value { get; }
	}
}