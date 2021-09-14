using DataInterpolation;
using GaussianInterpolationResearch.Reports;
using GaussianInterpolationResearch.TestFunctions;
using Interpolation;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace GaussianInterpolationResearch
{
	public partial class Form1 : Form
	{
		public TestFunctionBase[] testFunctions = new TestFunctionBase[] {
			new FermatsSpiral(),
			new ArchimedeanSpiral(),
			new XInPower2(), new OneByX(), new SqrtX(), new Sqrt3X(),
			new NaturalLogarithmX(), new Exp0_2X(), new _1_3PowerX(),
			new SinX(), new ArcSinX(), new ArcTgX(),
			new SinHX(), new ArcSinHX(),
			new CosecHX(), new SecHX(), new ArcSecHX()
		};

		private const double alphaStep = 1000;
		private const int pointsBetweenBasisNumber = 2;
		private static double countAlpha(int iter, int size, double xMin, double xMax)
		{
			return Math.PI * size / ((xMax - xMin) * (xMax - xMin));
		}

		private string xAxis = "--- > X", yAxis = "--- > Y(x)";
		private string title = "";
		private int funcIt = 0;
		Covid19Settings covidSett;

		public Form1()
		{
			InitializeComponent();

			standartFunctionMode.CheckedChanged += (object s, EventArgs e) => modeChanged();

			alphaNonParametricTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChanged(alphaNonParametricTrBar, alphaNonParametricTb);
			alphaNonParametricTb.TextChanged += (object s, EventArgs e) => alphaTextChanged(alphaNonParametricTrBar, alphaNonParametricTb);
			alphaParametricTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChanged(alphaParametricTrBar, alphaParametricTb);
			alphaParametricTb.TextChanged += (object s, EventArgs e) => alphaTextChanged(alphaParametricTrBar, alphaParametricTb);
			alphaSummaryTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChanged(alphaSummaryTrBar, alphaSummaryTb);
			alphaSummaryTb.TextChanged += (object s, EventArgs e) => alphaTextChanged(alphaSummaryTrBar, alphaSummaryTb);

			zedGraph.SizeChanged += onSizeChanged;
			zedGraph.ZoomEvent += onSizeChanged;
			for (int i = 0; i < methodChooser.Items.Count; i++)
				methodChooser.SetItemChecked(i, i == 0);
			methodChooser.ItemCheck += (object sender, ItemCheckEventArgs e) => updateShowingMethod(e);

			settingGraphic();
			zedGraph.AxisChange();
			zedGraph.Invalidate();
		}

		bool skipEvent = false;
		private void alphaTrBarChanged(TrackBar trackBar, TextBox textBox) { if (!skipEvent) textBox.Text = (trackBar.Value / alphaStep).ToDoubString(); }
		private void alphaTextChanged(TrackBar trackBar, TextBox textBox)
		{
			try {
				int newVal = (int)(double.Parse(textBox.Text) * alphaStep);
				if (trackBar.Value != newVal) {
					skipEvent = true;
					trackBar.Value = newVal;
					skipEvent = false;
				}
			} catch {
				trackBar.Enabled = false;
				trackBar.Value = trackBar.Maximum;
				trackBar.Enabled = true;
			}
		}

		private void onSizeChanged(object s, EventArgs e) => setAxisEqualScale();
		private void onSizeChanged(ZedGraphControl zg, ZoomState old, ZoomState r) => setAxisEqualScale();

		private void modeChanged()
		{
			groupBox2.Enabled = !groupBox2.Enabled;
			groupBox1.Enabled = !groupBox2.Enabled;
			stepGb.Enabled = !stepGb.Enabled;
			zedGraph.GraphPane.CurveList.Clear();
			zedGraph.Invalidate();

			if (covid19Mode.Checked) {
				zedGraph.SizeChanged -= onSizeChanged;
				zedGraph.ZoomEvent -= onSizeChanged;
			}
			if (standartFunctionMode.Checked) {
				zedGraph.SizeChanged += onSizeChanged;
				zedGraph.ZoomEvent += onSizeChanged;
			}
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			groupBox3.Enabled = false;
			ReportInputInfo[] reportData = new ReportInputInfo[testFunctions.Length];

			for (funcIt = 0; funcIt < testFunctions.Length; funcIt++) {
				try {
					TestFunctionBase testFunction = testFunctions[funcIt];

					var interpolationStep = stepFixedMode.Checked
											? (IInterpolationStep)new FixedStep(double.Parse(stepTb.Text, CultureInfo.GetCultureInfo("en-US")))
											: new IncreasingStep(testFunction);

					var dataInterpolation = DataInterpolationFactory.GetInstance(testFunction, interpolationStep);

					alphaNonParametricTb.Text = dataInterpolation.GaussianAlpha[Method.Gaus].ToDoubString();
					alphaParametricTb.Text = dataInterpolation.GaussianAlpha[Method.GausParamNormal].ToDoubString();
					alphaSummaryTb.Text = dataInterpolation.GaussianAlpha[Method.GausParamSum].ToDoubString();

					redrawGraphic(dataInterpolation as FunctionInterpolation);

					// waiting, while user check 'checkBox1'
					while (!checkBox1.Checked && !autoReportChBx.Checked) {
						await Task.Delay(1000);
						if (!standartFunctionMode.Checked)
							return;
						if (double.Parse(alphaNonParametricTb.Text) != dataInterpolation.GaussianAlpha[Method.Gaus]
							|| double.Parse(alphaParametricTb.Text) != dataInterpolation.GaussianAlpha[Method.GausParamNormal]
							|| double.Parse(alphaSummaryTb.Text) != dataInterpolation.GaussianAlpha[Method.GausParamSum]) {
							dataInterpolation = DataInterpolationFactory.GetInstance(testFunction, interpolationStep);

							var gaussianAlpha = dataInterpolation.GaussianAlpha;
							gaussianAlpha[Method.Gaus] = double.Parse(alphaNonParametricTb.Text);
							gaussianAlpha[Method.GausParamNormal] = double.Parse(alphaParametricTb.Text);
							gaussianAlpha[Method.GausParamSum] = double.Parse(alphaSummaryTb.Text);
							dataInterpolation.GaussianAlpha = gaussianAlpha;

							redrawGraphic(dataInterpolation as FunctionInterpolation);
						}
					}

					reportData[funcIt] = new ReportInputInfo {
						InterpolationData = dataInterpolation as FunctionInterpolation,
						GraphicImage = zedGraph.GraphPane.GetImage()
					};

				} catch (Exception ex) {
					string funcName = "Unknown function";
					try { funcName = testFunctions[funcIt].Name; } catch { }
					MessageBox.Show(ex.Message, funcName);
				} finally {
					checkBox1.Checked = false;
				}
			}

			if (exportToWordCB.Checked) {
				await exportToMsWord(reportData);
			}

			autoReportChBx.Checked = false;
			groupBox3.Enabled = true;
		}

		private async void button2_Click(object sender, EventArgs e)
		{
			covidSett ??= new Covid19Settings();

			if (DialogResult.OK != covidSett.ShowDialog(this)) {
				return;
			}

			try {
				xAxis = "--- > Day";
				yAxis = "--- > Number of ill people";

				var dataInterpolation = DataInterpolationFactory.GetInstance(covidSett.GetPoints());

				alphaNonParametricTb.Text = dataInterpolation.GaussianAlpha[Method.Gaus].ToDoubString();
				alphaParametricTb.Text = dataInterpolation.GaussianAlpha[Method.GausParamNormal].ToDoubString();
				alphaSummaryTb.Text = dataInterpolation.GaussianAlpha[Method.GausParamSum].ToDoubString();

				redrawGraphic(dataInterpolation);

				while (covid19Mode.Checked) {
					await Task.Delay(1000);
					if (double.Parse(alphaNonParametricTb.Text) != dataInterpolation.GaussianAlpha[Method.Gaus]
							|| double.Parse(alphaParametricTb.Text) != dataInterpolation.GaussianAlpha[Method.GausParamNormal]
							|| double.Parse(alphaSummaryTb.Text) != dataInterpolation.GaussianAlpha[Method.GausParamSum]) {
						dataInterpolation = DataInterpolationFactory.GetInstance(covidSett.GetPoints());

						var gaussianAlpha = dataInterpolation.GaussianAlpha;
						gaussianAlpha[Method.Gaus] = double.Parse(alphaNonParametricTb.Text);
						gaussianAlpha[Method.GausParamNormal] = double.Parse(alphaParametricTb.Text);
						gaussianAlpha[Method.GausParamSum] = double.Parse(alphaSummaryTb.Text);
						dataInterpolation.GaussianAlpha = gaussianAlpha;

						redrawGraphic(dataInterpolation);
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "COVID19 Error");
			} finally {
				zedGraph.GraphPane.CurveList.Clear();
				zedGraph.Invalidate();
			}
		}

		private void zoomGraphic()
		{
			updateShowingMethod(null);
			zedGraph.RestoreScale(zedGraph.GraphPane);
			setAxisTitleName();
			zedGraph.AxisChange();
			zedGraph.Invalidate();
		}

		private void redrawGraphic(IDataInterpolation dataInterpolation)
		{
			GraphPane pane = zedGraph.GraphPane;

			pane.CurveList.Clear();

			var methodInterpolatedPoints = dataInterpolation.BuildInterpolations();

			foreach (var interpolatedPoints in methodInterpolatedPoints) {
				InterpolationBase intrplMethod = dataInterpolation.GetInterpolationMethods()[0][interpolatedPoints.Key];

				{
					// add interpolation to graphic
					LineItem tmpCurve = pane.AddCurve($"F(x) = {intrplMethod.Name}", interpolatedPoints.Value, intrplMethod.CurveColor, intrplMethod.Symbol);
					tmpCurve.Symbol.Size = 5;
					tmpCurve.Symbol.Fill = new Fill(intrplMethod.CurveColor);
					tmpCurve.Line.IsVisible = true;
				}

				{
					// update score
					BasisAndCorrectFuncValues basisAndFuncValues = dataInterpolation.GetBasisAndFuncValues()[0];
					var distances = Halper.GetDistanceBetweenMethodPoints(basisAndFuncValues, interpolatedPoints.Value);
					var methodMark = Halper.GetAlgorithmScore(distances);
					setScore(methodMark, interpolatedPoints.Key);
				}
			}

			{
				FunctionInterpolation funcInterpolation = dataInterpolation as FunctionInterpolation;
				TestFunctionBase testFunction = funcInterpolation?.TestFunction;

				// add basis curve to graphic
				title = testFunction == null ? "Covid-19 Statistics" : $"Interpolation for y = F({testFunction.Name}) {testFunction.Subname}";

				BasisAndCorrectFuncValues basisAndFuncValues = dataInterpolation.GetBasisAndFuncValues()[0];

				LineItem tmpCurve;
				tmpCurve = pane.AddCurve($"Main_basis = {(testFunction == null ? "" : testFunction.Name)}", basisAndFuncValues.BasisPoints, Color.Red, SymbolType.TriangleDown);
				tmpCurve.Symbol.Size = 7;
				tmpCurve.Symbol.Fill = new Fill(Color.Red);
				tmpCurve.Line.IsVisible = false;

				tmpCurve = pane.AddCurve($"Middle_basis = {(testFunction == null ? "" : testFunction.Name)}", basisAndFuncValues.CorrectFuncValuesPoints, Color.Green, SymbolType.TriangleDown);
				tmpCurve.Symbol.Size = 3;
				tmpCurve.Symbol.Fill = new Fill(Color.Green);
				tmpCurve.Line.IsVisible = false;
			}

			zoomGraphic();
		}

		private void setScore(double methodMark, Method method)
		{
			string text = methodMark.ToString("F16");
			switch (method) {
				case Method.Lagrange:
					scoreLagrageLb.Text = text;
					break;
				case Method.Gaus:
					scoreGausLb.Text = text;
					break;
				case Method.GausParamNormal:
					scoreParamNormLb.Text = text;
					break;
				case Method.GausParamSum:
					scoreParamSummLb.Text = text;
					break;
			}
		}

		private void settingGraphic()
		{
			setAxisTitleName();
			setLgent();
			setAxisProperties();
			setAxisEqualScale();
			enableDotMoving();
			enableGrid();
		}

		private void setAxisTitleName()
		{
			GraphPane pane = zedGraph.GraphPane;

			// Изменим тест надписи по оси X
			pane.XAxis.Title.Text = xAxis;

			// Изменим параметры шрифта для оси X
			pane.XAxis.Title.FontSpec.IsUnderline = false;
			pane.XAxis.Title.FontSpec.IsBold = true;
			pane.XAxis.Title.FontSpec.FontColor = Color.Black;

			// Изменим текст по оси Y
			pane.YAxis.Title.Text = yAxis;

			// Изменим текст заголовка графика
			pane.Title.Text = title;

			// В параметрах шрифта сделаем заливку красным цветом
			//pane.Title.FontSpec.Fill.Brush = new SolidBrush(Color.Red);
			//pane.Title.FontSpec.Fill.IsVisible = true;

			// Сделаем шрифт не полужирным
			pane.Title.FontSpec.IsBold = false;

			pane.Title.FontSpec.FontColor = Color.Blue;
			pane.Title.FontSpec.Size = 14;
		}

		private void setLgent()
		{
			GraphPane pane = zedGraph.GraphPane;
			pane.Legend.FontSpec.Size = 9;
			pane.Legend.Position = LegendPos.InsideBotRight;
		}

		private void enableGrid()
		{
			GraphPane pane = zedGraph.GraphPane;

			// Включим ось Х и У
			pane.XAxis.MajorGrid.IsZeroLine = true;
			pane.YAxis.MajorGrid.IsZeroLine = true;

			// Включаем отображение сетки напротив крупных рисок по оси X
			pane.XAxis.MajorGrid.IsVisible = true;
			// Задаем вид пунктирной линии для крупных рисок по оси X:
			// Длина штрихов равна 10 пикселям, ... 
			pane.XAxis.MajorGrid.DashOn = 3;
			// затем 5 пикселей - пропуск
			pane.XAxis.MajorGrid.DashOff = 10;
			pane.XAxis.MajorGrid.Color = Color.DarkGray;


			// Включаем отображение сетки напротив крупных рисок по оси Y
			pane.YAxis.MajorGrid.IsVisible = true;
			// Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
			pane.YAxis.MajorGrid.DashOn = 3;
			pane.YAxis.MajorGrid.DashOff = 10;
			pane.YAxis.MajorGrid.Color = Color.DarkGray;

			// Включаем отображение сетки напротив мелких рисок по оси X
			//pane.YAxis.MinorGrid.IsVisible = true;

			//// Задаем вид пунктирной линии для крупных рисок по оси Y: 
			//// Длина штрихов равна одному пикселю, ... 
			//pane.YAxis.MinorGrid.DashOn = 1;

			//// затем 2 пикселя - пропуск
			//pane.YAxis.MinorGrid.DashOff = 2;

			//// Включаем отображение сетки напротив мелких рисок по оси Y
			//pane.XAxis.MinorGrid.IsVisible = true;

			//// Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
			//pane.XAxis.MinorGrid.DashOn = 1;
			//pane.XAxis.MinorGrid.DashOff = 2;
		}

		private void enableDotMoving()
		{
			// !!!
			// Перемещать точки можно будет с помощью средней кнопки мыши...
			zedGraph.EditButtons = MouseButtons.Middle;

			// ... и при нажатой клавише Alt.
			zedGraph.EditModifierKeys = Keys.Alt;

			// Точки можно перемещать, как по горизонтали,...
			zedGraph.IsEnableHEdit = true;

			// ... так и по вертикали.
			zedGraph.IsEnableVEdit = true;
		}

		private void setAxisProperties()
		{
			GraphPane pane = zedGraph.GraphPane;

			// Установим шаг основных меток, равным 5
			//pane.XAxis.Scale.MajorStep = 0.5;
			//pane.YAxis.Scale.MajorStep = 0.5;

			pane.XAxis.Scale.MinorStepAuto = true;
			pane.YAxis.Scale.MinorStepAuto = true;

			// Немного уменьшим шрифт меток, чтобы их больше умещалось
			pane.XAxis.Scale.FontSpec.Size = 11;
			pane.YAxis.Scale.FontSpec.Size = 11;

			// сделаем чтобы оси пересекались в (0;0)
			//pane.XAxis.Cross = 0;
			//pane.YAxis.Cross = 0;

			// Подпишемся на событие, которое будет вызываться при выводе каждой отметки на оси
			pane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XAxis_ScaleFormatEvent);
			pane.YAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(YAxis_ScaleFormatEvent);
		}

		private string YAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
		{
			if (index % 2 == 0) {
				// Если текущее index кратно 10, то возьмем его в квадратные скобки
				return string.Format("[{0}]", val);
			} else {
				// Остальные числа просто преобразуем в строку
				return val.ToString();
			}
		}

		private string XAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
		{
			if (covidSett != null && covid19Mode.Checked) {
				if (val < 0 || val > 10000)
					return val.ToString();
				if (val % 1 == 0) {
					return $"[{covidSett.dateTimePicker1.Value.Date.AddDays(val).ToShortDateString()}]";
				} else {
					return Math.Round(24.0 * (val - Math.Truncate(val)), 1).ToString().Replace(',', ':');
				}
			} else {
				if (index % 2 == 0) {
					// Если текущее index кратно 10, то возьмем его в квадратные скобки
					return string.Format("[{0}]", val);
				} else {
					// Остальные числа просто преобразуем в строку
					return val.ToString();
				}
			}
		}

		private void setAxisEqualScale()
		{
			GraphPane pane = zedGraph.GraphPane;

			double Xmin = pane.XAxis.Scale.Min;
			double Xmax = pane.XAxis.Scale.Max;

			double Ymin = pane.YAxis.Scale.Min;
			double Ymax = pane.YAxis.Scale.Max;

			PointF PointMin = pane.GeneralTransform(Xmin, Ymin, CoordType.AxisXYScale);
			PointF PointMax = pane.GeneralTransform(Xmax, Ymax, CoordType.AxisXYScale);
			double dX = Math.Abs(Xmax - Xmin);
			double dY = Math.Abs(Ymax - Ymin);

			double Kx = dX / Math.Abs(PointMax.X - PointMin.X);
			double Ky = dY / Math.Abs(PointMax.Y - PointMin.Y);

			double K = Kx / Ky;
			//MessageBox.Show(K.ToString());
			if (K > 1.0) {
				pane.YAxis.Scale.Min = pane.YAxis.Scale.Min - dY * (K - 1.0) / 2.0;
				pane.YAxis.Scale.Max = pane.YAxis.Scale.Max + dY * (K - 1.0) / 2.0;
			} else {
				K = 1.0 / K;
				pane.XAxis.Scale.Min = pane.XAxis.Scale.Min - dX * (K - 1.0) / 2.0;
				pane.XAxis.Scale.Max = pane.XAxis.Scale.Max + dX * (K - 1.0) / 2.0;

			}

			// Обновим данные об осях
			zedGraph.AxisChange();
			// Обновляем график
			zedGraph.Invalidate();

		}

		void updateShowingMethod(ItemCheckEventArgs e)
		{
			for (int i = 0; i < methodChooser.Items.Count; i++) {
				if (e != null)
					i = e.Index;

				string lineToFind = null;
				switch (i) {
					case 0:
						lineToFind = "basis";
						break;
					case 1:
						lineToFind = "lagrange";
						break;
					case 2:
						lineToFind = "non";
						break;
					case 3:
						lineToFind = "normal";
						break;
					case 4:
						lineToFind = "summary";
						break;
				}

				var curve = zedGraph.GraphPane.CurveList.Where(c => c.Label.Text.ToLower().Contains(lineToFind)).ToList();
				if (curve != null) {
					if (e != null) {
						curve.ForEach(c => c.IsVisible = e.NewValue == CheckState.Checked);
						zedGraph.Invalidate();
						return;
					}
					curve.ForEach(c => c.IsVisible = methodChooser.GetItemChecked(i));
				}
			}

			zedGraph.Invalidate();
		}

		private async Task exportToMsWord(ReportInputInfo[] reportData)
		{
			try {
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Visible = true;
				button1.Enabled = false;

				var report = new MsWordReportBuilder(reportData);
				await report.Build();
				//await startSTATask(() => buildReport(reportData));
			} catch (Exception ex) {
				MessageBox.Show($"Error while generating Report!\n{ex.Message}");
			}
			Cursor.Current = Cursors.Arrow;
			progressBar1.Visible = false;
			button1.Enabled = true;
		}

		private void scoreLagrageLb_Click(object sender, EventArgs e)
		{
			Console.WriteLine(scoreLagrageLb.Text);
			Console.WriteLine(scoreGausLb.Text);
			Console.WriteLine(scoreParamNormLb.Text);
			Console.WriteLine(scoreParamSummLb.Text);
		}
	}
}