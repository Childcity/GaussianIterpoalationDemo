using SystemSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Interpolation {

    [Serializable]
    public class InterpolationException : Exception {

        public InterpolationException()
        {
        }

        public InterpolationException(string message) : base(message)
        {
        }

        public InterpolationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InterpolationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }

    public static class Extension {
        public static double XMax(this IList<PointPair> pointPairs) => pointPairs.Max(p => p.X);
        public static double XMin(this IList<PointPair> pointPairs) => pointPairs.Min(p => p.X);
        public static double YMax(this IList<PointPair> pointPairs) => pointPairs.Max(p => p.Y);
        public static double YMin(this IList<PointPair> pointPairs) => pointPairs.Min(p => p.Y);
    }

    public interface IInterpolation {
        PointPair GetPoint(double X);
    }

    public abstract class InterpolationBase : IInterpolation {

        protected InterpolationBase(PointPairList inputPoints) => InputPoints = inputPoints;

        public virtual PointPairList InputPoints { get; }

        public abstract PointPair GetPoint(double X);

        public abstract string Name { get; protected set; }
    }

    public class LagrangeInterpolation : InterpolationBase {

        public LagrangeInterpolation(PointPairList inputPoints) : base(inputPoints)
        { }

        public override string Name { get; protected set; } = "Lagrange";

        public override PointPair GetPoint(double X)
        {
            double L = 0;

            for (int i = 0; i < InputPoints.Count; ++i) {
                double l = 1;

                for (int j = 0; j < InputPoints.Count; ++j)
                    if (i != j)
                        l *= (X - InputPoints[j].X) / (InputPoints[i].X - InputPoints[j].X);

                L += InputPoints[i].Y * l;
            }

            return new PointPair(){ X = X, Y = L };
        }
    }

    public class GaussianInterpolation : InterpolationBase {
        private readonly double[] basis;

        public GaussianInterpolation(PointPairList inputPoints, double alpha) : base(inputPoints)
        {
            Alpha = alpha;
            basis = findGaussianBasis();
        }

        public override string Name { get; protected set; } = "Gaussian Non Parametric";

        public override PointPair GetPoint(double Xl)
        {
            double G = 0;

            for (int i = 0; i < InputPoints.Count; i++) {
                double Xr = InputPoints[i].X;
                G += basis[i] * Math.Exp(-Alpha * Math.Pow(Xl - Xr, 2));
            }

            return new PointPair() { X = Xl, Y = G };
        }

        public virtual double Alpha { get; protected set; }

        private double[] findGaussianBasis()
        {
            int n = InputPoints.Count;

            double[,] Ab = new double[n, n + 1]; // n + 1 because the result (Y) will be here

            // Create Basis matrix. Xl -> Xn, Xr -> X[1..n] in formula
            for (int i = 0; i < n; i++) {
                double xL = InputPoints[i].X;

                for (int j = 0; j < n; j++) {
                    double xR = InputPoints[j].X;
                    Ab[i, j] = Math.Exp(-Alpha * Math.Pow(xL - xR, 2));
                }

                Ab[i, n] = InputPoints[i].Y;
            }

            try {
                return GaussJordanElimination.SolveSystem(Ab, n);
            } catch (NoSolutionException e) {
                throw new InterpolationException("Решения для базисов ф-и Гаусса НЕ найдено!", e);
            } catch (InfiniteSolutionException e) {
                throw new InterpolationException("Решения для базисов ф-и Гаусса содержит бесконечность!", e);
            } catch (Exception e) {
                throw new InterpolationException("GaussJordanElimination Error", e);
            }
        }
    }

    public enum ParametricType { Normal, Summary };

    public class GaussianParametricInterpolation : GaussianInterpolation {
        private GaussianInterpolation gaussianXt;
        private GaussianInterpolation gaussianYt;

        public GaussianParametricInterpolation(PointPairList inputPoints, double alpha, ParametricType type) : base(inputPoints, alpha)
        {
            Type = type;
            Name += type.ToString();

            XTArray = new PointPairList();
            YTArray = new PointPairList();

            if (type == ParametricType.Normal) {
                for (int i = 0; i < inputPoints.Count; i++) {
                    XTArray.Add(new PointPair(i, inputPoints[i].X)); // fill Xt(t)
                    YTArray.Add(new PointPair(i, inputPoints[i].Y)); // fill Yt(t)
                }

            } else if (type == ParametricType.Summary) {
                double previousT = 0;
                XTArray.Add(new PointPair(previousT, inputPoints[0].X)); // fill Xt[0](t)
                YTArray.Add(new PointPair(previousT, inputPoints[0].Y)); // fill Y[0](t)

                for (int i = 1; i < inputPoints.Count; i++) {
                    PointPair prevPoint = inputPoints[i - 1];
                    PointPair currPoint = inputPoints[i];
                    previousT += distanceBetween(prevPoint, currPoint);

                    XTArray.Add(new PointPair(previousT, inputPoints[i].X)); // fill Xt(t)
                    YTArray.Add(new PointPair(previousT, inputPoints[i].Y)); // fill Y(t)
                }
            }

            TMin = XTArray.First().X;
            TMax = XTArray.Last().X;
        }

        public override string Name { get; protected set; } = "Gaussian Parametric ";

        public ParametricType Type { get; protected set; }

        public new double Alpha { get => base.Alpha; set => base.Alpha = value; }

        public double GetT(int index) => XTArray[index].X /*T*/;

        public double TMax { get; protected set; }

        public double TMin { get; protected set; }

        public PointPairList XTArray { get; protected set; }

        public PointPairList YTArray { get; protected set; }

        public override PointPair GetPoint(double T)
        {
            gaussianXt = gaussianXt ?? new GaussianInterpolation(XTArray, Alpha);
            gaussianYt = gaussianYt ?? new GaussianInterpolation(YTArray, Alpha);
            return new PointPair(gaussianXt.GetPoint(T).Y, gaussianYt.GetPoint(T).Y);
        }

        private double distanceBetween(PointPair a, PointPair b)
        {
            double x1 = a.X, x2 = b.X,
                    y1 = a.Y, y2 = b.Y;
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
    }

}
