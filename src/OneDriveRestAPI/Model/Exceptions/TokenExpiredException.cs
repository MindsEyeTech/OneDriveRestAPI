using System;

namespace OneDriveRestAPI.Model.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException()
        {//The access token doesn't have the correct permission for the resource. The access token must include one of the following scopes: 'wl.skydrive_update'.
        }

        public TokenExpiredException(string message) : base(message)
        {
        }

        public TokenExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //protected TokenExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //}
    }

    public class SerializationInfo
    {
    }
}