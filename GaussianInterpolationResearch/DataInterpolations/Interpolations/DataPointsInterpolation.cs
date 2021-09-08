using ZedGraph;
using GaussianMethodAlpha = System.Collections.Generic.Dictionary<Interpolation.Method, double>;
using InterpolatedPointsDict = System.Collections.Generic.Dictionary<Interpolation.Method, ZedGraph.PointPairList>;
using InterpolationMethods = System.Collections.Generic.Dictionary<Interpolation.Method, Interpolation.InterpolationBase>;

namespace DataInterpolation
{
	public class DataPointsInterpolation : IDataInterpolation
	{
		private readonly IDataInterpolation dataInterpolation;

		public DataPointsInterpolation(PointPairList dataPoints) =>
			dataInterpolation = new SimpleFunctionInterpolation(null, new DataBasisAndCorrectFuncValues(dataPoints));

		public GaussianMethodAlpha GaussianAlpha { 
			get => dataInterpolation.GaussianAlpha; 
			set => dataInterpolation.GaussianAlpha = value;
		}

		public BasisAndCorrectFuncValues[] GetBasisAndFuncValues() => dataInterpolation.GetBasisAndFuncValues();
		public InterpolationMethods[] GetInterpolationMethods() => dataInterpolation.GetInterpolationMethods();
		public InterpolatedPointsDict BuildInterpolations() => dataInterpolation.BuildInterpolations();
	}
}
