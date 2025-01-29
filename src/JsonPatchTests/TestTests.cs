using System;
using System.Text.Json.Nodes;
using JsonPatch.Operations;

using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class TestTests
    {
        [Fact]
        public void Test_a_value()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/author");

            patchDocument.AddOperation(new TestOperation() { Path = pointer, Value = JsonValue.Create("Billy Burton") });

            Assert.Throws<InvalidOperationException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });

        }
    }
}
