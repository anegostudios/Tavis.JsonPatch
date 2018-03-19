using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class PathNotFoundException : Exception {

        public PathNotFoundException(string message) : base(message) { }
        public PathNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class JsonPointer
    {
        private readonly string[] _Tokens;

        public JsonPointer(string pointer)
        {
            _Tokens = pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        private JsonPointer(string[] tokens)
        {
            _Tokens = tokens;
        }
        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        public bool IsNewPointer()
        {
            return _Tokens.Last() == "-";
        }

        public JsonPointer ParentPointer
        {
            get
            {
                if (_Tokens.Length == 0) return null;
                return new JsonPointer(_Tokens.Take(_Tokens.Length - 1).ToArray());
            }
        }

        public JToken Find(JToken sample)
        {
            if (_Tokens.Length == 0)
            {
                return sample;
            }

            var pointer = sample;

            for (int depth = 0; depth < _Tokens.Length; depth++)
            {
                string token = _Tokens[depth];

                if (pointer is JArray)
                {
                    try
                    {
                        pointer = pointer[Convert.ToInt32(token)];
                    }
                    catch (InvalidCastException e)
                    {
                        throw new PathNotFoundException("Cannot traverse beyond depth " + depth + ". The json token at this depth is an array, but non-integer value " + token + " was supplied", e);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new PathNotFoundException("Cannot traverse beyond depth " + depth + ". The json token at this depth is an array, but integer value " + token + " is out of range", e);
                    }
                }
                else
                {
                    pointer = pointer[token];

                    if (pointer == null)
                    {
                        throw new PathNotFoundException("Cannot traverse beyond depth " + depth + ". Token " + token + " was not found");
                    }
                }
            }

            return pointer;
        }

        public override string ToString()
        {
            return "/" + String.Join("/", _Tokens);
        }
    }
}
