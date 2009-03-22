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
    using System.Collections.Generic;
    using Castle.MonoRail.Framework;
    using Services.Subscriptions;

    [Layout("default")]
    public class SubscriptionsController : 
        SmartDispatcherController
    {
        

        public SubscriptionsController()
        {
            
        }

        public void View()
        {
            List <Subscription> subs = new List<Subscription>();
            subs.Sort(delegate(Subscription left, Subscription right)
                          {
                              return left.EndpointUri.ToString().CompareTo(right.EndpointUri.ToString());
                          });
            PropertyBag.Add("subscriptions", subs);
        }
    }
}