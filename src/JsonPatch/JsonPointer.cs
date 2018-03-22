using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class PathNotFoundException : Exception {

        public PathNotFoundException(string message)
            : base(message) { }
        public PathNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class JsonPointer
    {
        private readonly string[] _Tokens;

        public string this[int index] { get { return _Tokens[index]; } }

        public string Last { get { return _Tokens[Depth - 1]; } }

        public int Depth { get { return _Tokens.Length; } }

        public bool IsRoot { get { return (Depth == 0); } }

        public JsonPointer(string pointer)
        {
            _Tokens = (pointer != "/")
                ? pointer.Split('/').Skip(1).Select(Decode).ToArray()
                : new string[0];
        }

        public JToken Find(JToken sample, bool skipLast = false)
        {
            var pointer = sample;
            int length = Depth - (skipLast ? 1 : 0);
            for (int depth = 0; depth < length; depth++)
            {
                var token  = _Tokens[depth];
                var parent = pointer;

                try {
                    int index;
                    if (int.TryParse(token, out index))
                        pointer = pointer[index];
                    else pointer = pointer[token];
                } catch (Exception ex) { throw new PathNotFoundException(
                    $"Cannot traverse beyond '{ ToString(depth + 1) }'. " +
                    $"Parent at this depth is a { parent.GetType().Name }.", ex); }

                if (pointer == null) throw new PathNotFoundException(
                    $"Cannot traverse beyond '{ ToString(depth + 1) }'. " +
                    $"Parent at this depth is a { parent.GetType().Name }.");
            }

            return pointer;
        }

        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        private string Encode(string token)
        {
            return Uri.EscapeDataString(token.Replace("~", "~0").Replace("/", "~1"));
        }

        public override string ToString()
        {
            return ToString(Depth);
        }
        public string ToString(int depth)
        {
            return "/" + String.Join("/", _Tokens.Take(depth).Select(Encode));
        }
    }
}
