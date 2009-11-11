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
namespace MassTransit.Grid.Messages
{
	using System;

	public abstract class NotifyNodeMessageBase
	{
		protected NotifyNodeMessageBase(NotifyNodeMessageBase source)
		{
			ControlUri = source.ControlUri;
			DataUri = source.DataUri;
			Created = source.Created;
			LastUpdated = source.LastUpdated;
		}

		protected NotifyNodeMessageBase(Uri controlUri, Uri dataUri, DateTime created, DateTime lastUpdated)
		{
			ControlUri = controlUri;
			DataUri = dataUri;
			Created = created;
			LastUpdated = lastUpdated;
		}

		protected NotifyNodeMessageBase()
		{
		}

		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}