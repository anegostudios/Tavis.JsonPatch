using System.Linq;
using JsonPatch.Operations.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;

namespace JsonPatch.Operations
{
    public class AddMergeOperation : Operation, IValueOperation
    {
        public JToken Value { get; set; }

        public override void Write(JsonWriter writer)
        {
            if (int.TryParse(Path.Tokens.Last(), out _)) Path.Tokens[Path.Tokens.Count - 1] = "-";
            writer.WriteStartObject();

            WriteOp(writer, "add");
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