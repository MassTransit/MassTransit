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
namespace MassTransit.Util
{
	using System;
	using System.Diagnostics;

	public static class Guard
	{
		public static class Against
		{
			public static bool UseExceptions
			{
				get { return _useExceptions; }
				set { _useExceptions = value; }
			}

			private static bool _useExceptions = true;

			public static void Null(object obj, string message)
			{
				if (UseExceptions)
				{
					if (obj == null)
						throw new PreconditionException(message);
				}
				else
				{
					Trace.Assert(obj != null, "Precondition: " + message);
				}
			}

			public static void NullOrEmpty(string s, string message)
			{
				if (UseExceptions)
				{
					if(string.IsNullOrEmpty(s))
						throw new PreconditionException(message);
				}
				else
				{
					Trace.Assert(string.IsNullOrEmpty(s), "Precondition: " + message);
				}
			}

			public static void IndexOutOfRange(int index, int count)
			{
				if (index < 0 || index >= count)
					throw new ArgumentException("The specified index was out of range");
			}
		}
	}
}