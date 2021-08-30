using GaussianInterpolationResearch.TestFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using static GaussianInterpolationResearch.Utils;

namespace GaussianInterpolationResearch.InterpolationBuilders
{

	public interface IInterpolationStep
	{
		double Get(int iter);
	}

	public class FixedStep : IInterpolationStep
	{
		private readonly int step;

		public FixedStep(int step) => this.step = step;

		public double Get(int iter) => step;
	}

	public class IncreasingStep : IInterpolationStep
	{
		private readonly TestFunctionBase testFunction;

		public IncreasingStep(TestFunctionBase testFunction) => this.testFunction = testFunction;

		public double Get(int iter) => testFunction.GetStep(iter);
	}

	public interface I2DFunctionPoint
	{
		public abstract PointPair Get2DFunctionPoint(double argument);
	}

	//public class ParametricFunctionInterpolation : SimpleFunctionInterpolation
	//{
	//
	//}

	public class FunctionInterpolation
	{
		public FunctionInterpolation(TestFunctionBase testFunction, IInterpolationStep step, Func<double, PointPair> functionPoint2D)
		{
			//constructBasisAndCorrectFuncValues
		}

		public int PointsNumberBetweenBasis { get; } = 2;

		public PointPairList BasisPoints { get; private set; }

		public PointPairList CorrectFuncValues { get; private set; }

		private void constructBasisAndCorrectFuncValues(TestFunctionBase testFunction, IInterpolationStep step, Func<double, PointPair> testFunctionPoint)
		{
			//ParametricTestFunction paramTestFunc = testFunction as ParametricTestFunction;

			// generate basis points and basis points with middle points from func
			int currIter = 0;
			double correctXMax = testFunction.XMax + 0.0001;
			for (double x = testFunction.XMin; x < correctXMax; x += step.Get(currIter), currIter++) {
				//if (paramTestFunc != null) {
				//	//BasisPoints.Add(new PointPair(x, isX ? paramTestFunc.GetValue(x).X : paramTestFunc.GetValue(x).Y));
				//} else {
				//	BasisPoints.Add(testFunction.GetValue(x));
				//}
				//Log($"t={x}, {(isX ? "x" : "y")}={BasisPoints.Last()}");
				BasisPoints.Add(testFunctionPoint(x));

				if (double.IsNaN(BasisPoints.Last().Y) || double.IsInfinity(BasisPoints.Last().Y)) {
					throw new ArgumentException($"XMin == {testFunction.XMin} correctXMax == {correctXMax}\n" +
						$"BasisPoints.Last() == {BasisPoints.Last()}\n" +
						$"double.IsNaN(BasisPoints.Last().Y) == {double.IsNaN(BasisPoints.Last().Y)}\n" +
						$"double.IsInfinity(BasisPoints.Last().Y) == {double.IsInfinity(BasisPoints.Last().Y)}", nameof(testFunction));
				}

				PointPair curPoint = BasisPoints.Last();
				CorrectFuncValues.Add(curPoint);

				double nextX = x + step.Get(currIter + 1);
				if (nextX < correctXMax) {
					// here we should put middle points
					double delta = (nextX - x) / (PointsNumberBetweenBasis + 1);
					for (int pbi = 1; pbi <= PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
						double middle = pbi * delta;
						CorrectFuncValues.Add(testFunctionPoint(middle));

						//if (paramTestFunc != null) {
						//	middle += x; // x means t
						//CorrectFuncValues.Add(new PointPair(middle, isX ? paramTestFunc.GetValue(middle).X : paramTestFunc.GetValue(middle).Y));
						//} else {
						//	middle += curPoint.X;
						//	CorrectFuncValues.Add(testFunction.GetValue(middle));
						//}
					}
				}
			}
		}

	}
}
