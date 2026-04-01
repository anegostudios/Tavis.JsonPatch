using JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class MoveTests
    {

        [Fact]
        public void Move_property()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var frompointer = new JsonPointer("/books/0/author");
            var topointer = new JsonPointer("/books/1/author");

            patchDocument.AddOperation(new MoveOperation() { FromPath = frompointer, Path = topointer });

            patchDocument.ApplyTo(sample);


            var result = (string)topointer.Find(sample);
            Assert.Equal("F. Scott Fitzgerald", result);
        }

        [Fact]
        public void Move_array_element()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var frompointer = new JsonPointer("/books/1");
            var topointer = new JsonPointer("/books/0/child");

            patchDocument.AddOperation(new MoveOperation() { FromPath = frompointer, Path = topointer });

            patchDocument.ApplyTo(sample);


            var result = topointer.Find(sample);
            Assert.IsType(typeof(JObject), result);
        }

        [Fact]
        public void Move_similar_named_paths()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var frompointer = new JsonPointer("/books");
            var topointer = new JsonPointer("/bookshelf");

            patchDocument.AddOperation(new MoveOperation() { FromPath = frompointer, Path = topointer });

            patchDocument.ApplyTo(sample);

            var result = topointer.Find(sample);
            Assert.IsType<JArray>(result);
        }
    }
}
