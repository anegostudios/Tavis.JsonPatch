using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPatch.Operations.Abstractions;


using Tavis;

namespace JsonPatch.Operations
{
    public class MoveOperation : Operation
    {
        public JsonPointer FromPath { get; set; }

        public override void Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteOp(writer, "move");
            WritePath(writer, Path);
            WriteFromPath(writer, FromPath);

            writer.WriteEndObject();
        }

        public override void Read(JsonObject jOperation)
        {
            Path = new JsonPointer(jOperation["path"].GetValue<string>());
            FromPath = new JsonPointer(jOperation["from"].GetValue<string>());
        }
    }
}