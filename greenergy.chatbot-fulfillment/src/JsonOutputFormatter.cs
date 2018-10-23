using System.Buffers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace greenergy.chatbot_fulfillment.OutputFormatters
{
    // Forces json output encoding to use UTF-8 to fix interoperability problem with DialogFlow.
    // Also removes null elements from serialized json documents
    public class DialogFlowJsonOutputFormatter : JsonOutputFormatter
    {
        public DialogFlowJsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool) 
            : base(serializerSettings, charPool)
        {
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public override Encoding SelectCharacterEncoding(OutputFormatterWriteContext context)
        {
            return Encoding.UTF8;
        }
    }
}