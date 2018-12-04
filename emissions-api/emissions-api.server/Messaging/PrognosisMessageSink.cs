using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Greenergy.Emissions.Messaging
{
    public class PrognosisMessageSink : IPrognosisMessageSink
    {
        private ILogger<PrognosisMessageSink> _logger;

        public PrognosisMessageSink(ILogger<PrognosisMessageSink> logger)
        {
            _logger = logger;
        }

        public async Task SendPrognoses(List<List<OptimalConsumptionPrognosis>> regionalInfos)
        {
            _logger.LogInformation(PrettyJson(regionalInfos));
        }

        private string PrettyJson(object obj)
        {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }
    }
}