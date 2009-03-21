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
namespace MassTransit.Dashboard.Controllers
{
    using System;
    using System.Collections.Generic;
    using Castle.MonoRail.Framework;
    using Services.HealthMonitoring.Messages;

    [Layout("default")]
    public class HealthController :
        SmartDispatcherController
    {
        private readonly IServiceBus _bus;
        private readonly Guid _correlationId = Guid.NewGuid();
        private ServiceBusRequest<HealthController> _request;

        public HealthController(IServiceBus bus)
        {
            _bus = bus;
        }

        //public IAsyncResult BeginView()
        //{
        //    _request = _bus.Request()
        //        .From(this)
        //        .WithCallback(ControllerContext.Async.Callback, ControllerContext.Async.State);

        //    _request.Send(new HealthStatusRequest(_bus.Endpoint.Uri, this.CorrelationId));

        //    return _request;
        //}
        //public void EndView()
        //{
        //    //IAsyncResult r = ControllerContext.Async.Output; //TODO: Do I need this?
        //    PropertyBag.Add("statuses", _response.HealthInformation);
        //}

        public void View()
        {
            //IList<HealthInformation> infos = _cache.List();
            IList<HealthInformation> infos = new List<HealthInformation>();
            PropertyBag.Add("statuses", infos);
        }

        public void Ping(string uri)
        {
            _request = _bus.Request().From(this);

            _request.Send(new Ping(_correlationId));

            _request.AsyncWaitHandle.WaitOne(3000, true); //the consumes method will set the property bag
        }
    }
}