using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

string QueueContainerHostname = "rabbitmq";
var factory = new ConnectionFactory { HostName = QueueContainerHostname };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclarePassive(queue: "user_registration");

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var emailMessage = JsonSerializer.Deserialize<EmailMessage>(message);

    await Emailer.SendEmail(emailMessage);
    Console.WriteLine($" [x] Received message.\n     To: {emailMessage.Email}\n     Subject: {emailMessage.Subject}\n     Body: {emailMessage.Body}");
};

channel.BasicConsume(queue: "user_registration",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

await Task.Run(() => Thread.Sleep(Timeout.Infinite));

