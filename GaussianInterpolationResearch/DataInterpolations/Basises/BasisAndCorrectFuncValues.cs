using ZedGraph;

namespace DataInterpolation
{
	public abstract class BasisAndCorrectFuncValues
	{
		public BasisAndCorrectFuncValues(int pointsNumberBetweenBasis = 2)
		{
			PointsNumberBetweenBasis = pointsNumberBetweenBasis;
		}

		public int PointsNumberBetweenBasis { get; protected set; }

		public PointPairList BasisPoints { get; protected set; }

		public PointPairList CorrectFuncValuesPoints { get; protected set; }
	}
}
