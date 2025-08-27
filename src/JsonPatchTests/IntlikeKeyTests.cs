using JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class IntlikeKeyTests
    {
        public static JToken GetSample()
        {
            return JToken.Parse(@"{
                '12': 'one dozen',
                '3': [ 'three', 'blind', 'mice' ],
                'foo': { '1234': 'bar', '0xdeadbeef': 3735928559, '0o0': 'spooky' }
            }");
        }

        [Fact]
        public void GetValue_Root()
        {
            var sample = GetSample();
            var pointer = new JsonPointer("/12");

            JToken token = pointer.Find(sample);

            Assert.Equal("one dozen", (string)token);
        }

        [Fact]
        public void GetValue_Array()
        {
            var sample = GetSample();
            var pointer = new JsonPointer("/3/2");

            JToken token = pointer.Find(sample);

            Assert.Equal("mice", (string)token);
        }

        [Fact]
        public void GetValue_Nonroot()
        {
            var sample = GetSample();
            var pointer = new JsonPointer("/foo/1234");

            JToken token = pointer.Find(sample);

            Assert.Equal("bar", (string)token);
        }

        [Fact]
        public void GetValue_Hex()
        {
            var sample = GetSample();
            var pointer = new JsonPointer("/foo/0xdeadbeef");

            JToken token = pointer.Find(sample);

            Assert.Equal(3735928559L, (long)token);
        }

        [Fact]
        public void GetValue_Mixed()
        {
            var sample = GetSample();
            var pointer = new JsonPointer("/foo/0o0");

            JToken token = pointer.Find(sample);

            Assert.Equal("spooky", (string)token);
        }

        [Fact]
        public void AddAtEnd()
        {

            var sample = GetSample();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/3/-");

            patchDocument.AddOperation(new AddReplaceOperation() { Path = pointer, Value = new JValue("elephant") });

            patchDocument.ApplyTo(sample);

            Assert.Equal("elephant", sample["3"][3]);
        }
    }
}
