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
namespace MassTransit.Util
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class WeakReference<T> : WeakReference
		where T : class
	{
		public WeakReference(T target)
			: base(target)
		{
		}

		public WeakReference(T target, bool trackResurrection)
			: base(target, trackResurrection)
		{
		}

		protected WeakReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public new T Target
		{
			get { return (T) base.Target; }
			set { base.Target = value; }
		}
	}
}