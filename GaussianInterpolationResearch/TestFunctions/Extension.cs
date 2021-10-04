namespace GaussianInterpolationResearch.TestFunctions
{
	public static class Extension
	{
		public static double Normalize(this double value, double min, double max) => (value - min) / (max - min);
		public static double FromNormalizedToScale(this double value, double min, double max) => (max - min) * value + min;
		public static double Rad(this double val) => System.Math.PI / 180 * val;
	}
}
