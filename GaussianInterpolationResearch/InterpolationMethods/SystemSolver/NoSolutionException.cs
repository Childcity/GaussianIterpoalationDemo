using System;

namespace SystemSolver {
	[Serializable]
	public class NoSolutionException : ArgumentException {

		public NoSolutionException()
		{
		}

		public NoSolutionException(string message) : base(message)
		{
		}

		public NoSolutionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoSolutionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
		{
		}
	}
}

