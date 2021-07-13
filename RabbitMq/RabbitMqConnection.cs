using RabbitMQ.Client;
using System;

namespace RabbitMq
{
    public class RabbitMqConnection
    {
        public IConnection connection;

        public RabbitMqConnection()
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqp://guest:guest@localhost:5672/test");

            if (connection == null) connection = factory.CreateConnection();
        }
        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqp://guest:guest@localhost:5672/test");

            if(connection == null) return factory.CreateConnection();
            return connection;
        }

        public void CloseConnection()
        {
            if (connection == null) return;

            connection.Dispose();
        }
    }
}
