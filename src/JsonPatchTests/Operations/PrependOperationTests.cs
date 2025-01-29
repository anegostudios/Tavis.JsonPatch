using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using JsonPatch.Operations;


using Tavis;
using Xunit;

namespace JsonPatchTests.Operations
{
    public class PrependOperationTests
    {
        #region Mocks

        private const string MockTitle = "The Eye of The World";
        private const string MockAuthor = "Robert Jordan";

        private static JsonNode MockPatchData()
        {
            return JsonNode.Parse($$"""
                    { 
                      "title" : "{{MockTitle}}",
                      "author" : "{{MockAuthor}}"
                    }
                    """);
        }

        private static JsonObject MockJsonFile()
        {
            return JsonNode.Parse("""
            {
                "books": [
                    {
                      "title" : "The Great Gatsby",
                      "author" : "F. Scott Fitzgerald"
                    },
                    {
                      "title" : "The Grapes of Wrath",
                      "author" : "John Steinbeck"
                    }
                ]
            }
            """)!.AsObject();
        }

        private static JsonNode MockPatch()
        {
            return JsonNode.Parse($$"""
            [{
                "op": "prepend",
                "path": "/books/0",
                "value": {{MockPatchData().ToJsonString()}}
            }]
            """);
        }

        #endregion

        #region Read: Good Path

        [Fact]
        public void Read_ShouldPrependElementToArray_WhenIndexIsExplicitlyProvided()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/books/1"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var list = sample["books"].AsArray().ToList();
            Assert.Equal(3, list.Count);

            var actual = list[0];
            Assert.Equal(MockAuthor, actual["author"].GetValue<string>());
            Assert.Equal(MockTitle, actual["title"].GetValue<string>());
        }

        [Fact]
        public void Read_ShouldPrependElementToArray_WhenIndexIsProvidedAsHyphen()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/books/-"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var list = sample["books"].AsArray().ToList();
            Assert.Equal(3, list.Count);

            var actual = list[0];
            Assert.Equal(MockAuthor, actual["author"].GetValue<string>());
            Assert.Equal(MockTitle, actual["title"].GetValue<string>());
        }

        [Fact]
        public void Read_ShouldPrependElementToArray_WhenIndexProvidedIsOutsideTheBoundsOfTheArray()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/books/4"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var list = sample["books"].AsArray().ToList();
            Assert.Equal(3, list.Count);

            var actual = list[0];
            Assert.Equal(MockAuthor, actual["author"].GetValue<string>());
            Assert.Equal(MockTitle, actual["title"].GetValue<string>());
        }

        #endregion

        #region Read: Bad Path

        [Fact]
        public void Read_ShouldThrowArgumentException_WhenInvalidIndexIsProvided()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/books/invalid"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);

            Assert.Throws<InvalidOperationException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });
        }

        [Fact]
        public void Read_ShouldThrowPathNotFoundException_WhenInvalidPathIsProvided()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/invalid/0"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);

            Assert.Throws<PathNotFoundException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });
        }

        [Fact]
        public void Read_ShouldThrowPathNotFoundException_WhenInvalidPathAndIndexAreProvided()
        {
            var sample = MockJsonFile();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/invalid/invalid"),
                Value = MockPatchData()
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);

            Assert.Throws<PathNotFoundException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });
        }

        #endregion

        #region Write: Good Path

        [Fact]
        public void Write_ShouldProducePrependPatch_WhenCalled()
        {
            // Arrange
            var expected = MockPatch().ToJsonString();
            var patchDocument = new PatchDocument();
            var sut = new PrependOperation
            {
                Path = new JsonPointer("/books/1"),
                Value = MockPatchData()
            };

            // Act
            patchDocument.AddOperation(sut);
            var stream = patchDocument.ToStream();
            var reader = new StreamReader(stream);

            // Assert
            var actual = reader.ReadToEnd();
            Assert.Equal(expected, actual);

            // Teardown
            reader.Dispose();
            stream.Dispose();
        }

        #endregion
    }
}