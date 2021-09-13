using GaussianInterpolationResearch.TestFunctions;
using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
	public abstract class FunctionInterpolation : IDataInterpolation
	{
		protected FunctionInterpolation(TestFunctionBase testFunc)
		{
			TestFunction = testFunc;
		}

		public TestFunctionBase TestFunction { get; set; }
		public abstract GaussianMethodAlpha GaussianAlpha { get; set; }

		protected BasisAndCorrectFuncValues[] BasisAndFuncValues { get; set; }
		protected InterpolationMethods[] InterpolationMethods { get; set; }

		public abstract InterpolatedPointsDict BuildInterpolations();

		public BasisAndCorrectFuncValues[] GetBasisAndFuncValues() => BasisAndFuncValues;
		public InterpolationMethods[] GetInterpolationMethods() => InterpolationMethods;
	}
}
