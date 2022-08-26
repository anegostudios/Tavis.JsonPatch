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
    public class AddEachTests
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



    }
}
