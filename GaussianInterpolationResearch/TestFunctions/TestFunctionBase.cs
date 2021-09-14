using System;
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
		private double miniStep;
		private int pointsCount;

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
		public override string Name { get; protected set; } = "a + b*Phi";
		public override string Subname { get; protected set; } = "Archimedean Spiral";
		public override double XMin { get; protected set; } = 0;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: (initSpiralRadius + spiralGowthRate * t) * Math.Cos(t), 
			y: (initSpiralRadius + spiralGowthRate * t) * Math.Sin(t));

		private static readonly double initSpiralRadius = 0.01;
		private static readonly double finalSpiralRadius = 1;
		private static readonly double numOfTurns = 2;
		private static readonly double spiralGowthRate = (finalSpiralRadius - initSpiralRadius) / (2 * Math.PI * numOfTurns);
	}

	public class FermatsSpiral : ParametricTestFunction
	{
		public override string Name { get; protected set; } = "a * sqrt(Phi)";
		public override string Subname { get; protected set; } = "Fermat's spiral";
		public override double XMin { get; protected set; } = -2 * Math.PI * numOfTurns;
		public override double XMax { get; protected set; } = 2 * Math.PI * numOfTurns;
		public override PointPair GetValue(double t) => new PointPair(
			x: (initSpiralRadius + Math.Sqrt(t)) * Math.Cos(t),
			y: (initSpiralRadius + Math.Sqrt(t)) * Math.Sin(t));

		private static readonly double initSpiralRadius = 0.01;
		private static readonly double finalSpiralRadius = 1;
		private static readonly double numOfTurns = 2;
		private static readonly double spiralGowthRate = (finalSpiralRadius - initSpiralRadius) / (2 * Math.PI * numOfTurns);
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
