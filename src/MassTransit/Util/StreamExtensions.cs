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
namespace MassTransit.Util
{
	using System;
	using System.IO;

#if NET35
	public static class StreamExtensions
	{
		public static void CopyTo(this Stream fromStream, Stream toStream)
		{
			if (fromStream == null)
				throw new ArgumentNullException("fromStream");
			if (toStream == null)
				throw new ArgumentNullException("toStream");

			var bytes = new byte[8192];
			int dataRead;
			while ((dataRead = fromStream.Read(bytes, 0, bytes.Length)) > 0)
				toStream.Write(bytes, 0, dataRead);
		}
	}

#endif

}