using System;
using System.Linq;
using ZedGraph;

namespace GaussianInterpolationResearch.TestFunctions {
	/*
			ArchimedeanSpiral{ x = (a + bt) * cos(t),  y = (a + bt) * sin(t) }
			x^2
			1/x
			Sqrt x
			log x
			Exp x
			1.3^x

			Sin x
			Arcsin x
			Arctan x

			sinh x
			arsinh x 
			csch x 

			sech x
			arsech x
		*/

	public abstract class TestFunctionBase {
		private int pointsCount;
		protected double miniStep;

		protected TestFunctionBase() => PointsCount = 15;
		public abstract string Name { get; protected set; }
		public virtual string Subname { get; protected set; }
		public virtual double XMin { get; protected set; } = 0.1;
		public virtual double XMax { get; protected set; } = 10.1;
		public virtual int PointsCount {
			get => pointsCount; 
			set {
				pointsCount = value;
				int countOfMiniSteps = 0;
				for (int i = 1; i <= pointsCount; i++) {
					countOfMiniSteps += i;
					//Console.WriteLine($"countOfMiniSteps={countOfMiniSteps}, i={i}");
				}
				miniStep = (XMax - XMin) / countOfMiniSteps;
				//Console.WriteLine($"countOfMiniSteps={countOfMiniSteps}, miniStep={miniStep}");
			}
		}

		public virtual double GetStep(int i)
		{
			double currentStep = 0;
			for (int stepI = 0; stepI <= i; stepI++) {
				currentStep += miniStep;//countStep(stepI+1);
			}
			//Console.WriteLine($"Func[{Name}]. Cur step[{i}]: {currentStep}");
			return currentStep;
		}

		public virtual PointPair GetValue(double x) => throw new NotImplementedException();
		
		private double countStep(double currentStepLengInPercent)
		{
			double deltaXMaxMin = XMax - XMin; // 100%
			return deltaXMaxMin * currentStepLengInPercent / 100;
		}
	}

	public abstract class ParametricTestFunction : TestFunctionBase {
		public override double XMin { get; protected set; } = 0;
		public override double XMax { get; protected set; } = 2 * Math.PI;
	}

	public class ArchimedeanSpiral : ParametricTestFunction {
		private const double initSpiralRadius = 0.01;
		private const double finalSpiralRadius = 1;
		private const double numOfTurns = 2;
		private const double spiralGowthRate = (finalSpiralRadius - initSpiralRadius) / (2 * Math.PI * numOfTurns);
		
