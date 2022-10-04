using JsonPatch.Operations;
using JsonPatch.Operations.Abstractions;

namespace JsonPatch.Adaptors
{
    public abstract class BaseTargetAdapter : IPatchTarget
    {
        public void ApplyOperation(Operation operation)
        {
            switch (operation)
            {
                case AddReplaceOperation replaceOperation:
                    AddReplace(replaceOperation);
                    break;
                case AddOperation addOperation:
                    Add(addOperation);
                    break;
                case AddEachOperation eachOperation:
                    AddEach(eachOperation);
                    break;
                case CopyOperation copyOperation:
                    Copy(copyOperation);
                    break;
                case MoveOperation moveOperation:
                    Move(moveOperation);
                    break;
                case RemoveOperation removeOperation:
                    Remove(removeOperation);
                    break;
                case ReplaceOperation replaceOperation:
                    Replace(replaceOperation);
                    break;
                case TestOperation testOperation:
                    Test(testOperation);
                    break;
                case InsertOperation insertOperation:
                    Insert(insertOperation);
                    break;
                case PrependOperation prependOperation:
                    Prepend(prependOperation);
                    break;
            }
        }

        protected abstract void Add(AddOperation operation);
        protected abstract void AddReplace(AddReplaceOperation operation);
        protected abstract void AddEach(AddEachOperation operation);
        protected abstract void Copy(CopyOperation operation);
        protected abstract void Insert(InsertOperation operation);
        protected abstract void Move(MoveOperation operation);
        protected abstract void Prepend(PrependOperation operation);
        protected abstract void Remove(RemoveOperation operation);
        protected abstract void Replace(ReplaceOperation operation);
        protected abstract void Test(TestOperation operation);
    }
}