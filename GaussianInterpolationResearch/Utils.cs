using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianInterpolationResearch
{
	static class Utils
	{
		public static void Log<T>(T value) => Console.WriteLine(value);
		
		public static string ToDoubString(this double num) => num.ToString("F18");//.TrimEnd(new char[] { '0' });

	}
}
