using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPatch.Operations.Abstractions;


using Tavis;

namespace JsonPatch.Operations
{
    public class RemoveOperation : Operation
    {
        
        public override void Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteOp(writer, "remove");
            WritePath(writer, Path);

            writer.WriteEndObject();
        }

        public override void Read(JsonObject jOperation)
        {
            Path = new JsonPointer(jOperation["path"].GetValue<string>());
        }
    }
}