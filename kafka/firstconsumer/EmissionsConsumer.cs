using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Confluent.Kafka;

namespace Greenergy.Messaging
{
    public class EmissionsConsumer : IEmissionsConsumer
    {
        public void Consume(CancellationToken cts)
        {
            var config = new ConsumerConfig
            {
                GroupId = "teslacharger",
                BootstrapServers = "green-kafka:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };

            using (var c = new Consumer<string, string>(config))
            {
                c.Subscribe("future-consumption");
                // c.OnError += (_, msg) =>
                // {
                //     message(msg.Value);
                // };

                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        var cr = c.Consume(cts);
                        Console.WriteLine($"Consumed message from '{cr.Topic}', partion {cr.Partition}, offset {cr.Offset}, length {cr.Value.Length}, head {cr.Value.Substring(0, 30)}");
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}