using GaussianInterpolationResearch.TestFunctions;
using ZedGraph;
using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
	public class ParametricFunctionInterpolation : FunctionInterpolation
	{
		private readonly SimpleFunctionInterpolation XTInterpolation;
		private readonly SimpleFunctionInterpolation YTInterpolation;

		public ParametricFunctionInterpolation(TestFunctionBase testFunc, IInterpolationStep intrplStep)
			: base(testFunc)
		{
			XTInterpolation = new SimpleFunctionInterpolation(
				TestFunction,
				new FunctionBasisAndCorrectFuncValues(TestFunction, intrplStep, t => new PointPair(t, TestFunction.GetValue(t).X))
			);

			YTInterpolation = new SimpleFunctionInterpolation(
				TestFunction,
				new FunctionBasisAndCorrectFuncValues(TestFunction, intrplStep, t => new PointPair(t, TestFunction.GetValue(t).Y))
			);

			BasisAndFuncValues = new BasisAndCorrectFuncValues[] {
				XTInterpolation.GetBasisAndFuncValues()[0],
				YTInterpolation.GetBasisAndFuncValues()[0]
			};

			InterpolationMethods = new InterpolationMethods[] {
				XTInterpolation.GetInterpolationMethods()[0],
				YTInterpolation.GetInterpolationMethods()[0]
			};
		}

		public override GaussianMethodAlpha GaussianAlpha
		{
			get => XTInterpolation.GaussianAlpha;
			set {
				XTInterpolation.GaussianAlpha = value;
				YTInterpolation.GaussianAlpha = value;

				InterpolationMethods = new InterpolationMethods[] {
					XTInterpolation.GetInterpolationMethods()[0],
					YTInterpolation.GetInterpolationMethods()[0]
				};
			}
		}

		public override InterpolatedPointsDict BuildInterpolations()
		{
			var XTInterpolatedPoints = XTInterpolation.BuildInterpolations();
			var YTInterpolatedPoints = YTInterpolation.BuildInterpolations();

			return XTInterpolatedPoints;
		}
	}
}
