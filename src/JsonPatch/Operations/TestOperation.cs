using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPatch.Operations.Abstractions;


using Tavis;

namespace JsonPatch.Operations
{
    public class TestOperation : Operation, IValueOperation
    {
        public JsonNode Value { get; set; }

        public override void Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteOp(writer, "test");
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