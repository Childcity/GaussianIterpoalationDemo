using System;
using System.Drawing;
using System.Linq;
using ZedGraph;

namespace Interpolation {
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
                CurveColor = Color.DeepSkyBlue;
                Symbol = SymbolType.Circle;

                for (int i = 0; i < inputPoints.Count; i++) {
                    XTArray.Add(new PointPair(i, inputPoints[i].X)); // fill Xt(t)
                    YTArray.Add(new PointPair(i, inputPoints[i].Y)); // fill Yt(t)
                }

            } else if (type == ParametricType.Summary) {
                CurveColor = Color.LawnGreen;
                Symbol = SymbolType.XCross;

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

        public override Color CurveColor { get; protected set; }

        public override SymbolType Symbol { get; protected set; }

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
