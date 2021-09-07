using GaussianInterpolationResearch.TestFunctions;
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

	public interface IDataInterpolation
	{
		public abstract GaussianMethodAlpha GaussianAlpha { get; set; }

		public abstract InterpolatedPointsDict BuildInterpolations();
		public abstract BasisAndCorrectFuncValues[] GetBasisAndFuncValues();
		public abstract InterpolationMethods[] GetInterpolationMethods();
	}

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

	public class SimpleFunctionInterpolation : FunctionInterpolation
	{
		protected readonly Func<double, PointPair> testFunctionPoint;
		protected GaussianMethodAlpha gaussianAlpha = new GaussianMethodAlpha();

		public SimpleFunctionInterpolation(TestFunctionBase testFunc, BasisAndCorrectFuncValues basisAndCorrectFuncValues)
			: base(testFunc)
		{
			BasisAndFuncValues = new BasisAndCorrectFuncValues[] {
				basisAndCorrectFuncValues
			};

			makeMethodsAndSetupAlpha();
		}

		public override InterpolatedPointsDict BuildInterpolations()
		{
			var interpolatedPoints = new InterpolatedPointsDict();
			foreach (var method in InterpolationMethods[0]) {
				interpolatedPoints[method.Key] = makeInterpolation(method.Value);
			}

			return interpolatedPoints;
		}

		public override GaussianMethodAlpha GaussianAlpha 
		{ 
			get => gaussianAlpha; 
			set { 
				gaussianAlpha = value;
				makeMethodsAndSetupAlpha(); 
			}
		}

		private void makeMethodsAndSetupAlpha()
		{
			InterpolationMethods = new InterpolationMethods[1];
			var basisPoints = BasisAndFuncValues[0].BasisPoints;

			double gaussAlpha = GaussianAlpha.TryGetValue(Method.Gaus, out var customGaussAlpha)
								? customGaussAlpha
								: Halper.GetSidorenkoAlpha(basisPoints.Count - 1, basisPoints.XMin(), basisPoints.XMax());

			GaussianAlpha[Method.Gaus] = gaussAlpha;
			InterpolationMethods[0] = Halper.AllInterpolations(basisPoints, gaussAlpha);

			// Setup Alpha for Gaussian methods of interpolation
			foreach (var gaussianParamIntrplType in new[] { Method.GausParamNormal, Method.GausParamSum }) {
				var gaussianParamIntrpl = InterpolationMethods[0][gaussianParamIntrplType].toGaussian();

				var (TMin, TMax) = (gaussianParamIntrpl.TMin, gaussianParamIntrpl.TMax);
				gaussianParamIntrpl.Alpha = GaussianAlpha.TryGetValue(gaussianParamIntrplType, out var customParamGaussAlpha)
											? customParamGaussAlpha
											: Halper.GetSidorenkoAlpha(basisPoints.Count - 1, TMin, TMax);

				GaussianAlpha[gaussianParamIntrplType] = gaussianParamIntrpl.Alpha;
			}
		}

		private PointPairList makeInterpolation(InterpolationBase intrplMethod)
		{
			var basisPoints = BasisAndFuncValues[0].BasisPoints;
			PointPairList interpolatedPoints = new PointPairList();

			if (intrplMethod is GaussianParametricInterpolation) {
				var parametricIntrpl = intrplMethod as GaussianParametricInterpolation;

				for (int ti = 1; ti < basisPoints.Count; ti++) {
					double prevT = parametricIntrpl.GetT(ti - 1);
					double curT = parametricIntrpl.GetT(ti);
					double delta = (curT - prevT) / (BasisAndFuncValues[0].PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= BasisAndFuncValues[0].PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
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
					double delta = (curX - prevX) / (BasisAndFuncValues[0].PointsNumberBetweenBasis + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= BasisAndFuncValues[0].PointsNumberBetweenBasis; pbi++) { // pbi means Point Between Bassis Iter
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

	public class DataPointsInterpolation : IDataInterpolation
	{
		private readonly IDataInterpolation dataInterpolation;

		public DataPointsInterpolation(PointPairList dataPoints) =>
			dataInterpolation = new SimpleFunctionInterpolation(null, new DataBasisAndCorrectFuncValues(dataPoints));

		public GaussianMethodAlpha GaussianAlpha { 
			get => dataInterpolation.GaussianAlpha; 
			set => dataInterpolation.GaussianAlpha = value;
		}

		public BasisAndCorrectFuncValues[] GetBasisAndFuncValues() => dataInterpolation.GetBasisAndFuncValues();
		public InterpolationMethods[] GetInterpolationMethods() => dataInterpolation.GetInterpolationMethods();
		public InterpolatedPointsDict BuildInterpolations() => dataInterpolation.BuildInterpolations();
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

		public static List<double> GetDistanceBetweenMethodPoints(BasisAndCorrectFuncValues basisAndCorrectFuncValues, PointPairList interpolatedPoints)
		{
			List<double> distanceBetweenMethodPoints = new List<double>();

			var correctFuncValuesPoints = basisAndCorrectFuncValues.CorrectFuncValuesPoints;
			for (int i = 0; i < correctFuncValuesPoints.Count; i++) {
				if (i % 3 == 0) // We don't include basis points
					continue;

				double delta = distance(correctFuncValuesPoints[i], interpolatedPoints[i]);
				distanceBetweenMethodPoints.Add(delta);
			}

			return distanceBetweenMethodPoints;

			static double distance(PointPair p, PointPair r) => Math.Sqrt(Math.Pow(p.X - r.X, 2) + Math.Pow(p.Y - r.Y, 2));
		}

		public static double GetAlgorithmScore(List<double> distanceBetweenMethodPoints)
		{
			double methodMark = 0;

			foreach (double delta in distanceBetweenMethodPoints) {
				double doubleDelta = delta * delta; // delta^2
				methodMark += doubleDelta;
			}

			methodMark /= distanceBetweenMethodPoints.Count; // average delta
			return Math.Sqrt(methodMark);
		}
	}

	public abstract class BasisAndCorrectFuncValues
	{
		public BasisAndCorrectFuncValues(int pointsNumberBetweenBasis = 2)
		{
			PointsNumberBetweenBasis = pointsNumberBetweenBasis;
		}

		public int PointsNumberBetweenBasis { get; protected set; }

		public PointPairList BasisPoints { get; protected set; }

		public PointPairList CorrectFuncValuesPoints { get; protected set; }
	}

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

	public class DataBasisAndCorrectFuncValues : BasisAndCorrectFuncValues
	{
		public DataBasisAndCorrectFuncValues(PointPairList dataPoints)
		{
			CorrectFuncValuesPoints = dataPoints;

			BasisPoints = new PointPairList();
			for (int i = 0; i <= dataPoints.Count; i += PointsNumberBetweenBasis + 1) {
				BasisPoints.Add(dataPoints[i]);
			}
		}
	}
}
