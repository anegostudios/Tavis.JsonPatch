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

        string origPointer;

        public JsonPointer(string pointer)
        {
            this.origPointer = pointer;

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

                int index;
                bool isInt = int.TryParse(token, out index);

                Exception ex = null;

                try {

                    if (isInt)
                    {
                        pointer = pointer[index];
                    }
                    else
                    {
                        pointer = pointer[token];
                    }

                } catch (Exception exception) {
                    pointer = null;
                    ex = exception;
                }

                if (pointer == null)
                {
                    if (depth > 0)
                    {
                        string foundPath = _Tokens.Take(depth).Aggregate("", (a,b) => a +"/"+b);
                        throw new PathNotFoundException(string.Format(
                            "The json path {0} was not found. Could traverse until {1}, but then '{2}' does not exist. Full json at this path: {3}", origPointer, foundPath, _Tokens[depth], parent.ToString()
                        ), ex);
                    } else
                    {
                        throw new PathNotFoundException(string.Format(
                            "The json path {0} was not found. No such element '{1}' at the root path", origPointer, _Tokens[0]
                        ), ex);
                    }
                }
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
