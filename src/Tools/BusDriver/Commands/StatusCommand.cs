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
namespace BusDriver.Commands
{
    using System.Threading;
    using Formatting;
    using MassTransit;
    using MassTransit.Diagnostics.Introspection;
    using log4net;
    using Magnum.Extensions;

    public class StatusCommand :
        Consumes<CurrentBusStatus>.All,
        Command,
        IPendingCommand
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (StatusCommand));

        readonly string _uriString;
        UnsubscribeAction _unsubscribe;
        readonly ManualResetEvent _complete;

        public StatusCommand(string uriString)
        {
            _uriString = uriString;
            _complete = new ManualResetEvent(false);
        }

        public bool Execute()
        {
            var uri = _uriString.ToUri("The status URI was invalid");

            var bus = Program.Bus;

            var endpoint = bus.GetEndpoint(uri);

            _log.DebugFormat("Sending status request to '{0}'", uri);

            _unsubscribe = bus.SubscribeInstance(this);

            endpoint.Send<GetBusStatus>(new GetBusStatusImpl(), x=>x.SendResponseTo(bus));

            Program.AddPendingCommand(this);

            return true;
        }


        public void Consume(CurrentBusStatus message)
        {
            if (_unsubscribe != null)
                _unsubscribe();
            _unsubscribe = null;

            ITextBlock text = new TextBlock()
                .BeginBlock("Status URI:", _uriString)
                .EndBlock();

            foreach (var entry in message.Entries)
            {
                text.BodyFormat("{0}:{1}", entry.Key, entry.Value);

            }
            text.EndBlock();

            _log.Info(text.ToString());
            _complete.Set();
        }


        public string Description
        {
            get { return "status on '{0}'".FormatWith(_uriString); }
        }

        public WaitHandle WaitHandle
        {
            get { return _complete; }
        }
    }
}