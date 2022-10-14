using JsonPatch.Operations.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;

namespace JsonPatch.Operations
{
    public class PrependOperation : Operation, IValueOperation
    {
        public JToken Value { get; set; }

        public void FixPath()
        {
            var lastToken = Path.Last;
            if (int.TryParse(lastToken, out _) || lastToken == "-") Path.Tokens[Path.Tokens.Count - 1] = "0";
        }

        public override void Write(JsonWriter writer)
        {
            FixPath();
            writer.WriteStartObject();

            WriteOp(writer, "prepend");
            WritePath(writer, Path);
            WriteValue(writer, Value);

            writer.WriteEndObject();
        }

        public override void Read(JObject jOperation)
        {
            Path = new JsonPointer((string)jOperation.GetValue("path"));
            Value = jOperation.GetValue("value");
        }
    }
}