using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPatch.Operations.Abstractions;


using Tavis;

namespace JsonPatch.Operations
{
    public class AddMergeOperation : Operation, IValueOperation
    {
        public JsonNode Value { get; set; }

        public override void Write(Utf8JsonWriter writer)
        {
            if (int.TryParse(Path.Tokens.Last(), out _)) Path.Tokens[Path.Tokens.Count - 1] = "-";
            writer.WriteStartObject();

            WriteOp(writer, "add");
            WritePath(writer, Path);
            WriteValue(writer, Value);

            writer.WriteEndObject();
        }

        public override void Read(JsonObject jOperation)
        {
            Path = new JsonPointer(jOperation["path"].GetValue<string>());
            Value = jOperation["value"];
        }
    }
}