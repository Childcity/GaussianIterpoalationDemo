﻿using GaussianInterpolationResearch.CovidApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace GaussianInterpolationResearch {
	public partial class Covid19Settings : Form {
		private static readonly int DefaultPointsCount = 7;

		private CovidRequest covidRequest;

		public Covid19Settings()
		{
			InitializeComponent();
		}

		public (PointPairList basis, PointPairList correctFVal) GetBasis()
		{
			string country = comboBox2.SelectedItem.ToString();
			PointPairList correctFuncValue;

			if (comboBox1.SelectedIndex == 0) {
				correctFuncValue = covidRequest.GetGeneralPoints(country, dateTimePicker1.Value, dateTimePicker2.Value);
			} else {
				correctFuncValue = covidRequest.GetEveryDayPoints(country, dateTimePicker1.Value, dateTimePicker2.Value);
			}

			PointPairList basis = new PointPairList();
			for (int i = 0; i <= correctFuncValue.Count; i += 3) {
				basis.Add(correctFuncValue[i]);
			}
			return (basis, correctFuncValue);
		}

		private async void covid19Settings_Load(object sender, EventArgs e)
		{
			if (covidRequest != null)
				return;

			Enabled = false;
			Cursor = Cursors.WaitCursor;

			covidRequest = new CovidRequest();
			await covidRequest.LoadAsync();


			if (covidRequest.Countries.Count == 0) {
				MessageBox.Show("Can't find File any with db (*.json) or Load db from Internet!", "Error");
				Enabled = true; covidRequest = null; Close();
				return;
			}

			comboBox1.SelectedIndex = 0;
			comboBox2.Items.AddRange(covidRequest.Countries.ToArray());
			comboBox2.SelectedItem = covidRequest.Countries.Contains("Ukraine") ? "Ukraine" : (covidRequest.Countries.FirstOrDefault() ?? "");

			string s = comboBox2.SelectedItem.ToString();
			var d = covidRequest.GetLastDate(comboBox2.SelectedItem.ToString());
			DateTime endDate = new DateTime(Math.Min(
												DateTime.Now.Ticks,
												covidRequest.GetLastDate(comboBox2.SelectedItem.ToString()).Ticks
												));
			dateTimePicker1.Value = endDate.AddDays(-((DefaultPointsCount * 3) + 1));
			dateTimePicker2.Value = endDate;
			numericUpDown1.Value = DefaultPointsCount;
			dateTimePicker1.ValueChanged += updateDates;
			dateTimePicker2.ValueChanged += updateDates;

			Cursor = Cursors.Default;
			Enabled = true;
		}

		private void updateDates(object sender, EventArgs e)
		{
			var startDate = dateTimePicker1.Value;
			int delta = Math.Abs((dateTimePicker2.Value - startDate).Days);

			while (((delta - 1) % 3) != 0) {
				startDate = startDate.AddDays(-1);
				delta = Math.Abs((dateTimePicker2.Value - startDate).Days);
			}

			dateTimePicker1.Value = startDate;
			Console.WriteLine(delta);
			numericUpDown1.Value = (delta - 1) / 3;
		}
	}
}
