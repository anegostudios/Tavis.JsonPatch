using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonPatch.Operations;
using JsonPatch.Operations.Abstractions;
using Tavis;


namespace JsonPatch.Adaptors
{
    public class JsonNetTargetAdapter : BaseTargetAdapter
    {
        private readonly JsonNode _target;

        public JsonNetTargetAdapter(JsonNode target)
        {
            _target = target;
        }

        protected override void Replace(ReplaceOperation operation)
        {
            var token = operation.Path.Find(_target);
            token.Parent[operation.Path.Last] = operation.Value;
        }

        protected override void AddMerge(AddMergeOperation mergeOperation)
        {
            AddInsertPrepend(mergeOperation);
        }

        protected override void Insert(InsertOperation operation)
        {
            AddInsertPrepend(operation);
        }

        protected override void Prepend(PrependOperation operation)
        {
            operation.FixPath();
            AddInsertPrepend(operation);
        }

        private void AddInsertPrepend(IValueOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);
            if (int.TryParse(operation.Path.Last, out var index))
            {
                token.AsArray().Insert(index, operation.Value);
                return;
            }

            if (operation.Path.Last == "-")
            {
                token.AsArray().Add(operation.Value);
                return;
            }

            switch (token[operation.Path.Last])
            {
                case JsonObject jObject:
                {
                    foreach (var (key, value) in operation.Value.AsObject())
                    {
                        jObject.Add(key, JsonSerializer.SerializeToNode(value));
                    }

                    return;
                }
                case JsonArray destinationJsonArray:
                {
                    if (operation.Value is not JsonArray array) throw new ArgumentException("Value must be a JsonArray");
                    foreach (var value in array) destinationJsonArray.Add(value);
                    return;
                }
                default:
                    token[operation.Path.Last] = JsonSerializer.SerializeToNode(operation.Value);
                    break;
            }
        }

        protected override void AddReplace(AddReplaceOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);

            if (int.TryParse(operation.Path.Last, out var index))
                token.AsArray().Insert(index, operation.Value);
            else if (operation.Path.Last == "-")
                token.AsArray().Add(operation.Value);
            else token[operation.Path.Last] = operation.Value;
        }

        protected override void AddEach(AddEachOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);

            var srcjarr = operation.Value as JsonArray;
            var dstjarr = token.AsArray();

            if (srcjarr == null)
            {
                throw new ArgumentException("Value must be a JsonArray");
            }
            if (dstjarr == null)
            {
                throw new ArgumentException("Target must be a JsonArray");
            }


            if (int.TryParse(operation.Path.Last, out var index))
            {
                foreach (var value in srcjarr)
                {
                    dstjarr.Insert(index++, JsonSerializer.SerializeToNode(value));
                }
            }
            else if (operation.Path.Last == "-")
            {
                foreach (var value in srcjarr)
                {
                    dstjarr.Add(JsonSerializer.SerializeToNode(value));
                }
            }
            else
            {
                throw new ArgumentException("Path must be an index or '-'");
            }
        }



        protected override void Remove(RemoveOperation operation)
        {
            var token = operation.Path.Find(_target);

            if (token.Parent is JsonArray ja)
            {
                ja.Remove(token);
            } else
            {
                // TODO
                token.Parent.AsObject().Remove(token.GetPath());
            }
        }

        protected override void Move(MoveOperation operation)
        {
            if (operation.Path.ToString().StartsWith(operation.FromPath.ToString()))
                throw new ArgumentException("To path cannot be below from path");

            var token = operation.FromPath.Find(_target);
            AddMerge(new AddMergeOperation { Path = operation.Path, Value = JsonSerializer.SerializeToNode(token) });
            Remove(new RemoveOperation { Path = operation.FromPath });
        }

        protected override void Test(TestOperation operation)
        {
            var existingValue = operation.Path.Find(_target);
            if (!existingValue.Equals(_target)) throw new InvalidOperationException(
                $"Value at { operation.Path } does not match.");
        }

        protected override void Copy(CopyOperation operation)
        {
            var token = operation.FromPath.Find(_target);
            var copyValue = JsonSerializer.SerializeToNode(token);
            AddMerge(new AddMergeOperation { Path = operation.Path, Value = copyValue });
        }
    }
}