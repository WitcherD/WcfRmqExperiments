using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace PerfWcf.Console
{
    public class RabbitConsumer
    {
        private const string ExchangeIdentifier = "rabbit.mq";
        private const string RoutingKey = "rabbit_routing";

        private readonly ConnectionFactory _connectionFactory;

        private static IConnection _singletonRabbitConnection;
        private static IModel _model;

        public RabbitConsumer(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void StartConsuming()
        {
            try
            {
                _singletonRabbitConnection = _connectionFactory.CreateConnection();
                _singletonRabbitConnection.ConnectionShutdown += Connection_ConnectionShutdown;
                //_singletonRabbitConnection.RecoverySucceeded += Connection_RecoverySucceeded;
                //_singletonRabbitConnection.ConnectionRecoveryError += Connection_ConnectionRecoveryError;
                _singletonRabbitConnection.CallbackException += Connection_CallbackException;
                
                _model = _singletonRabbitConnection.CreateModel();
                _model.ModelShutdown += Model_ModelShutdown;
                _model.CallbackException += Model_CallbackException;
                
                _model.ExchangeDeclare(ExchangeIdentifier, ExchangeType.Direct);
                _model.QueueDeclare("BugNinjaQueue", false, false, false, null);
                _model.QueueBind("BugNinjaQueue", ExchangeIdentifier, RoutingKey, null);
                
                //_model.BasicQos(0, 0, false);
                _model.BasicQos(0, 20, false);

                var consumer = new EventingBasicConsumer(_model);
                consumer.Received += Consumer_Received;
                consumer.Shutdown += Consumer_Shutdown;
                consumer.Unregistered += Consumer_Unregistered;
                consumer.Registered += Consumer_Registered;
                _model.BasicConsume("BugNinjaQueue", false, consumer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while starting the consumer for RabbitMQ: ");
            }
        }
        void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(e.Body.ToArray()))
                {
                    var temp = (FakeRmqMessage)bf.Deserialize(ms);
                    Log.Information($"[{Thread.CurrentThread.ManagedThreadId}] RMQ Message {temp.Guid} received.");
                }

                Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while consuming message: ");
            }

            _model.BasicAck(e.DeliveryTag, false); //Acknowledge. This is not really required.
        }

        private void Consumer_Registered(object sender, ConsumerEventArgs e)
        {
            Log.Information("In Consumer Regestered");
        }

        private void Consumer_Unregistered(object sender, ConsumerEventArgs e)
        {
            Log.Information("In Consumer Unregestered");
        }

        void Consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
            Log.Information("Consumer shut-down");
        }

        private void Model_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            Log.Error(e.Exception, "In Model Callback Exception ");
        }

        private void Model_ModelShutdown(object sender, ShutdownEventArgs e)
        {
            Log.Information("In Model Shutdown");
        }

        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            Log.Error(e.Exception, "In Callback Exception Detail");
        }

        private void Connection_ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            Log.Error(e.Exception, "In Recovery Error");
        }

        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            Log.Information("In Recovery Succeeded");
        }

        void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Log.Information("RabbitMQ consumer connection shutdown");
        }
    }
}
