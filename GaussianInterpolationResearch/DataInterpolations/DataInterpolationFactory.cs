using GaussianInterpolationResearch.TestFunctions;
using ZedGraph;

namespace DataInterpolation
{
	public static class DataInterpolationFactory
	{
		public static IDataInterpolation GetInstance(TestFunctionBase testFunc, IInterpolationStep intrplStep)
		{
			if (testFunc is ParametricTestFunction) {
				return new ParametricFunctionInterpolation(testFunc, intrplStep);
			}

			return new SimpleFunctionInterpolation(testFunc, new FunctionBasisAndCorrectFuncValues(testFunc, intrplStep, x => testFunc.GetValue(x)));
		}

		public static IDataInterpolation GetInstance(PointPairList dataPoints)
		{
			return new DataPointsInterpolation(dataPoints);
		}
	}
}
