using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
	public interface IDataInterpolation
	{
		public abstract GaussianMethodAlpha GaussianAlpha { get; set; }

		public abstract InterpolatedPointsDict BuildInterpolations();
		public abstract BasisAndCorrectFuncValues[] GetBasisAndFuncValues();
		public abstract InterpolationMethods[] GetInterpolationMethods();
	}
}
