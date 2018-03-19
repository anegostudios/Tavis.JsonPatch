using System;
using System.Linq;
using Newtonsoft.Json.Linq;

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
            var parentPointer = operation.Path.ParentPointer;
            var token = parentPointer.Find(_target);
            // FIXME: Probably fails if path points to root element, which *should* replace the entire document with the specified value.

            int index;
            if (int.TryParse(operation.Path.Last, out index))
            {
                ((JArray)token).Insert(index, operation.Value);
            }
            else if (operation.Path.Last == "-")
            {
                ((JArray)token).Add(operation.Value);
            }
            else
            {
                ((JObject)token)[operation.Path.Last] = operation.Value;
            }
        }


        protected override void Remove(RemoveOperation operation)
        {
            var token = operation.Path.Find(_target);
            if (token.Parent is JProperty)
            {
                token.Parent.Remove();
            }
            else
            {
                token.Remove();
            }
        }

        protected override void Move(MoveOperation operation)
        {
            if (operation.Path.ToString().StartsWith(operation.FromPath.ToString())) throw new ArgumentException("To path cannot be below from path");

            var token = operation.FromPath.Find(_target);
            Remove(new RemoveOperation(){Path = operation.FromPath});
            Add(new AddOperation() {Path = operation.Path, Value = token});
        }

        protected override void Test(TestOperation operation)
        {
            var existingValue = operation.Path.Find(_target);
            if (!existingValue.Equals(_target))
            {
                throw new InvalidOperationException("Value at " + operation.Path.ToString() + " does not match.");
            }
        }

        protected override void Copy(CopyOperation operation)
        {
            var token = operation.FromPath.Find(_target);  // Do I need to clone this?
            Add(new AddOperation() {Path = operation.Path, Value = token});
        }
    }
}