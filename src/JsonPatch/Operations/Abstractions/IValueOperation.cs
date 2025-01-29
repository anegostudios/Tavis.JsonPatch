
using System.Text.Json.Nodes;
using Tavis;

namespace JsonPatch.Operations.Abstractions
{
    public interface IValueOperation
    {
        JsonPointer Path { get; }
        JsonNode Value { get; }
    }
}