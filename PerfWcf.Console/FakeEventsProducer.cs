using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace PerfWcf.Console
{
    public class FakeEventsProducer
    {
        public static void StartPublishingRmqEvents(CancellationTokenSource cancellationTokenSource)
        {
            Task.Factory.StartNew(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var message = new FakeRmqMessage { Guid = Guid.NewGuid() };
                    RabbitPublisher.PublishToQueue(message);
                    Log.Information($"[{Thread.CurrentThread.ManagedThreadId}] RMQ Message {message.Guid} published");
                    Thread.Sleep(100);
                }
            }, cancellationTokenSource.Token);
        }

        public static void StartCallingWcfEndpoints(CancellationTokenSource cancellationTokenSource)
        {
            Task.Factory.StartNew(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    Log.Information($"[{Thread.CurrentThread.ManagedThreadId}] Trigger a StateEvent");
                    StateService.SpawnDirectSession("123", "123", 1, false, false, false, 0);
                    Thread.Sleep(5000);
                }
            }, cancellationTokenSource.Token);

            Task.Factory.StartNew(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    Log.Information($"[{Thread.CurrentThread.ManagedThreadId}] Trigger a StateEvent");
                    StateService.SpawnDirectSession("123", "123", 1, false, false, false, 0);
                    Thread.Sleep(1000);
                }
            }, cancellationTokenSource.Token);
        }
    }
}
