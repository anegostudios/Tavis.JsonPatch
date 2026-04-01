using System;
using System.Linq;
using JsonPatch.Operations;
using JsonPatch.Operations.Abstractions;
using Newtonsoft.Json.Linq;

namespace JsonPatch.Adaptors
{
    public class JsonNetTargetAdapter : BaseTargetAdapter
    {
        private readonly JToken _target;

        public JsonNetTargetAdapter(JToken target)
        {
            _target = target;
        }

        protected override void Replace(ReplaceOperation operation)
        {
            var token = operation.Path.Find(_target);
            token.Replace(operation.Value);
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
                ((JArray)token).Insert(index, operation.Value);
                return;
            }

            if (operation.Path.Last == "-")
            {
                ((JArray)token).Add(operation.Value);
                return;
            }

            switch (token[operation.Path.Last])
            {
                case JObject jObject:
                {
                    jObject.Merge(operation.Value);
                    return;
                }
                case JArray destinationJArray:
                {
                    if (!(operation.Value is JArray array)) throw new ArgumentException("Value must be a JArray");
                    foreach (var value in array) destinationJArray.Add(value);
                    return;
                }
                default:
                    token[operation.Path.Last] = operation.Value;
                    break;
            }
        }

        protected override void AddReplace(AddReplaceOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);

            if (int.TryParse(operation.Path.Last, out var index))
                ((JArray)token).Insert(index, operation.Value);
            else if (operation.Path.Last == "-")
                ((JArray)token).Add(operation.Value);
            else token[operation.Path.Last] = operation.Value;
        }

        protected override void AddEach(AddEachOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);

            var srcjarr = operation.Value as JArray;
            var dstjarr = token as JArray;

            if (srcjarr == null)
            {
                throw new ArgumentException("Value must be a JArray");
            }
            if (dstjarr == null)
            {
                throw new ArgumentException("Target must be a JArray");
            }


            if (int.TryParse(operation.Path.Last, out var index))
            {
                foreach (var value in srcjarr)
                {
                    dstjarr.Insert(index++, value);
                }
            }
            else if (operation.Path.Last == "-")
            {
                foreach (var value in srcjarr)
                {
                    dstjarr.Add(value);
                }
            }
            else
            {
                throw new ArgumentException("Path must be an index or '-'");
            }
        }



        protected override void Remove(RemoveOperation operation)
        {
            JToken token = operation.Path.Find(_target);

            if (token.Parent is JArray)
            {
                token.Remove();
            } else
            {
                token.Parent.Remove();
            }
        }

        protected override void Move(MoveOperation operation)
        {
            if (operation.FromPath.Tokens.Zip(operation.Path.Tokens, (a, b) => a.Equals(b)).Aggregate(true, (acc, v) => acc && v))
                throw new ArgumentException("To path cannot be below from path");

            var token = operation.FromPath.Find(_target);
            AddMerge(new AddMergeOperation { Path = operation.Path, Value = token });
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
            token = token.DeepClone();
            AddMerge(new AddMergeOperation { Path = operation.Path, Value = token });
        }
    }
}