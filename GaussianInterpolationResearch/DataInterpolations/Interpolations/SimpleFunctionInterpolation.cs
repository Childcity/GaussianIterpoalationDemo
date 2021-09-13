using GaussianInterpolationResearch.TestFunctions;
using Interpolation;
using System;
using ZedGraph;
using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
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
						interpolatedPoints.Add(intrplMethod.GetPoint(prevT + pbi * delta));
					}
				}

				double lastT = parametricIntrpl.GetT(basisPoints.Count - 1);
				interpolatedPoints.Add(parametricIntrpl.GetPoint(lastT));

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

				double lastX = basisPoints[basisPoints.Count - 1].X;
				interpolatedPoints.Add(intrplMethod.GetPoint(lastX));
			}

			return interpolatedPoints;
		}
	}
}
