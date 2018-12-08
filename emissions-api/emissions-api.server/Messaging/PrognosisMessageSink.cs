using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Greenergy.Emissions.API.Client.Models;
using Greenergy.Emissions.Optimization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Greenergy.Emissions.Messaging
{
    public class PrognosisMessageSink : IPrognosisMessageSink, IDisposable
    {
        private ILogger<PrognosisMessageSink> _logger;
        private Producer<string, string> _producer;

        public PrognosisMessageSink(ILogger<PrognosisMessageSink> logger)
        {
            _logger = logger;
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "green-kafka:9092" }
            };
            _producer = new Producer<string, string>(config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
        }

        public void Dispose()
        {
            _producer.Flush(100);
            _producer.Dispose();
        }

        public async Task SendPrognoses(List<RegionalConsumptionPrognoses> regionalInfos)
        {
            try
            {
                foreach (var r in regionalInfos)
                {
                    await _producer.ProduceAsync("future-consumption", r.Region, PrettyJson(r.Prognoses));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private string PrettyJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}