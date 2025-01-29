using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using JsonPatch.Adaptors;
using JsonPatch.Operations;
using JsonPatch.Operations.Abstractions;



// ReSharper disable CheckNamespace
// ReSharper disable StringLiteralTypo

namespace Tavis
{
    public class PatchDocument
    {
        public List<Operation> Operations { get; } = new List<Operation>();

        public PatchDocument(params Operation[] operations)
        {
            foreach (var operation in operations)
            {
                AddOperation(operation);
            }
        }

        public void ApplyTo(JsonNode jToken) => ApplyTo(new JsonNetTargetAdapter(jToken));

        public void ApplyTo(IPatchTarget target)
        {
            foreach (var operation in Operations)
            {
                target.ApplyOperation(operation);
            }
        }

        public void AddOperation(Operation operation) => Operations.Add(operation);

        public static PatchDocument Load(Stream document)
        {
            using (var reader = new StreamReader(document))
            {
                return Parse(reader.ReadToEnd());
            }
        }

        public static PatchDocument Parse(string jsonDocument)
        {
            var document = new PatchDocument();
            if (!(JsonObject.Parse(jsonDocument) is JsonArray root)) return document;
            foreach (var jOperation in root)
            {
                var op = CreateOperation((string)jOperation["op"]);
                op.Read(jOperation.AsObject());
                document.AddOperation(op);
            }
            return document;
        }

        private static Operation CreateOperation(string op)
        {
            switch (op)
            {
                case "add": return new AddMergeOperation();
                case "append": return new AddMergeOperation();
                case "addeach": return new AddEachOperation();
                case "addreplace": return new AddReplaceOperation();
                case "copy": return new CopyOperation();
                case "insert": return new InsertOperation();
                case "move": return new MoveOperation();
                case "prepend": return new PrependOperation();
                case "remove": return new RemoveOperation();
                case "replace": return new ReplaceOperation();
                case "test": return new TestOperation();
            }
            return null;
        }

        /// <summary>
        ///     Create memory stream with serialized version of PatchDocument 
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            var stream = new MemoryStream();
            CopyToStream(stream);
            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        ///     Copy serialized version of Patch document to provided stream
        /// </summary>
        /// <param name="stream"></param>
        public void CopyToStream(Stream stream)
        {
            var sw = new Utf8JsonWriter(stream);
            sw.WriteStartArray();
            foreach (var operation in Operations)
            {
                operation.Write(sw);
            }
            sw.WriteEndArray();
            sw.Flush();
        }
    }
}