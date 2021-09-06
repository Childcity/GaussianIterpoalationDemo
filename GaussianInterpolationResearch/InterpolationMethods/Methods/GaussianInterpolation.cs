using SystemSolver;
using System;
using ZedGraph;
using System.Drawing;

namespace Interpolation {

	public class GaussianInterpolation : InterpolationBase {
        private readonly double[] basis;

        public GaussianInterpolation(PointPairList inputPoints, double alpha) : base(inputPoints)
        {
            Alpha = alpha;
            basis = findGaussianBasis();
        }

        public override string Name { get; protected set; } = "Gaussian Non Parametric";

        public override Color CurveColor { get; protected set; } = Color.MediumSlateBlue;

        public override SymbolType Symbol { get; protected set; } = SymbolType.Square;

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

}
