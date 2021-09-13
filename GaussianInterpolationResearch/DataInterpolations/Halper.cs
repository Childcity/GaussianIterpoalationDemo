using Interpolation;
using System;
using System.Collections.Generic;
using ZedGraph;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
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
}
