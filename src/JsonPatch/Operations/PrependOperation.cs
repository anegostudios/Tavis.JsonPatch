using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPatch.Operations.Abstractions;


using Tavis;

namespace JsonPatch.Operations
{
    public class PrependOperation : Operation, IValueOperation
    {
        public JsonNode Value { get; set; }

        public void FixPath()
        {
            var lastToken = Path.Last;
            if (int.TryParse(lastToken, out _) || lastToken == "-") Path.Tokens[Path.Tokens.Count - 1] = "0";
        }

        public override void Write(Utf8JsonWriter writer)
        {
            FixPath();
            writer.WriteStartObject();

            WriteOp(writer, "prepend");
            WritePath(writer, Path);
            WriteValue(writer, Value);

            writer.WriteEndObject();
        }

        public override void Read(JsonObject jOperation)
        {
            Path = new JsonPointer(jOperation["path"].GetValue<string>());
            Value = jOperation["value"].AsValue();
        }
    }
}