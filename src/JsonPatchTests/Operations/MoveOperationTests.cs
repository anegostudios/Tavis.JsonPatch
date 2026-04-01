using System;
using System.IO;
using System.Linq;
using JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace JsonPatchTests.Operations
{
    public class MoveOperationTests
    {
        #region Mocks

        private const string MockBooksTitle0 = "The Great Gatsby";
        private const string MockBooksAuthor0 = "F. Scott Fitzgerald";

        private const string MockBooksTitle1 = "The Grapes of Wrath";
        private const string MockBooksAuthor1 = "John Steinbeck";

        private static JToken MockJsonFile()
        {
            return JToken.Parse($@"{{
                'books': [
                    {{
                      'title' : '{MockBooksTitle0}',
                      'author' : '{MockBooksAuthor0}'
                    }},
                    {{
                      'title' : '{MockBooksTitle1}',
                      'author' : '{MockBooksAuthor1}'
                    }}
                ]
            }}");
        }

        private static JToken MockPatch()
        {
            return JToken.Parse(@"[{
                'op': 'move',
                'path': '/bookshelf',
                'from': '/books'
            }]");
        }

        #endregion

        #region Valid Paths

        [Fact]
        public void ShouldMoveElementToSpecificLocation()
        {
            var sample = MockJsonFile();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/read"),
                FromPath = new JsonPointer("/books"),
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var target = sample["read"].ToList();
            Assert.Equal(2, target.Count);

            var book0 = target[0];
            Assert.Equal(MockBooksAuthor0, book0["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle0, book0["title"].ToObject<string>());

            var book1 = target[1];
            Assert.Equal(MockBooksAuthor1, book1["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle1, book1["title"].ToObject<string>());
        }

        [Fact]
        public void ShouldAddElementAtSpecificLocation_WhenIndexIsExplicitlyProvided()
        {
            var sample = MockJsonFile();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/read"),
                FromPath = new JsonPointer("/books/1"),
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var origin = sample["books"];
            Assert.IsType<JArray>(origin);
            Assert.Single(origin);

            var book0 = origin[0];
            Assert.Equal(MockBooksAuthor0, book0["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle0, book0["title"].ToObject<string>());

            var book1 = sample["read"];
            Assert.IsType<JObject>(book1);

            Assert.Equal(MockBooksAuthor1, book1["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle1, book1["title"].ToObject<string>());
        }

        [Fact]
        public void ShouldMoveElementToSpecificLocation_PathSubstringsAreFine()
        {
            var sample = MockJsonFile();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/bookshelf"),
                FromPath = new JsonPointer("/books"),
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);
            patchDocument.ApplyTo(sample);

            var target = sample["bookshelf"].ToList();
            Assert.Equal(2, target.Count);

            var book0 = target[0];
            Assert.Equal(MockBooksAuthor0, book0["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle0, book0["title"].ToObject<string>());

            var book1 = target[1];
            Assert.Equal(MockBooksAuthor1, book1["author"].ToObject<string>());
            Assert.Equal(MockBooksTitle1, book1["title"].ToObject<string>());

        }

        #endregion

        #region Invalid Paths

        [Fact]
        public void ShouldThrowInvalidOperationException_WhenPathDoesnotExisit()
        {
            var sample = MockJsonFile();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/temp/1"),
                FromPath = new JsonPointer("/books/invalid")
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);

            Assert.Throws<Tavis.PathNotFoundException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });
        }

        [Fact]
        public void ShouldThrowArgumentException_WhenPathIsASubPathOfFromPath()
        {
            var sample = MockJsonFile();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/books/finished"),
                FromPath = new JsonPointer("/books")
            };

            var patchDocument = new PatchDocument();
            patchDocument.AddOperation(sut);

            Assert.Throws<ArgumentException>(() =>
            {
                patchDocument.ApplyTo(sample);
            });
        }

        #endregion

        #region Serialzation

        [Fact]
        public void ShouldProduceMovePatch()
        {
            // Arrange
            var expected = MockPatch().ToString(Formatting.Indented);
            var patchDocument = new PatchDocument();
            var sut = new MoveOperation
            {
                Path = new JsonPointer("/bookshelf"),
                FromPath = new JsonPointer("/books")
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
