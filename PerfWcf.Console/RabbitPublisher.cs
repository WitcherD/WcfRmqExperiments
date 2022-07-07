using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;
using Serilog;

namespace PerfWcf.Console
{
    public class RabbitPublisher
    {
        private const string ExchangeIdentifier = "rabbit.mq";
        private const string RoutingKey = "rabbit_routing";

        private readonly ConnectionFactory _connectionFactory;

        private static IConnection _connection;
        private static IModel _model;

        public RabbitPublisher(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Init()
        {
            _connection = _connectionFactory.CreateConnection();
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
            //_connection.RecoverySucceeded += Connection_RecoverySucceeded;
            //_connection.ConnectionRecoveryError += Connection_ConnectionRecoveryError;
            _connection.CallbackException += Connection_CallbackException;

            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeIdentifier, ExchangeType.Direct);
            _model.QueueDeclare("BugNinjaQueue", false, false, false, null);
            _model.QueueBind("BugNinjaQueue", ExchangeIdentifier, RoutingKey, null);
        }

        public static void PublishToQueue(object message)
        {
            var binaryFormatter = new BinaryFormatter();
            try
            {
                byte[] serializedMessage;
                using (var ms = new MemoryStream())
                {
                    binaryFormatter.Serialize(ms, message);
                    serializedMessage = ms.ToArray();
                }
                
                var props = _model.CreateBasicProperties();
                props.DeliveryMode = 2;
                _model.BasicPublish(ExchangeIdentifier, RoutingKey, props, serializedMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while publishing message to RabbitMQ");
            }
        }

        private void Connection_CallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            Log.Error(e.Exception, "In Callback Exception");
        }

        private void Connection_ConnectionRecoveryError(object sender, RabbitMQ.Client.Events.ConnectionRecoveryErrorEventArgs e)
        {
            Log.Error(e.Exception , "In Connection Recovery");
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Log.Information("Rabbit MQ connection shutdown. Reply Text: " + e.ReplyText);
        }
        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            Log.Information("In Recovery Succeeded");
        }
    }
}
