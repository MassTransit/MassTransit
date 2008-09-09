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
namespace ClientGUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;
    using Messages;

    public class TrapperKeeper :
        Consumes<QuestionAnswered>.Selected
    {
        private readonly ManualResetEvent _done = new ManualResetEvent(false);
        private readonly Stopwatch _stopwatch;
        private readonly Stopwatch _sendingStopwatch;
        private readonly int _target;
        private int _answered;
        private readonly Dictionary<Guid, SubmitQuestion> _questions = new Dictionary<Guid, SubmitQuestion>();

        public TrapperKeeper(int target)
        {
            _target = target;
            _stopwatch = Stopwatch.StartNew();
            _sendingStopwatch = Stopwatch.StartNew();
        }

        public ManualResetEvent Done
        {
            get { return _done; }
        }

        public long SendRate
        {
            get { return SendingElapsedMilliseconds/_target; }
        }
        public long Rate
        {
            get { return ElapsedMilliseconds/_target; }
        }

        public long SendingElapsedMilliseconds
        {
            get { return _sendingStopwatch.ElapsedMilliseconds; }
        }
        public long ElapsedMilliseconds
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }

        public int Answered
        {
            get { return _answered; }
        }

        public void Consume(QuestionAnswered message)
        {
            if (Interlocked.Increment(ref _answered) == _target)
            {
                _stopwatch.Stop();
                _done.Set();
            }
        }

        public bool Accept(QuestionAnswered message)
        {
            lock (_questions)
                return _questions.ContainsKey(message.CorrelationId);
        }

        public void Add(SubmitQuestion question)
        {
            lock (_questions)
                _questions.Add(question.CorrelationId, question);
        }

        public void SendComplete()
        {
            _sendingStopwatch.Stop();
        }

        
    }
}