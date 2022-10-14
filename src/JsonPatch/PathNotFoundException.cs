using System;

namespace Tavis
{
    public class PathNotFoundException : Exception 
    {
        public PathNotFoundException(string message)
            : base(message) { }
        public PathNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}