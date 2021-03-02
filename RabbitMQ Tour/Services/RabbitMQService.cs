using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RabbitMQ_Tour.Pages.Index;

namespace RabbitMQ_Tour.Services
{
    public class RabbitMQService
    {
        public void SendEvent(TourSelectionModel message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");

                var routingKey = message.Book ? "tour.booked" : "tour.cancelled";

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(exchange: "topic_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);

                Debug.WriteLine(" [x] Sent '{0}'", routingKey);
            }
        }
    }
}
