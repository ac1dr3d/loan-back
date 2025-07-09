using RabbitMQ.Client;
using System.Text;

namespace LoanBack.Consumers;

public class LoanQueueConsumer : ILoanQueueConsumer
{
    private readonly ConnectionFactory _factory;

    public LoanQueueConsumer(IConfiguration config)
    {
        _factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            UserName = config["RabbitMQ:User"] ?? "guest",
            Password = config["RabbitMQ:Pass"] ?? "guest"
        };
    }

    public async Task<List<int>> GetLoanIdsFromQueueAsync()
    {
        var loanIds = new List<int>();

        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "loan_requests",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        BasicGetResult result;
        while ((result = channel.BasicGet("loan_requests", autoAck: false)) != null)
        {
            var body = result.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);

            if (int.TryParse(message, out int loanId))
            {
                loanIds.Add(loanId);
                channel.BasicAck(result.DeliveryTag, false);
            }
            else
            {
                channel.BasicReject(result.DeliveryTag, false); // discard bad message
            }
        }

        return loanIds;
    }
}

