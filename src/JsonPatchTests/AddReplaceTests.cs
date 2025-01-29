using System.Text.Json.Nodes;
using JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class AddReplaceTests
    {


        [Fact]
        public void AddReplace_an_array_element()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/-");

            var value = new JsonObject();
            value["author"] = "James Brown";
            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = value });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JsonArray;

            Assert.Equal(3, list.Count);
            Assert.Equal(list[2]["author"].ToString(), "James Brown");
        }



        [Fact]
        public void InsertReplace_an_array_element()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            var value = new JsonObject();
            value["author"] = "James Brown";
            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = value });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JsonArray;

            Assert.Equal(3, list.Count);
            Assert.Equal((string)sample["books"][0]["author"], "James Brown");

        }

        [Fact]
        public void AddReplace_an_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/title");

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = JsonValue.Create("Little Red Riding Hood") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("Little Red Riding Hood", result);

        }

        [Fact]
        public void AddReplace_an_non_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/ISBN");

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = JsonValue.Create("213324234343") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("213324234343", result);

        }


        [Fact]
        public void AddReplace_an_non_existing_object_replace()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/attributes");

            patchDocument.AddOperation(new AddReplaceOperation { Path = pointer, Value = JsonNode.Parse("{ \"age\": 15 }") });
            patchDocument.AddOperation(new AddReplaceOperation { Path = pointer, Value = JsonNode.Parse("{ \"pages\": 200 }") });
            patchDocument.ApplyTo(sample);

            var pointerAge = new JsonPointer("/books/0/attributes/age");
            var pointerPages = new JsonPointer("/books/0/attributes/pages");

            Assert.Throws<PathNotFoundException>(() => { pointerAge.Find(sample); });

            var result = (int)pointerPages.Find(sample);
            Assert.Equal(200, result);

        }

    }
}
