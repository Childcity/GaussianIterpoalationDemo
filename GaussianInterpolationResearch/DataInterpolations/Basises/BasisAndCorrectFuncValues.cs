using ZedGraph;

namespace DataInterpolation
{
	public class BasisAndCorrectFuncValues
	{
		public BasisAndCorrectFuncValues(int pointsNumberBetweenBasis = 2)
		{
			PointsNumberBetweenBasis = pointsNumberBetweenBasis;
		}

		public int PointsNumberBetweenBasis { get; protected set; }

		public PointPairList BasisPoints { get; set; }

		public PointPairList CorrectFuncValuesPoints { get; set; }
	}
}
