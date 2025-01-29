using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;


// ReSharper disable UnusedMember.Global

namespace Tavis
{
    public class JsonPointer
    {
        private readonly string _origPointer;

        public string this[int index] => Tokens[index];

        public string Last => Tokens[Depth - 1];

        public int Depth => Tokens.Count;

        public bool IsRoot => Depth == 0;
        
        public List<string> Tokens { get; }
        
        public JsonPointer(string pointer)
        {
            this._origPointer = pointer;
            Tokens = pointer
                .Split('/')
                .Skip(1)
                .Select(Decode)
                .ToList();
        }

        public JsonNode Find(JsonNode sample, bool skipLast = false)
        {
            JsonNode pointer = sample;
            var length = Depth - (skipLast ? 1 : 0);
            for (var depth = 0; depth < length; depth++)
            {
                var token  = Tokens[depth];
                var parent = pointer;
                var isInt = int.TryParse(token, out var index);

                Exception ex = null;

                try
                {
                    pointer = isInt ? pointer[index] : pointer[token];
                } catch (Exception exception) {
                    pointer = null;
                    ex = exception;
                }

                if (pointer != null) continue;
                if (depth <= 0) 
                    throw new PathNotFoundException(
                        $"The json path {_origPointer} was not found. No such element '{Tokens[0]}' at the root path", ex);
                var foundPath = Tokens.Take(depth).Aggregate("", (a,b) => a +"/"+b);
                throw new PathNotFoundException(
                    $"The json path {_origPointer} was not found. Could traverse until {foundPath}, " +
                    $"but then '{Tokens[depth]}' does not exist. Full json at this path: {parent}", ex);

            }

            return pointer;
        }

        public override string ToString()
        {
            return ToString(Depth);
        }

        public string ToString(int depth)
        {
            return "/" + string.Join("/", Tokens.Take(depth).Select(Encode));
        }

        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        private string Encode(string token)
        {
            return Uri.EscapeDataString(token.Replace("~", "~0").Replace("/", "~1"));
        }
    }
}
