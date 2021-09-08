using GaussianInterpolationResearch.TestFunctions;

namespace DataInterpolation
{
	public class IncreasingStep : IInterpolationStep
	{
		private readonly TestFunctionBase testFunction;

		public IncreasingStep(TestFunctionBase testFunction) => this.testFunction = testFunction;

		public double Get(int iter) => testFunction.GetStep(iter);
	}
}