		public override string Name { get; protected set; } = "a + b*Phi";
		public override string Subname { get; protected set; } = "Archimedean Spiral";
		public override double XMin { get; protected set; } = 0;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: (initSpiralRadius + spiralGowthRate * t) * Math.Cos(t), 
			y: (initSpiralRadius + spiralGowthRate * t) * Math.Sin(t));
}

	public class FermatsSpiral : ParametricTestFunction {
		private const double initSpiralRadius = 0.1;
		private const double finalSpiralRadius = 1;
		private const double numOfTurns = 1;
		private const double spiralGowthRate = (finalSpiralRadius - initSpiralRadius) / (0.7 * Math.PI * numOfTurns);

		public override string Name { get; protected set; } = "a * sqrt(Phi)";
		public override string Subname { get; protected set; } = "Fermat's spiral";
		public override double XMin { get; protected set; } = -2 * Math.PI * numOfTurns;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => 
			t < 0 ? new PointPair(
						x: -spiralGowthRate * Math.Sqrt(-t) * Math.Cos(-t),
						y: -spiralGowthRate * Math.Sqrt(-t) * Math.Sin(-t))
				  : new PointPair(
						x: spiralGowthRate * Math.Sqrt(t) * Math.Cos(t),
						y: spiralGowthRate * Math.Sqrt(t) * Math.Sin(t));

		}

	public class LituusSpiral : ParametricTestFunction {
		private const double numOfTurns = 3;

		public override string Name { get; protected set; } = "1 / sqrt(Phi)";
		public override string Subname { get; protected set; } = "Lituus spiral";
		public override double XMin { get; protected set; } = 0.5;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: 1 / Math.Sqrt(t) * Math.Cos(t),
			y: 1 / Math.Sqrt(t) * Math.Sin(t));
	}

	public class HyperbolicSpiral : ParametricTestFunction {
		public override string Name { get; protected set; } = "a / Phi";
		public override string Subname { get; protected set; } = "Hyperbolic spiral";
		public override double XMin { get; protected set; } = 0 * Math.PI * numOfTurns;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t)
		{
			if (t > -0.2 && t < 0.2) 
				return new PointPair(GetValue(0.2));
			t = t < 0 ? (-(t - XMin + 0.001)) : t;
			return new PointPair(
						x: 1 / (t + 0.001) * Math.Cos(t),
						y: 1 / (t + 0.001) * Math.Sin(t));
		}

		private const double numOfTurns = 3;
	}

	public class LogarithmicSpiral : ParametricTestFunction	{
		private const double numOfTurns = 5;
		private const double alpha = 0.02;
		private const double betta = 0.1;

		public override string Name { get; protected set; } = "a * e^(b*Phi)";
		public override string Subname { get; protected set; } = "Logarithmic spiral";
		public override double XMin { get; protected set; } = 0;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: alpha * Math.Exp(betta * t) * Math.Cos(t),
			y: alpha * Math.Exp(betta * t) * Math.Sin(t));
	}

	public class EulerSpiral : ParametricTestFunction {
		private const int T = 4; // Influence on turns number
		private const int N = 10000;
		private const int scale = 20;

		private const int X = 0, Y = 1;
		private static readonly double[,] cacheXY = new double[N, 2];

		static EulerSpiral() => initCache();

		private static void initCache()
		{
			const double dt = T / (double)N; // Distance delta
			double t = 0; // Distance, that grow with each iteration

			(double zeroDx, double zeroDy) = getClothoid(t, dt);
			cacheXY[0, X] = zeroDx;
			cacheXY[0, Y] = zeroDy;
			t += dt;

			for (int i = 1; i < N; i++, t += dt) {
				(double dx, double dy) = getClothoid(t, dt);
				cacheXY[i, X] = cacheXY[i - 1, X] + dx * scale;
				cacheXY[i, Y] = cacheXY[i - 1, Y] + dy * scale;
			}

			static (double dx, double dy) getClothoid(double t, double dt) => (
				dx: dt * Math.Cos(t * t), 
				dy: dt * Math.Sin(t * t)
			);
		}

		public override string Name { get; protected set; }
		public override string Subname { get; protected set; } = "Clothoid (Euler spiral)";
		public override double XMin { get; protected set; } = Enumerable.Range(0, cacheXY.GetLength(0)).Min(i => cacheXY[i, X]);
		public override double XMax { get; protected set; } = Enumerable.Range(0, cacheXY.GetLength(0)).Max(i => cacheXY[i, X]);

		public override PointPair GetValue(double t)
		{
			double normalizedT = t.Normalize(XMin, XMax);
			int curvePointIndex = (int)normalizedT.FromNormalizedToScale(0, N - 1);
			return new PointPair(
				cacheXY[curvePointIndex, X], 
				cacheXY[curvePointIndex, Y]
			);
		}
	}

	public class GoldenSpiral : ParametricTestFunction { 
		private const double numOfTurns = 3;
		private const double alpha = 0.12;
		private const double thetta = 76; // Tangential angle about 76 degree
		private readonly double pow = thetta.Rad() / (3 * Math.PI / 5);

		public override string Name { get; protected set; } = "";
		public override string Subname { get; protected set; } = "Golden spiral (Fibonacci)";
		public override double XMin { get; protected set; } = 0;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: alpha * Math.Pow(t, pow) * Math.Cos(t),
			y: alpha * Math.Pow(t, pow) * Math.Sin(t));
	}
	public class GoldenSpiralX : TestFunctionBase
	{
		private readonly GoldenSpiral goldenSpiral = new GoldenSpiral();
		public override string Name { get; protected set; } = "";
		public override string Subname { get; protected set; } = "F(x): Golden spiral (Fibonacci)";
		public override double XMin { get => goldenSpiral.XMin; }
		public override double XMax { get => goldenSpiral.XMax; }
		public override PointPair GetValue(double t) => new PointPair(
			x: t,
			y: goldenSpiral.GetValue(t).X
		);
	}
	public class TheodorusSpiral : ParametricTestFunction {
		private const int trianglesNumber = 9;
		private const int X = 0, Y = 1;
		private static readonly double[,] cacheXY = new double[trianglesNumber + 1, 2];

		static TheodorusSpiral() => initCache();

		public override string Name { get; protected set; } = "";
		public override string Subname { get; protected set; } = "Theodorus spiral";
		public override double XMin { get; protected set; } = Enumerable.Range(0, cacheXY.GetLength(0)).Min(i => cacheXY[i, X]);
		public override double XMax { get; protected set; } = Enumerable.Range(0, cacheXY.GetLength(0)).Max(i => cacheXY[i, X]);

		private static void initCache()
		{
			// Find the edge points.

			// Add the first point.
			double t = 0; // Keeps track of the angle that the points make with respect to the spiral’s center

			// Find the leading edge point for each triangle
			for (int i = 1; i <= trianglesNumber; i++) {
				double radius = Math.Sqrt(i);
				cacheXY[i, X] = radius * Math.Cos(t);
				cacheXY[i, Y] = radius * Math.Sin(t);

				t -= Math.Atan2(1, radius);
			}
		}

		public override PointPair GetValue(double t)
		{
			double normalizedT = Extension.Normalize(t, XMin, XMax);
			int curvePointIndex = (int)normalizedT.FromNormalizedToScale(0, cacheXY.GetLength(0) - 1);
			return new PointPair(
				cacheXY[curvePointIndex, X],
				cacheXY[curvePointIndex, Y]
			);
		}
	}

	public class XInPower2 : TestFunctionBase {
		public override string Name { get; protected set; } = "X^2";
		public override PointPair GetValue(double x) => new PointPair(x, x * x);
	}

	public class OneByX : TestFunctionBase {
		public override string Name { get; protected set; } = "1/X";
		public override PointPair GetValue(double x) => new PointPair(x, 1 / x);
	}

	public class SqrtX : TestFunctionBase {
		public override string Name { get; protected set; } = "√x";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Sqrt(x));
	}

	public class Sqrt3X : TestFunctionBase {
		public override string Name { get; protected set; } = "x^(1/3)";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Pow(x, 1/3.0));
	}

	public class NaturalLogarithmX : TestFunctionBase {
		public override string Name { get; protected set; } = "lg(x) == log_e(x)";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Log(x));
	}

	public class Exp0_2X : TestFunctionBase {
		public override string Name { get; protected set; } = "e^0.2x";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Exp(0.2 * x));
	}

	public class _1_3PowerX : TestFunctionBase {
		public override string Name { get; protected set; } = "1.3^x";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Pow(1.3, x));
	}

	public class SinX : TestFunctionBase {
		public override string Name { get; protected set; } = "sin(x)";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Sin(x));
	}

	public class ArcSinX : TestFunctionBase {
		public override string Name { get; protected set; } = "arcsin(x)";
		public override double XMin { get; protected set; } = -1;
		public override double XMax { get; protected set; } = 1;
		//public override double GetStep(int i) { return 0.4; } 
		public override PointPair GetValue(double x) => new PointPair(x, Math.Asin(x));
	}

	public class ArcTgX : TestFunctionBase {
		public override string Name { get; protected set; } = "arctg(x)";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Atan(x));
	}

	public class SinHX : TestFunctionBase {
		public override string Name { get; protected set; } = "sinh(x)";
		public override double XMin { get; protected set; } = -3;
		public override double XMax { get; protected set; } = 3;
		//public override double GetStep(int i) { return 0.9; } 
		public override PointPair GetValue(double x) => new PointPair(x, Math.Sinh(x));
	}

	public class ArcSinHX : TestFunctionBase {
		public override string Name { get; protected set; } = "arsinh(x)";
		public override PointPair GetValue(double x) => new PointPair(x, Math.Log(x + Math.Sqrt(x * x + 1)));
	}

	public class CosecHX : TestFunctionBase {
		public override string Name { get; protected set; } = "csch(x)";
		public override PointPair GetValue(double x) => new PointPair(x, 2 / (Math.Exp(x) - Math.Exp(-x)));
	}

	public class SecHX : TestFunctionBase {
		public override string Name { get; protected set; } = "sech(x)";
		public override PointPair GetValue(double x) => new PointPair(x, 2 / (Math.Exp(x) + Math.Exp(-x)));
	}

	public class ArcSecHX : TestFunctionBase {
		public override string Name { get; protected set; } = "arcsech(x)";
		public override double XMin { get; protected set; } = 0.1;
		public override double XMax { get; protected set; } = 0.9;
		//public override double GetStep(int i) { return 0.08; }
		public override PointPair GetValue(double x) => new PointPair(x, Math.Log((Math.Sqrt(-x * x + 1) + 1) / x));
	}
}
