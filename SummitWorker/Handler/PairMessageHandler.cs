using System.Text;
using System.Text.Json;
using AWS.Messaging;
using Confluent.Kafka;

namespace SummitWorker.Handler;

public class PairMessageHandler(IConfiguration configuration) : IMessageHandler<PairMessage>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<PairMessage> messageEnvelope,
        CancellationToken cancellationToken)
    {
        var pairMessage = messageEnvelope.Message;
        var pairCode = Convert.ToHexString(Encoding.UTF8.GetBytes(new Random().Next().ToString())[..3]);

        var config = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString("kafka"),
        };

        var pairRequestedMessage = new PairRequestedMessage(pairMessage.Teacher, pairMessage.Student, pairCode);
        using var producer = new ProducerBuilder<Null, string>(config).Build();

        await producer.ProduceAsync(topic: "pair-request-processed", new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(pairRequestedMessage)
        }, cancellationToken);

        return MessageProcessStatus.Success();
    }
}