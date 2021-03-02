using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Email_Service
{

    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                var queueName = channel.QueueDeclare().QueueName;


                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: "tour.booked");

                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    var message = JsonConvert.DeserializeObject<TourSelectionModel>(Encoding.UTF8.GetString(body));

                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey,
                                      message.Name);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
    public class TourSelectionModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Tour { get; set; }
        public bool Book { get; set; }
    }
}
