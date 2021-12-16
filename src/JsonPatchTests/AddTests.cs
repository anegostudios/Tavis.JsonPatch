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
    public class AddTests
    {
        [Fact]
        public void Append_array_elements_noIndex()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/");
            patchDocument.AddOperation(new AddEachOperation()
            {
                Path = pointer,
                Value = new JArray() {
                        new JObject(new[] { new JProperty("author", "James Brown") }),
                        new JObject(new[] { new JProperty("cat", "Garfield") }),
                        new JObject(new[] { new JProperty("producer", "Kingston") }),
                    }
            });

            Assert.Throws(typeof(ArgumentException), () => {
                patchDocument.ApplyTo(sample);
            });
        }

        [Fact]
        public void Append_array_elements_wrongDataType()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/");
            patchDocument.AddOperation(new AddEachOperation()
            {
                Path = pointer,
                Value = new JObject(new[] { new JProperty("producer", "Kingston") })
            });

            Assert.Throws(typeof(ArgumentException), () => {
                patchDocument.ApplyTo(sample);
            });
        }


        [Fact]
        public void Append_array_elements()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/-");

            patchDocument.AddOperation(new AddEachOperation()
            {
                Path = pointer,
                Value = new JArray() {
                new JObject(new[] { new JProperty("author", "James Brown") }),
                new JObject(new[] { new JProperty("cat", "Garfield") }),
                new JObject(new[] { new JProperty("producer", "Kingston") }),
            }
            });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(5, list.Count);
            Assert.Equal(list[2]["author"].ToString(), "James Brown");
            Assert.Equal(list[3]["cat"].ToString(), "Garfield");
            Assert.Equal(list[4]["producer"].ToString(), "Kingston");
        }


        [Fact]
        public void Insert_array_elements()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            patchDocument.AddOperation(new AddEachOperation()
            {
                Path = pointer,
                Value = new JArray() {
                new JObject(new[] { new JProperty("author", "James Brown") }),
                new JObject(new[] { new JProperty("cat", "Garfield") }),
                new JObject(new[] { new JProperty("producer", "Kingston") }),
            }
            });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(5, list.Count);
            Assert.Equal(list[0]["author"].ToString(), "James Brown");
            Assert.Equal(list[1]["cat"].ToString(), "Garfield");
            Assert.Equal(list[2]["producer"].ToString(), "Kingston");
        }


        [Fact]
        public void Add_an_array_element()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/-");

            patchDocument.AddOperation(new AddOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal(list[2]["author"].ToString(), "James Brown");
        }



        [Fact]
        public void Insert_an_array_element()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            patchDocument.AddOperation(new AddOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal((string)sample["books"][0]["author"], "James Brown");

        }

        [Fact]
        public void Add_an_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/title");

            patchDocument.AddOperation(new AddOperation() { Path = pointer, Value = new JValue("Little Red Riding Hood") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("Little Red Riding Hood", result);

        }

        [Fact]
        public void Add_an_non_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/ISBN");

            patchDocument.AddOperation(new AddOperation() { Path = pointer, Value = new JValue("213324234343") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("213324234343", result);

        }

    }
}
