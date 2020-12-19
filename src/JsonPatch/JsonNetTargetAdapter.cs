using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tavis
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

        protected override void Add(AddOperation operation)
        {
            var token = operation.Path.Find(_target, skipLast: true);

            int index;
            if (int.TryParse(operation.Path.Last, out index))
                ((JArray)token).Insert(index, operation.Value);
            else if (operation.Path.Last == "-")
                ((JArray)token).Add(operation.Value);
            else token[operation.Path.Last] = operation.Value;
        }


        protected override void Remove(RemoveOperation operation)
        {
            JToken token = operation.Path.Find(_target);

            if (token is JValue)
            {
                token.Parent.Remove();
            } else
            {
                token.Remove();
            }
        }

        protected override void Move(MoveOperation operation)
        {
            if (operation.Path.ToString().StartsWith(operation.FromPath.ToString()))
                throw new ArgumentException("To path cannot be below from path");

            var token = operation.FromPath.Find(_target);
            Add(new AddOperation { Path = operation.Path, Value = token });
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
            Add(new AddOperation { Path = operation.Path, Value = token });
        }
    }
}