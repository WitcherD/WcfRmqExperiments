using System.Threading;
using RabbitMQ.Client;
using Serilog;

namespace PerfWcf.Console
{
    public class GlobalStateService
    {
        private RabbitConsumer _consumer;
        private RabbitPublisher _publisher;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public GlobalStateService()
        {
            StateService.State_DirectSessionRequested += StateServiceOnState_DirectSessionRequested;
        }

        public void Start()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "",
                Port = 5672,
                UserName = "",
                Password = "",
                VirtualHost = "",
                UseBackgroundThreadsForIO = true,
                ConsumerDispatchConcurrency = 10
            };

            _publisher = new RabbitPublisher(connectionFactory);
            _publisher.Init();

            _consumer = new RabbitConsumer(connectionFactory);
            _consumer.StartConsuming();

            FakeEventsProducer.StartPublishingRmqEvents(_cancellationTokenSource);
            FakeEventsProducer.StartCallingWcfEndpoints(_cancellationTokenSource);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void StateServiceOnState_DirectSessionRequested(object sender, StateSessionEventArgs e)
        {
            Log.Information($"[{Thread.CurrentThread.ManagedThreadId}] StateEvent was invoked");
            Thread.Sleep(1000);
        }
    }
}
