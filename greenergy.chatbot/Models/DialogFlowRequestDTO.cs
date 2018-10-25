using System;
using System.Collections.Generic;

namespace greenergy.chatbot_fulfillment.RequestModels
{
    public class DialogFlowRequestDTO
    {
        public string responseId { get; set; }
        public string session { get; set; }
        public QueryResult queryResult { get; set; }
        public OriginalDetectIntentRequest originalDetectIntentRequest { get; set; }

        public class Parameters
        {

            public DateTime Time { get; set; }
            public DateTime Date { get; set; }
            public float Kilometers { get; set; }
            public string doCharge { get; set; }
        }

        public class Text
        {
            public List<string> text { get; set; }
        }

        public class FulfillmentMessage
        {
            public Text text { get; set; }
        }

        public class Parameters2
        {
            public string param { get; set; }
        }

        public class OutputContext
        {
            public string name { get; set; }
            public int lifespanCount { get; set; }
            public Parameters2 parameters { get; set; }
        }

        public class Intent
        {
            public string name { get; set; }
            public string displayName { get; set; }
        }

        public class DiagnosticInfo
        {
        }

        public class QueryResult
        {
            public string queryText { get; set; }
            public Parameters parameters { get; set; }
            public bool allRequiredParamsPresent { get; set; }
            public string fulfillmentText { get; set; }
            public List<FulfillmentMessage> fulfillmentMessages { get; set; }
            public List<OutputContext> outputContexts { get; set; }
            public Intent intent { get; set; }
            public float intentDetectionConfidence { get; set; }
            public DiagnosticInfo diagnosticInfo { get; set; }
            public string languageCode { get; set; }
        }

        public class OriginalDetectIntentRequest
        {
        }

    }
}