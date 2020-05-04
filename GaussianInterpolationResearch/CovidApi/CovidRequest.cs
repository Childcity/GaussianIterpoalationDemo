using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace GaussianInterpolationResearch.CovidApi {
	using CountryInfoDict = Dictionary<string, List<DayInfo>>;

	public class CovidRequest {
		private const string StatisticUrl = "https://pomber.github.io/covid19/timeseries.json";

		private CountryInfoDict countryStat = new CountryInfoDict();

		public static async Task<CountryInfoDict> FetchDataAsync()
		{
			try {
				using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })) {
					HttpResponseMessage response = await client.GetAsync(StatisticUrl);
					response.EnsureSuccessStatusCode();
					string result = await response.Content.ReadAsStringAsync();
					return CountryInfo.FromJson(result);
				}
			} catch {
				try {
					// if Exception -> Load from json from currwnt directory (if exist)
					string appDir = AppDomain.CurrentDomain.BaseDirectory;
					//Getting first Json file and trying to get daabase from it
					string firstFileName = new DirectoryInfo(appDir)
												.GetFiles("*.json")
												.OrderByDescending(n => n.Name)
												.FirstOrDefault()
												?.Name;
					return CountryInfo.FromJson(File.ReadAllText(firstFileName));
				} catch (Exception ex) {
					Console.WriteLine(ex.ToString());
					return new CountryInfoDict();
				}
			}
		}

		public List<string> Countries => countryStat.Keys.ToList();

		public async Task LoadAsync() => countryStat = await FetchDataAsync();

		public DateTime GetLastDate(string country)
		{
			return countryStat[country].Max(day => day.date).DateTime;
		}

		public PointPairList GetGeneralPoints(string country, DateTime from, DateTime to)
		{
			PointPairList days = new PointPairList();
			var daysByCountry = countryStat[country].Where(day => day.date >= from.Date && day.date <= to.Date).ToList();
			daysByCountry.Sort((day1, day2) => day1.date.CompareTo(day2.date));
			int x = 0;
			foreach (DayInfo day in daysByCountry) {
				days.Add(new PointPair(x++, day.confirmed, day.date.DateTime.ToShortDateString()));
			}
			return days;
		}

		public PointPairList GetEveryDayPoints(string country, DateTime from, DateTime to)
		{
			PointPairList days = new PointPairList();

			from = from.AddDays(-1);
			var daysByCountry = countryStat[country].Where(day => day.date >= from.Date && day.date <= to.Date).ToList();
			daysByCountry.Sort((day1, day2) => day1.date.CompareTo(day2.date));

			for (int i = 1; i < daysByCountry.Count; i++) {
				DayInfo prevDay = daysByCountry[i-1];
				DayInfo curDay = daysByCountry[i];
				long curDayTotalIlled = curDay.confirmed - prevDay.confirmed;
				days.Add(new PointPair(i-1, curDayTotalIlled, curDay.date.DateTime.ToShortDateString()));
			}
			return days;
		}
	}
}
