using System;

namespace SystemSolver {
	[Serializable]
	public class InfiniteSolutionException : Exception {

		public InfiniteSolutionException()
		{
		}

		public InfiniteSolutionException(string message) : base(message)
		{
		}

		public InfiniteSolutionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InfiniteSolutionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}