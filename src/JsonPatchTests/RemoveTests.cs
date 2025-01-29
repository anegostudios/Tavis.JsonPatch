using System.Text.Json.Nodes;
using JsonPatch.Operations;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class RemoveTests
    {
        [Fact]
        public void Remove_a_property()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/author");

            patchDocument.AddOperation(new RemoveOperation() { Path = pointer });

            patchDocument.ApplyTo(sample);

            //Assert.Throws<PathNotFoundException>(() => { pointer.Find(sample); });

            Assert.Equal(sample["books"][0]["title"].ToString(), "The Great Gatsby");
        }

        [Fact]
        public void Remove_an_object()
        {

            var sample = PatchTests.GetSample3();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/textures");

            patchDocument.AddOperation(new RemoveOperation() { Path = pointer });

            patchDocument.ApplyTo(sample);

//            Assert.Throws<PathNotFoundException>(() => { pointer.Find(sample); });

            Assert.Equal(sample["lightAbsorption"].ToString(), "99");
        }


        [Fact]
        public void Remove_an_array_element()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            patchDocument.AddOperation(new RemoveOperation() { Path = pointer });

            patchDocument.ApplyTo(sample);

            // Assert.Throws<PathNotFoundException>(() =>
            // {
            //     var x = pointer.Find(JsonValue.Create("/books/1").AsObject());
            // });

            Assert.Equal(sample["books"][0]["title"].ToString(), "The Grapes of Wrath");

        }
    }
}
