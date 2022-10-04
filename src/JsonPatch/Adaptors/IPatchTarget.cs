using JsonPatch.Operations.Abstractions;

namespace JsonPatch.Adaptors
{
    public interface IPatchTarget
    {
        void ApplyOperation(Operation operation);
    }
}
