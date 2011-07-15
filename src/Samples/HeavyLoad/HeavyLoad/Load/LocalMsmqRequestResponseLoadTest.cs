using System;
using System.Threading;
using MassTransit;
using Magnum.Extensions;

namespace HeavyLoad.Load
{
    public class LocalMsmqRequestResponseLoadTest :
        IDisposable
    {
        const int _repeatCount = 10000;
        readonly IServiceBus _bus;
        readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

        int _requestCounter;
        int _responseCounter;

        public LocalMsmqRequestResponseLoadTest()
        {
            _bus = ServiceBusFactory.New(x =>
            {
                x.ReceiveFrom("msmq://localhost/heavy_load");
                x.SetPurgeOnStartup(true);

                x.UseMsmq();

                x.Subscribe(s =>
                {
                    s.Handler<GeneralMessage>(Handle);
                });
            });
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Run(StopWatch stopWatch)
        {
            stopWatch.Start();

            CheckPoint publishCheckpoint = stopWatch.Mark("Sending " + _repeatCount + " messages");
            CheckPoint receiveCheckpoint = stopWatch.Mark("Request/Response " + _repeatCount + " messages");

            for (int index = 0; index < _repeatCount; index++)
            {
                _bus.MakeRequest(x =>
                {
                    x.Publish(new GeneralMessage());
                    Interlocked.Increment(ref _requestCounter);
                })
                    .When<SimpleResponse>()
                    .IsReceived(msg => { 
                        Interlocked.Increment(ref _responseCounter);

                        if (_responseCounter == _repeatCount)
                            _responseEvent.Set();
                    })
                    .OnTimeout(()=> { })
                    .TimeoutAfter(5.Seconds())
                    .Send();
            }

            publishCheckpoint.Complete(_repeatCount);

            _completeEvent.WaitOne(TimeSpan.FromSeconds(60), true);

            _responseEvent.WaitOne(TimeSpan.FromSeconds(60), true);

            receiveCheckpoint.Complete(_requestCounter + _responseCounter);

            stopWatch.Stop();
        }

        void Handle(GeneralMessage obj)
        {
            _bus.Publish(new SimpleResponse());

            
            if (_requestCounter == _repeatCount)
                _completeEvent.Set();
        }
    }
}