using System.Collections.Generic;
using System.Linq;
using ZedGraph;

namespace Interpolation {

    public static class Extension {
        public static double XMax(this IList<PointPair> pointPairs) => pointPairs.Max(p => p.X);
        public static double XMin(this IList<PointPair> pointPairs) => pointPairs.Min(p => p.X);
        public static double YMax(this IList<PointPair> pointPairs) => pointPairs.Max(p => p.Y);
        public static double YMin(this IList<PointPair> pointPairs) => pointPairs.Min(p => p.Y);
    }

    public enum ParametricType { Normal, Summary };

    public enum Method
    {
        Lagrange,
        Gaus,
        GausParamNormal,
        GausParamSum
    }
}
