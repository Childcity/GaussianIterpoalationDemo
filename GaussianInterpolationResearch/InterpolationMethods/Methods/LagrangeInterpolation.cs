using System.Drawing;
using ZedGraph;

namespace Interpolation {
	public class LagrangeInterpolation : InterpolationBase {

        public LagrangeInterpolation(PointPairList inputPoints) : base(inputPoints)
        { }

        public override string Name { get; protected set; } = "Lagrange";

        public override Color CurveColor { get; protected set; } = Color.Violet;

        public override SymbolType Symbol { get; protected set; } = SymbolType.Diamond;

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

}
