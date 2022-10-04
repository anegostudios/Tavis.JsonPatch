using Newtonsoft.Json.Linq;
using Tavis;

namespace JsonPatch.Operations.Abstractions
{
    public interface IValueOperation
    {
        JsonPointer Path { get; }
        JToken Value { get; }
    }
}