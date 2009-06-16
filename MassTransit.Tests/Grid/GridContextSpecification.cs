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
namespace MassTransit.Tests.Grid
{
	using System.IO;
	using log4net.Config;
	using Parallel;
	using Rhino.Mocks;
	using TextFixtures;

	public abstract class GridContextSpecification :
		LoopbackLocalAndRemoteTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			XmlConfigurator.ConfigureAndWatch(new FileInfo(@"grid\grid.log4net.config"));

			ObjectBuilder.Stub(x => x.GetInstance<ExceptionalWorker>()).Return(new ExceptionalWorker());
			ObjectBuilder.Stub(x => x.GetInstance<FactorLongNumbersTask>()).Return(new FactorLongNumbersTask());
			ObjectBuilder.Stub(x => x.GetInstance<SubTaskWorker<ExceptionalWorker, FactorLongNumber, LongNumberFactored>>())
				.Return(new SubTaskWorker<ExceptionalWorker, FactorLongNumber, LongNumberFactored>(RemoteBus, ObjectBuilder));
			ObjectBuilder.Stub(x => x.GetInstance<SubTaskWorker<FactorLongNumberWorker, FactorLongNumber, LongNumberFactored>>())
				.Return(new SubTaskWorker<FactorLongNumberWorker, FactorLongNumber, LongNumberFactored>(RemoteBus, ObjectBuilder));
		}
	}
}