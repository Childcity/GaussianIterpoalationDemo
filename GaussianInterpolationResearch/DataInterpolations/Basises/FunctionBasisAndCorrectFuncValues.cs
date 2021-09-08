using GaussianInterpolationResearch.TestFunctions;
using System;
using System.Linq;
using ZedGraph;

namespace DataInterpolation
{
	public class FunctionBasisAndCorrectFuncValues : BasisAndCorrectFuncValues
	{
		public FunctionBasisAndCorrectFuncValues(TestFunctionBase testFunction, IInterpolationStep step,
												 Func<double, PointPair> testFunctionPoint) 
			: base()
		{
			constructBasisAndCorrectFuncValues(testFunction, step, testFunctionPoint);
		}

		private void constructBasisAndCorrectFuncValues(/*needs only interval*/TestFunctionBase testFunction, IInterpolationStep step, Func<double, PointPair> testFunctionPoint)
		{
			BasisPoints = new PointPairList();
			CorrectFuncValuesPoints = new PointPairList();

			// generate basis points and basis points with middle points from func
			int currIter = 0;
			double correctXMax = testFunction.XMax + 0.0001;
			for (double x = testFunction.XMin; x < correctXMax; x += step.Get(currIter)) {
				BasisPoints.Add(testFunctionPoint(x));

				if (double.IsNaN(BasisPoints.Last().Y) || double.IsInfinity(BasisPoints.Last().Y)) {
					throw new ArgumentException($"XMin == {testFunction.XMin} correctXMax == {correctXMax}\n" +
						$"BasisPoints.Last() == {BasisPoints.Last()}\n" +
						$"double.IsNaN(BasisPoints.Last().Y) == {double.IsNaN(BasisPoints.Last().Y)}\n" +
						$"double.IsInfinity(BasisPoints.Last().Y) == {double.IsInfinity(BasisPoints.Last().Y)}", nameof(testFunction));
				}

				PointPair curPoint = BasisPoints.Last();
				CorrectFuncValuesPoints.Add(curPoint);

				double nextX = x + step.Get(currIter + 1);
				if (nextX < correctXMax) {
					// here we should put middle points
					double delta = (nextX - x) / (PointsNumberBetweenBasis + 1);
					for (int pbi = 1; pbi <= PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
						double middle = x + pbi * delta;
						CorrectFuncValuesPoints.Add(testFunctionPoint(middle));
					}
				}

				currIter++;
			}
		}
	}
}
