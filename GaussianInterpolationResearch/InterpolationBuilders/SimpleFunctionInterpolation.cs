using GaussianInterpolationResearch.TestFunctions;
using Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using static GaussianInterpolationResearch.Utils;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;

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

	public interface IFunctionInterpolation
	{
		public abstract InterpolatedPointsDict BuildInterpolations();
	}

	public class SimpleFunctionInterpolation : IFunctionInterpolation
	{
		private readonly TestFunctionBase testFunction;
		private readonly IInterpolationStep step;

		private BasisAndCorrectFuncValues basisAndFuncValues;
		private InterpolationMethods interpolationMethods;

		public SimpleFunctionInterpolation(TestFunctionBase testFunc, IInterpolationStep intrplStep, Func<double, PointPair> testFunctionPoint)
		{
			testFunction = testFunc;
			step = intrplStep;
		}

		public InterpolatedPointsDict BuildInterpolations()
		{
			makeBasisAndCorrectFuncValues();
			makeMethodsAndSetupAlpha();

			var interpolatedPoints = new InterpolatedPointsDict();
			foreach (var method in interpolationMethods) {
				interpolatedPoints[method.Key] = makeInterpolation(method.Value);
			}

			return interpolatedPoints;
		}

		private void makeBasisAndCorrectFuncValues()
		{
			basisAndFuncValues = new BasisAndCorrectFuncValues(
				testFunction, step, x => testFunction.GetValue(x)
			);
		}

		private void makeMethodsAndSetupAlpha()
		{
			var basisPoints = basisAndFuncValues.BasisPoints;

			double gaussAlpha = Halper.GetSidorenkoAlpha(basisPoints.Count - 1, basisPoints.XMin(), basisPoints.XMax());
			interpolationMethods = Halper.AllInterpolations(basisPoints, gaussAlpha);

			// Setup Alpha for Gaussian methods of interpolation
			foreach (var gaussianParamIntrplType in new[] { Method.GausParamNormal, Method.GausParamSum }) {
				var gaussianParamIntrpl = interpolationMethods[gaussianParamIntrplType].toGaussian();

				var (TMin, TMax) = (gaussianParamIntrpl.TMin, gaussianParamIntrpl.TMax);
				gaussianParamIntrpl.Alpha = Halper.GetSidorenkoAlpha(basisPoints.Count - 1, TMin, TMax);
			}
		}

		private PointPairList makeInterpolation(InterpolationBase intrplMethod)
		{
			var basisPoints = basisAndFuncValues.BasisPoints;
			PointPairList interpolatedPoints = new PointPairList();

			if (intrplMethod is GaussianParametricInterpolation) {
				var parametricIntrpl = intrplMethod as GaussianParametricInterpolation;

				for (int ti = 1; ti < basisPoints.Count; ti++) {
					double prevT = parametricIntrpl.GetT(ti - 1);
					double curT = parametricIntrpl.GetT(ti);
					double delta = (curT - prevT) / (basisAndFuncValues.PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= basisAndFuncValues.PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
						interpolatedPoints.Add(parametricIntrpl.GetPoint(prevT + pbi * delta));
					}
				}

				// вместо ниже в цикле выше на <= заменить
				                double lastT = parametricIntrpl.GetT(basisPoints.Count - 1);
				                PointPair lastPoint = parametricIntrpl.GetPoint(lastT);
				                interpolatedPoints.Add(lastPoint);

			} else {
				for (int xi = 1; xi < basisPoints.Count; xi++) {
					double prevX = basisPoints[xi - 1].X;
					double curX = basisPoints[xi].X;
					double delta = (curX - prevX) / (basisAndFuncValues.PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= basisAndFuncValues.PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
						interpolatedPoints.Add(intrplMethod.GetPoint(prevX + pbi * delta));
					}
				}
				
				// вместо ниже в цикле выше на <= заменить
								double lastX = basisPoints[basisPoints.Count - 1].X;
								PointPair lastPoint = intrplMethod.GetPoint(lastX);
								interpolatedPoints.Add(lastPoint);
			}

			return interpolatedPoints;
		}
	}

	public class ParametricFunctionInterpolation : IFunctionInterpolation
	{
		private readonly TestFunctionBase testFunction;
		private readonly IInterpolationStep step;

		public ParametricFunctionInterpolation(TestFunctionBase testFunc, IInterpolationStep intrplStep)
		{
			testFunction = testFunc;
			step = intrplStep;
		}

		public InterpolatedPointsDict BuildInterpolations()
		{

			var XTInterpolation = new SimpleFunctionInterpolation(
				testFunction, step, t => new PointPair(t, testFunction.GetValue(t).X));
			var YTInterpolation = new SimpleFunctionInterpolation(
				testFunction, step, t => new PointPair(t, testFunction.GetValue(t).Y));

			var XTInterpolatedPoints = XTInterpolation.BuildInterpolations();
			var YTInterpolatedPoints = XTInterpolation.BuildInterpolations();

			return XTInterpolatedPoints;
		}
	}

	public static class Halper
	{
		public static InterpolationMethods AllInterpolations(PointPairList basisPoints, double gaussAlpha)
		{
			return new InterpolationMethods {
				{ Method.Lagrange,        new LagrangeInterpolation(basisPoints) },
				{ Method.Gaus,            new GaussianInterpolation(basisPoints, gaussAlpha) },
				{ Method.GausParamNormal, new GaussianParametricInterpolation(basisPoints, gaussAlpha, ParametricType.Normal) },
				{ Method.GausParamSum,    new GaussianParametricInterpolation(basisPoints, gaussAlpha, ParametricType.Summary) }
			};
		}

		public static GaussianParametricInterpolation toGaussian(this InterpolationBase interpolation) => interpolation as GaussianParametricInterpolation;

		public static double GetSidorenkoAlpha(int size, double xMin, double xMax)
		{
			return Math.PI * size / ((xMax - xMin) * (xMax - xMin));
		}
	}

	public class BasisAndCorrectFuncValues
	{
		public BasisAndCorrectFuncValues(TestFunctionBase testFunction, IInterpolationStep step,
										 Func<double, PointPair> testFunctionPoint, 
										 int pointsNumberBetweenBasis = 2)
		{
			PointsNumberBetweenBasis = pointsNumberBetweenBasis;
			constructBasisAndCorrectFuncValues(testFunction, step, testFunctionPoint);
		}

		public int PointsNumberBetweenBasis { get; private set; }

		public PointPairList BasisPoints { get; private set; }

		public PointPairList CorrectFuncValuesPoints { get; private set; }

		private void constructBasisAndCorrectFuncValues(/*needs only interval*/TestFunctionBase testFunction, IInterpolationStep step, Func<double, PointPair> testFunctionPoint)
		{
			BasisPoints = new PointPairList();
			CorrectFuncValuesPoints = new PointPairList();

			// generate basis points and basis points with middle points from func
			int currIter = 0;
			double correctXMax = testFunction.XMax + 0.0001;
			for (double x = testFunction.XMin; x < correctXMax; x += step.Get(currIter)) {
				//if (paramTestFunc != null) {
				//	//BasisPoints.Add(new PointPair(x, isX ? paramTestFunc.GetValue(x).X : paramTestFunc.GetValue(x).Y));
				//} else {
				//	BasisPoints.Add(testFunction.GetValue(x));
				//}
				//Log($"t={x}, {(isX ? "x" : "y")}={BasisPoints.Last()}");
				BasisPoints.Add(testFunctionPoint(x));
				Log($"x//t={x}, X//y(t){BasisPoints.Last()}");

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
						double middle = pbi * delta;
						CorrectFuncValuesPoints.Add(testFunctionPoint(middle));

						//if (paramTestFunc != null) {
						//	middle += x; // x means t
						//CorrectFuncValues.Add(new PointPair(middle, isX ? paramTestFunc.GetValue(middle).X : paramTestFunc.GetValue(middle).Y));
						//} else {
						//	middle += curPoint.X;
						//	CorrectFuncValues.Add(testFunction.GetValue(middle));
						//}
					}
				}

				currIter++;
			}
		}

	}
}
