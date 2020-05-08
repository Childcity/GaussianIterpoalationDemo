using GaussianInterpolationResearch.CovidApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GaussianInterpolationResearch.CovidApi {
	public partial class DayInfo {
		public DateTimeOffset date { get; set; }

		public long confirmed { get; set; }

		public long deaths { get; set; }

		public long recovered { get; set; }
	}
}

public partial class DayInfo {
	public static List<DayInfo> FromJson(string json) => JsonConvert.DeserializeObject<List<DayInfo>>(json, JsonConverterSettings.Settings);
	public string ToJson(bool indented = false) => JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None, JsonConverterSettings.Settings);
}