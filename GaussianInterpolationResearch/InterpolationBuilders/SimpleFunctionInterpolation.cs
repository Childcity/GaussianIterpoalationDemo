using GaussianInterpolationResearch.TestFunctions;
using static GaussianInterpolationResearch.Utils;
using Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using ZedGraph;
using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace MethodsInterpolation
{

	public interface IInterpolationStep
	{
		double Get(int iter);
	}

	public class FixedStep : IInterpolationStep
	{
		private readonly double step;

		public FixedStep(double step) => this.step = step;

		public double Get(int _) => step;
	}

	public class IncreasingStep : IInterpolationStep
	{
		private readonly TestFunctionBase testFunction;

		public IncreasingStep(TestFunctionBase testFunction) => this.testFunction = testFunction;

		public double Get(int iter) => testFunction.GetStep(iter);
	}



	public interface IDataInterpolation
	{
		public abstract GaussianMethodAlpha CustomGaussianAlpha { get; set; }
		public abstract InterpolatedPointsDict BuildInterpolations();
	}

	public abstract class FunctionInterpolation : IDataInterpolation
	{
		protected readonly IInterpolationStep step;
		protected GaussianMethodAlpha customGaussianAlpha = new GaussianMethodAlpha();

		protected FunctionInterpolation(TestFunctionBase testFunc,
			IInterpolationStep intrplStep)
		{
			TestFunction = testFunc;
			step = intrplStep;
		}

		public TestFunctionBase TestFunction { get; set; }

		public abstract GaussianMethodAlpha CustomGaussianAlpha { get; set; }

		public abstract InterpolatedPointsDict BuildInterpolations();
	}

	public static class DataInterpolationFactory
	{
		public static IDataInterpolation GetInstance(TestFunctionBase testFunc, IInterpolationStep intrplStep)
		{
			if (testFunc is ParametricTestFunction) {
				return new ParametricFunctionInterpolation(testFunc, intrplStep);
			}

			return new SimpleFunctionInterpolation(testFunc, intrplStep, x => testFunc.GetValue(x));
		}
	}

	public class SimpleFunctionInterpolation : FunctionInterpolation
	{
		protected readonly Func<double, PointPair> testFunctionPoint;

		public SimpleFunctionInterpolation(TestFunctionBase testFunc, IInterpolationStep intrplStep, Func<double, PointPair> testFuncPoint)
			: base(testFunc, intrplStep)
		{
			testFunctionPoint = testFuncPoint;

			makeBasisAndCorrectFuncValues();
			makeMethodsAndSetupAlpha();
		}

		public override InterpolatedPointsDict BuildInterpolations()
		{
			var interpolatedPoints = new InterpolatedPointsDict();
			foreach (var method in InterpolationMethods) {
				interpolatedPoints[method.Key] = makeInterpolation(method.Value);
			}

			return interpolatedPoints;
		}

		public BasisAndCorrectFuncValues BasisAndFuncValues { get; private set; }

		public InterpolationMethods InterpolationMethods { get; private set; }

		public override GaussianMethodAlpha CustomGaussianAlpha
		{
			get => customGaussianAlpha;
			set {
				customGaussianAlpha = value;
				makeMethodsAndSetupAlpha();
			}
		}

		private void makeBasisAndCorrectFuncValues()
		{
			BasisAndFuncValues = new BasisAndCorrectFuncValues(TestFunction, step, testFunctionPoint);
		}

		private void makeMethodsAndSetupAlpha()
		{
			var basisPoints = BasisAndFuncValues.BasisPoints;

			double gaussAlpha = CustomGaussianAlpha.TryGetValue(Method.Gaus, out var customGaussAlpha)
								? customGaussAlpha
								: Halper.GetSidorenkoAlpha(basisPoints.Count - 1, basisPoints.XMin(), basisPoints.XMax());

			CustomGaussianAlpha[Method.Gaus] = gaussAlpha;
			InterpolationMethods = Halper.AllInterpolations(basisPoints, gaussAlpha);

			// Setup Alpha for Gaussian methods of interpolation
			foreach (var gaussianParamIntrplType in new[] { Method.GausParamNormal, Method.GausParamSum }) {
				var gaussianParamIntrpl = InterpolationMethods[gaussianParamIntrplType].toGaussian();

				var (TMin, TMax) = (gaussianParamIntrpl.TMin, gaussianParamIntrpl.TMax);
				gaussianParamIntrpl.Alpha = CustomGaussianAlpha.TryGetValue(gaussianParamIntrplType, out var customParamGaussAlpha)
											? customParamGaussAlpha
											: Halper.GetSidorenkoAlpha(basisPoints.Count - 1, TMin, TMax);

				CustomGaussianAlpha[gaussianParamIntrplType] = gaussianParamIntrpl.Alpha;
			}
		}

		private PointPairList makeInterpolation(InterpolationBase intrplMethod)
		{
			var basisPoints = BasisAndFuncValues.BasisPoints;
			PointPairList interpolatedPoints = new PointPairList();

			if (intrplMethod is GaussianParametricInterpolation) {
				var parametricIntrpl = intrplMethod as GaussianParametricInterpolation;

				for (int ti = 1; ti < basisPoints.Count; ti++) {
					double prevT = parametricIntrpl.GetT(ti - 1);
					double curT = parametricIntrpl.GetT(ti);
					double delta = (curT - prevT) / (BasisAndFuncValues.PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= BasisAndFuncValues.PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
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
					double delta = (curX - prevX) / (BasisAndFuncValues.PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= BasisAndFuncValues.PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
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

	public class ParametricFunctionInterpolation : FunctionInterpolation
	{
		private SimpleFunctionInterpolation XTInterpolation;
		private SimpleFunctionInterpolation YTInterpolation;

		public ParametricFunctionInterpolation(TestFunctionBase testFunc, IInterpolationStep intrplStep)
			: base(testFunc, intrplStep)
		{
			init();
		}

		public override GaussianMethodAlpha CustomGaussianAlpha { 
			get => XTInterpolation.CustomGaussianAlpha;
			set {
				customGaussianAlpha = value;
				if (XTInterpolation != null) {
					XTInterpolation.CustomGaussianAlpha = customGaussianAlpha;
				}
				//init();
			}
		}

		public override InterpolatedPointsDict BuildInterpolations()
		{
			var XTInterpolatedPoints = XTInterpolation.BuildInterpolations();
			var YTInterpolatedPoints = YTInterpolation.BuildInterpolations();

			return XTInterpolatedPoints;
		}

		public BasisAndCorrectFuncValues[] BasisAndFuncValues { get; private set; }

		public InterpolationMethods[] InterpolationMethods { get; private set; }

		void init()
		{

			XTInterpolation = new SimpleFunctionInterpolation(
				TestFunction, step, t => new PointPair(t, TestFunction.GetValue(t).X));

			YTInterpolation = new SimpleFunctionInterpolation(
				TestFunction, step, t => new PointPair(t, TestFunction.GetValue(t).Y));

			BasisAndFuncValues = new BasisAndCorrectFuncValues[] {
				XTInterpolation.BasisAndFuncValues,
				YTInterpolation.BasisAndFuncValues
			};

			InterpolationMethods = new InterpolationMethods[] {
				XTInterpolation.InterpolationMethods,
				YTInterpolation.InterpolationMethods
			};

			Log($"{XTInterpolation.CustomGaussianAlpha[Method.Gaus]}" +
				$"{XTInterpolation.CustomGaussianAlpha[Method.GausParamNormal]}" +
				$"{XTInterpolation.CustomGaussianAlpha[Method.GausParamSum]}");
			Log($"{YTInterpolation.CustomGaussianAlpha[Method.Gaus]}" +
				$"{YTInterpolation.CustomGaussianAlpha[Method.GausParamNormal]}" +
				$"{YTInterpolation.CustomGaussianAlpha[Method.GausParamSum]}");
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

		public static double GetAlgorithmScore(BasisAndCorrectFuncValues basisAndCorrectFuncValues, PointPairList interpolatedPoints)
		{
			// count delta between basis and interpolatedPoints
			List<double> distanceBetweenMethodPoints = new List<double>();
			double methodMark = 0;
			int countOfDeltas = 0;

			var correctFuncValuesPoints = basisAndCorrectFuncValues.CorrectFuncValuesPoints;
			for (int i = 0; i < correctFuncValuesPoints.Count; i++) {
				if (i % 3 == 0) // We don't include basis
					continue;

				double delta = distance(correctFuncValuesPoints[i], interpolatedPoints[i]);
				distanceBetweenMethodPoints.Add(delta);

				delta *= delta; // delta^2
				methodMark += delta;
				countOfDeltas++;
			}

			methodMark /= countOfDeltas; // average delta
			methodMark = Math.Sqrt(methodMark);

			return methodMark;

			static double distance(PointPair p, PointPair r) => Math.Sqrt(Math.Pow(p.X - r.X, 2) + Math.Pow(p.Y - r.Y, 2));
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
				//Log($"x//t={x}, X//y(t){BasisPoints.Last()}");

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
