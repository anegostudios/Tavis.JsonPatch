using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal(list[2]["author"].ToString(), "James Brown");
        }



        [Fact]
        public void InsertReplace_an_array_element()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal((string)sample["books"][0]["author"], "James Brown");

        }

        [Fact]
        public void AddReplace_an_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/title");

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = new JValue("Little Red Riding Hood") });

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

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = new JValue("213324234343") });

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

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = JToken.Parse("{ \"age\": 15 }") });
            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = JToken.Parse("{ \"pages\": 200 }") });
            patchDocument.ApplyTo(sample);

            var pointerAge = new JsonPointer("/books/0/attributes/age");
            var pointerPages = new JsonPointer("/books/0/attributes/pages");

            Assert.Throws(typeof(PathNotFoundException), () => { pointerAge.Find(sample); });

            var result = (string)pointerPages.Find(sample);
            Assert.Equal("200", result);

        }

    }
}
