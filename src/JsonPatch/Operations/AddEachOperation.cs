using JsonPatch.Operations.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;

namespace JsonPatch.Operations
{
    public class AddEachOperation : Operation, IValueOperation
    {
        public JToken Value { get; set; }

        public override void Write(JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteOp(writer, "addeach");
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