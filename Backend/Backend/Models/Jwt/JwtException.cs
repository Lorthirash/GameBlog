namespace Backend.Models.Options
{
    public class JwtException : Exception
    {
        public JwtException() { }
        public JwtException(string message) : base(message) { }
        public JwtException(string message, Exception inner) : base(message, inner) { }
        protected JwtException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
