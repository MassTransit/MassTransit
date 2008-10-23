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
namespace MassTransit.ServiceBus.Tests.Grid
{
    using System.IO;
    using MassTransit.Grid;

    public abstract class GridContextSpecification :
        LocalAndRemoteTestContext
    {
        static GridContextSpecification()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(@"grid\grid.log4net.config"));
        }

        protected override void Before_each()
        {
            Container.AddComponent<ExceptionalWorker>();
            Container.AddComponent<FactorLongNumbersTask>();
            Container.AddComponent<SubTaskWorker<ExceptionalWorker, FactorLongNumber,  LongNumberFactored>>();
            Container.AddComponent<SubTaskWorker<FactorLongNumberWorker, FactorLongNumber,  LongNumberFactored>>();
        }

        protected override void After_each()
        {
            
        }
    }
}