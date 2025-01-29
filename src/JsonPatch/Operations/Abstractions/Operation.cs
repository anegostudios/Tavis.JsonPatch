

using System.Text.Json;
using System.Text.Json.Nodes;
using Tavis;

namespace JsonPatch.Operations.Abstractions
{
    public abstract class Operation
    {
        public JsonPointer Path { get; set; }

        public abstract void Write(Utf8JsonWriter writer);

        protected static void WriteOp(Utf8JsonWriter writer, string op)
        {
            writer.WritePropertyName("op");
            writer.WriteStringValue(op);
        }

        protected static void WritePath(Utf8JsonWriter writer, JsonPointer pointer)
        {
            writer.WritePropertyName("path");
            writer.WriteStringValue(pointer.ToString());
        }

        protected static void WriteFromPath(Utf8JsonWriter writer, JsonPointer pointer)
        {
            writer.WritePropertyName("from");
            writer.WriteStringValue(pointer.ToString());
        }
        protected static void WriteValue(Utf8JsonWriter writer, JsonNode value)
        {
            writer.WritePropertyName("value");
            value.WriteTo(writer);
        }

        public abstract void Read(JsonObject jOperation);
    }
}