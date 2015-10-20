using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Model.Exceptions
{
    public class TokenUnauthorizedException : Exception
    {
        public TokenUnauthorizedException()
        {//The access token doesn't have the correct permission for the resource. The access token must include one of the following scopes: 'wl.skydrive_update'.
        }

        public TokenUnauthorizedException(string message) : base(message)
        {
        }

        public TokenUnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenUnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}