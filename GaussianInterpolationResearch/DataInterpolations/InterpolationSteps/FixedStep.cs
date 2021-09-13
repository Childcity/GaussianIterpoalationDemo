namespace DataInterpolation
{
	public class FixedStep : IInterpolationStep
	{
		private readonly double step;

		public FixedStep(double step) => this.step = step;

		public double Get(int _) => step;
	}
}
