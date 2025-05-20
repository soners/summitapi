using System.Text;
using AWS.Messaging;
using Confluent.Kafka;

namespace SummitWorker.Handler;

public class PairMessageHandler(IConfiguration configuration) : IMessageHandler<PairMessage>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<PairMessage> messageEnvelope,
        CancellationToken cancellationToken)
    {
        var pairCode = Convert.ToHexString(Encoding.UTF8.GetBytes(new Random().Next().ToString())[..3]);

        var config = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString("kafka"),
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        await producer.ProduceAsync(topic: "pair-processed", new Message<Null, string>
        {
            Value = pairCode
        }, cancellationToken);

        return MessageProcessStatus.Success();
    }
}