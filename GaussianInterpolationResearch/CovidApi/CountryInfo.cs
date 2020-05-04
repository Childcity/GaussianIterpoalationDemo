using Newtonsoft.Json;
using System.Collections.Generic;

namespace GaussianInterpolationResearch.CovidApi {
	using CountryInfoDict = Dictionary<string, List<DayInfo>>;

	public static class CountryInfo {
		/*
			{
				"Afghanistan": [
					{
						"date": "2020-1-22",
						"confirmed": 0,
						"deaths": 0,
						"recovered": 0
					}
				]
			}
		 */
		public static CountryInfoDict FromJson(string json) => JsonConvert.DeserializeObject<CountryInfoDict>(json, JsonConverterSettings.Settings);
		public static string ToJson(CountryInfoDict info, bool indented = false) => JsonConvert.SerializeObject(info, indented ? Formatting.Indented : Formatting.None, JsonConverterSettings.Settings);
	}
}
