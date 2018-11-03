using System;
using System.Collections.Generic;

namespace greenergy.chatbot_fulfillment.Models
{
    public class DialogFlowResponseDTO
    {
        public string fulfillmentText { get; set; }
        public List<FulfillmentMessage> fulfillmentMessages { get; set; }
        public string source { get; set; }
        public Payload payload { get; set; }
        public List<OutputContext> outputContexts { get; set; }
        public OutputContext followupEventInput { get; set; }
    }
    public class DialogFlowRequestDTO
    {
        public string responseId { get; set; }
        public string session { get; set; }
        public QueryResult queryResult { get; set; }
        public OriginalDetectIntentRequest originalDetectIntentRequest { get; set; }

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

    public class Button
    {
        public string text { get; set; }
        public string postback { get; set; }
    }

    public class Card
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string imageUri { get; set; }
        public List<Button> buttons { get; set; }
    }

    public class FulfillmentMessage
    {
    }

    public class CardFulfillmentMessage : FulfillmentMessage
    {
        public Card card { get; set; }
    }

    public class TextFulfillmentMessage : FulfillmentMessage
    {
        public Text text { get; set; }
    }

    public class Text
    {
        public List<string> text { get; set; }
    }

    public class SimpleResponse
    {
        public string textToSpeech { get; set; }
    }

    public class Item
    {
        public SimpleResponse simpleResponse { get; set; }
    }

    public class RichResponse
    {
        public List<Item> items { get; set; }
    }

    public class Google
    {
        public bool expectUserResponse { get; set; }
        public RichResponse richResponse { get; set; }
    }

    public class Facebook
    {
        public string text { get; set; }
    }

    public class Slack
    {
        public string text { get; set; }
    }

    public class Payload
    {
        public Google google { get; set; }
        public Facebook facebook { get; set; }
        public Slack slack { get; set; }
    }

    public class Parameters
    {
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public float kilometers { get; set; }
        public Duration duration { get; set; }
    }

    public class Duration
    {
        public float amount { get; set; }
        public string unit { get; set; }

        public int toMinutes()
        {
            if (unit.Equals("min"))
                return (int) amount;
            else if (unit.Equals("h"))
                return (int) amount*60;
            else if (unit.Equals("day"))
                return (int) amount*60*24;
            return -1;
        }
    }

    public class OutputContext
    {
        public string name { get; set; }
        public int lifespanCount { get; set; }
        public Parameters parameters { get; set; }
    }

    public class Intent
    {
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class DiagnosticInfo
    {
    }
}