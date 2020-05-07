using System;

namespace Interpolation {
	[Serializable]
    public class InterpolationException : Exception {

        public InterpolationException()
        {
        }

        public InterpolationException(string message) : base(message)
        {
        }

        public InterpolationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InterpolationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }

}
