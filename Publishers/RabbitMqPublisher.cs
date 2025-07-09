using System.Text;
using RabbitMQ.Client;

namespace LoanBack.Publishers;

public class RabbitMqPublisher : IRabbitMqPublisher
{

    public void PublishLoanRequest(int loanId)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "loan_requests",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = Encoding.UTF8.GetBytes(loanId.ToString());

        channel.BasicPublish(exchange: "",
                             routingKey: "loan_requests",
                             basicProperties: null,
                             body: body);
    }
}

