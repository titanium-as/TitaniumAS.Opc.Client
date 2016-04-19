using System;
using System.Runtime.Serialization;

namespace TitaniumAS.Opc.Client.Common
{
    [Serializable]
    public class OpcClientException : Exception
    {
        public OpcClientException()
        {
        }

        public OpcClientException(string message)
            : base(message)
        {
        }

        public OpcClientException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected OpcClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}