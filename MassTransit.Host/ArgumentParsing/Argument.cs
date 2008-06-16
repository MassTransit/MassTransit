/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransitHost.ArgumentParsing
{
	public class Argument : IArgument
	{
		private readonly string _value;
		private readonly string _key;

		public Argument(string value)
		{
			_value = value;
		}

		public Argument(string key, string value)
		{
			_key = key;
			_value = value;
		}

		#region IArgument Members

		public string Value
		{
			get { return _value; }
		}

		public string Key
		{
			get { return _key; }
		}

		#endregion
	}
}