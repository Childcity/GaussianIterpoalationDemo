using ZedGraph;

namespace DataInterpolation
{
	public class DataBasisAndCorrectFuncValues : BasisAndCorrectFuncValues
	{
		public DataBasisAndCorrectFuncValues(PointPairList dataPoints)
		{
			CorrectFuncValuesPoints = dataPoints;

			BasisPoints = new PointPairList();
			for (int i = 0; i <= dataPoints.Count; i += PointsNumberBetweenBasis + 1) {
				BasisPoints.Add(dataPoints[i]);
			}
		}
	}
}
