﻿using GaussianInterpolationResearch.TestFunctions;
using Interpolation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using Word = Microsoft.Office.Interop.Word;

namespace GaussianInterpolationResearch {
	public partial class Form1 : Form {
		/*
				x^2
				1/x
				Sqrt x
				Sqrt3 x
				log x
				Exp x
				1.3^x

				Sin x
				Arcsin x
				Arctan x

				sinh x
				arsinh x 
				csch x 

				sech x
				arsech x
			*/

		public TestFunctionBase[] testFunctions = new TestFunctionBase[] {
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

		private struct MethodData {
			public InterpolationBase Method { get; set; }
			public PointPairList InterpolatedFuncValue { get; set; }
			public double[] DeltaBetweenMethod { get; set; }
			public double MethodMark { get; set; }
		}

		private struct ReportData {
			public TestFunctionBase TestFunction { get; set; }
			public PointPairList CorrectFuncValue { get; set; }
			public List<MethodData> MethodsData { get; set; }
			public Image GraphicImage { get; set; }
		}

		public Form1()
		{
			InitializeComponent();

			standartFunctionMode.CheckedChanged += (object s, EventArgs e) => modeChanged();

			alphaNonParametricTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChenged(alphaNonParametricTrBar, alphaNonParametricTb);
			alphaNonParametricTb.TextChanged += (object s, EventArgs e) => alphaTextChenged(alphaNonParametricTrBar, alphaNonParametricTb);
			alphaParametricTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChenged(alphaParametricTrBar, alphaParametricTb);
			alphaParametricTb.TextChanged += (object s, EventArgs e) => alphaTextChenged(alphaParametricTrBar, alphaParametricTb);
			alphaSummaryTrBar.ValueChanged += (object s, EventArgs e) => alphaTrBarChenged(alphaSummaryTrBar, alphaSummaryTb);
			alphaSummaryTb.TextChanged += (object s, EventArgs e) => alphaTextChenged(alphaSummaryTrBar, alphaSummaryTb);

			zedGraph.SizeChanged += onSizeChanged;
			zedGraph.ZoomEvent += onSizeChanged;
			for (int i = 0; i < methodChooser.Items.Count; i++)
				methodChooser.SetItemChecked(i, true);
			methodChooser.ItemCheck += (object sender, ItemCheckEventArgs e) => updateShowingMethod(e);

			settingGraphic();
			zedGraph.AxisChange();
			zedGraph.Invalidate();
		}

		bool skipEvent = false;
		private void alphaTrBarChenged(TrackBar trackBar, TextBox textBox) { if (!skipEvent) textBox.Text = (trackBar.Value / alphaStep).ToDoubString(); }
		private void alphaTextChenged(TrackBar trackBar, TextBox textBox)
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
			stepGb.Enabled = false;
			ReportData[] reportData = new ReportData[testFunctions.Length];

			for (funcIt = 0; funcIt < testFunctions.Length; funcIt++) {
				try {
					TestFunctionBase testFunction = testFunctions[funcIt];
					PointPairList basisPoints;
					PointPairList correctFuncValue;

					(basisPoints, correctFuncValue) = constructBasisAndCorrectFuncValues(testFunction);

					setAxisTitleName();

					// create an array from different interpolations types
					double normalAlpha = countAlpha(0, basisPoints.Count - 1, basisPoints.XMin(), basisPoints.XMax());
					InterpolationBase[] interpolations = constructInterpolationList(basisPoints, normalAlpha);

					// setup Alpha for Gaussian methods of interpolation
					intrplHelper(interpolations).gaussParametricIntrpl.Alpha = countAlpha(0, basisPoints.Count - 1, intrplHelper(interpolations).gaussParametricIntrpl.TMin, intrplHelper(interpolations).gaussParametricIntrpl.TMax);
					intrplHelper(interpolations).gaussSummaryIntrpl.Alpha = countAlpha(0, basisPoints.Count - 1, intrplHelper(interpolations).gaussSummaryIntrpl.TMin, intrplHelper(interpolations).gaussSummaryIntrpl.TMax);
					alphaNonParametricTb.Text = normalAlpha.ToDoubString();
					alphaParametricTb.Text = intrplHelper(interpolations).gaussParametricIntrpl.Alpha.ToDoubString();
					alphaSummaryTb.Text = intrplHelper(interpolations).gaussSummaryIntrpl.Alpha.ToDoubString();

					zedGraph.GraphPane.CurveList.Clear();
					List<MethodData> methodData = drawGraphic(testFunction, basisPoints, correctFuncValue, interpolations);
					zoomGraphic();

					// waiting, while user check 'checkBox1'
					while (!checkBox1.Checked && !autoReportChBx.Checked) {
						await Task.Delay(1000);
						if(!standartFunctionMode.Checked)
							return;
						if (double.Parse(alphaNonParametricTb.Text) != intrplHelper(interpolations).gaussIntrpl.Alpha
							|| double.Parse(alphaParametricTb.Text) != intrplHelper(interpolations).gaussParametricIntrpl.Alpha
							|| double.Parse(alphaSummaryTb.Text) != intrplHelper(interpolations).gaussSummaryIntrpl.Alpha) 
						{
							normalAlpha = double.Parse(alphaNonParametricTb.Text);
							interpolations = constructInterpolationList(basisPoints, normalAlpha);
							intrplHelper(interpolations).gaussParametricIntrpl.Alpha = double.Parse(alphaParametricTb.Text);
							intrplHelper(interpolations).gaussSummaryIntrpl.Alpha = double.Parse(alphaSummaryTb.Text);
							zedGraph.GraphPane.CurveList.Clear();
							methodData = drawGraphic(testFunction, basisPoints, correctFuncValue, interpolations);
							zoomGraphic();
						}
					}

					reportData[funcIt] = new ReportData {
						CorrectFuncValue = correctFuncValue,
						TestFunction = testFunction,
						MethodsData = methodData,
						GraphicImage = zedGraph.GraphPane.GetImage()
					};

				} catch (Exception ex) {
					MessageBox.Show(ex.Message, testFunctions[funcIt].Name);
				} finally {
					checkBox1.Checked = false;
				}
			}

			if (exportToWordCB.Checked) {
				await exportToMsWord(reportData);
			}

			autoReportChBx.Checked = false;
			stepGb.Enabled = true;
		}

		private async void button2_Click(object sender, EventArgs e)
		{
			covidSett = covidSett ?? new Covid19Settings();
			if (DialogResult.OK == covidSett.ShowDialog(this)) {
				try {
					PointPairList basisPoints, correctFuncValue;
					(basisPoints, correctFuncValue) = covidSett.GetBasis();

					// create an array from different interpolations types
					double normalAlpha = countAlpha(0, basisPoints.Count - 1, basisPoints.XMin(), basisPoints.XMax());
					InterpolationBase[] interpolations = constructInterpolationList(basisPoints, normalAlpha);

					// setup Alpha for Gaussian methods of interpolation
					intrplHelper(interpolations).gaussParametricIntrpl.Alpha = countAlpha(0, basisPoints.Count - 1, intrplHelper(interpolations).gaussParametricIntrpl.TMin, intrplHelper(interpolations).gaussParametricIntrpl.TMax);
					intrplHelper(interpolations).gaussSummaryIntrpl.Alpha = countAlpha(0, basisPoints.Count - 1, intrplHelper(interpolations).gaussSummaryIntrpl.TMin, intrplHelper(interpolations).gaussSummaryIntrpl.TMax);
					alphaNonParametricTb.Text = normalAlpha.ToDoubString();
					alphaParametricTb.Text = intrplHelper(interpolations).gaussParametricIntrpl.Alpha.ToDoubString();
					alphaSummaryTb.Text = intrplHelper(interpolations).gaussSummaryIntrpl.Alpha.ToDoubString();

					zedGraph.GraphPane.CurveList.Clear();
					_ = drawGraphic(null, basisPoints, correctFuncValue, interpolations);
					xAxis = "--- > Day";
					yAxis = "--- > Number of ill people";
					zoomGraphic();
					while (covid19Mode.Checked) {
						await Task.Delay(1000);
						if (double.Parse(alphaNonParametricTb.Text) != intrplHelper(interpolations).gaussIntrpl.Alpha
							|| double.Parse(alphaParametricTb.Text) != intrplHelper(interpolations).gaussParametricIntrpl.Alpha
							|| double.Parse(alphaSummaryTb.Text) != intrplHelper(interpolations).gaussSummaryIntrpl.Alpha) {
							normalAlpha = double.Parse(alphaNonParametricTb.Text);
							interpolations = constructInterpolationList(basisPoints, normalAlpha);
							intrplHelper(interpolations).gaussParametricIntrpl.Alpha = double.Parse(alphaParametricTb.Text);
							intrplHelper(interpolations).gaussSummaryIntrpl.Alpha = double.Parse(alphaSummaryTb.Text);
							zedGraph.GraphPane.CurveList.Clear();
							_ = drawGraphic(null, basisPoints, correctFuncValue, interpolations);
							zoomGraphic();
						}
					}
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, "COVID19 Error");
				} finally {
					zedGraph.GraphPane.CurveList.Clear();
					zedGraph.Invalidate();
				}
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

		private List<MethodData> drawGraphic(TestFunctionBase testFunction, PointPairList basisPoints, PointPairList correctFuncValue, InterpolationBase[] interpolations)
		{
			List<MethodData> methodDatas = new List<MethodData>();
			GraphPane pane = zedGraph.GraphPane;

			int symbol = 1;
			foreach (InterpolationBase intrplMethod in interpolations) {
				PointPairList interpolatedPoints = doInterpolation(intrplMethod, basisPoints);
				MethodData methodData = new MethodData() {
					Method = intrplMethod,
					InterpolatedFuncValue = interpolatedPoints
				};

				{
					// add interpolation to graphic
					LineItem myCurve = null;
					myCurve = pane.AddCurve($"F(x) = {intrplMethod.Name}", interpolatedPoints, Color.Black, (SymbolType)symbol);
					myCurve.Symbol.Size = 4;
					myCurve.Symbol.Fill = new Fill(Color.FromKnownColor((KnownColor)(10 * symbol++)));
					myCurve.Line.IsVisible = false;
				}

				if (correctFuncValue != null) {
					var (deltaBetweenMethod, methodMark) = countAlgorithmScore(correctFuncValue, interpolatedPoints);
					setScore(methodMark, intrplMethod);
					methodData.DeltaBetweenMethod = deltaBetweenMethod;
					methodData.MethodMark = methodMark;
				}

				methodDatas.Add(methodData);
			}

			{
				// add basis curve to graphic
				title = testFunction == null ? "Covid-19 Statistics" : $"Interpolation for y = F({testFunction.Name})";
				LineItem myCurve;
				myCurve = pane.AddCurve($"y_basis = {(testFunction == null ? "" : testFunction.Name)}", basisPoints, Color.Red, SymbolType.TriangleDown);
				myCurve.Symbol.Size = 7;
				myCurve.Symbol.Fill = new Fill(Color.Red);
				myCurve.Line.IsVisible = false;
				if (correctFuncValue != null) {
					myCurve = pane.AddCurve($"y_basis = {(testFunction == null ? "" : testFunction.Name)}", correctFuncValue, Color.Green, SymbolType.TriangleDown);
					myCurve.Symbol.Size = 3;
					myCurve.Symbol.Fill = new Fill(Color.Green);
					myCurve.Line.IsVisible = false;
				}
			}

			return methodDatas;
		}

		private (double[] deltaBetweenMethod, double methodMark) countAlgorithmScore(PointPairList correctFuncValue, PointPairList interpolatedPoints)
		{
			// count delta between basis and interpolatedPoints
			List<double> deltaBetweenMethod = new List<double>();
			double methodMark = 0;
			int countOfDeltas = 0;

			for (int i = 0; i < correctFuncValue.Count; i++) {
				if (i % 3 == 0) // We don't include basis
					continue;

				double delta = correctFuncValue[i].Y - interpolatedPoints[i].Y;
				delta *= delta; // delta^2
				methodMark += delta;
				deltaBetweenMethod.Add(delta);
				countOfDeltas++;
			}
			methodMark /= countOfDeltas;

			double yMax = correctFuncValue.YMax();
			methodMark /= yMax * yMax; // normalize method mark 

			return (deltaBetweenMethod.ToArray(), methodMark);
		}

		private PointPairList doInterpolation(InterpolationBase intrplMethod, PointPairList basisPoints)
		{
			PointPairList interpolatedPoints = new PointPairList();
			if (intrplMethod is GaussianParametricInterpolation) {
				var parametricIntrpl = intrplMethod as GaussianParametricInterpolation;
				for (int ti = 1; ti < basisPoints.Count; ti++) {
					double prevT = parametricIntrpl.GetT(ti - 1);
					double curT = parametricIntrpl.GetT(ti);
					double delta = (curT - prevT) / (pointsBetweenBasisNumber + 1);

					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= pointsBetweenBasisNumber; pbi++) { // pbi means Point Between Bassis Iter
						interpolatedPoints.Add(parametricIntrpl.GetPoint(prevT + pbi * delta));
					}
				}
				double lastT = parametricIntrpl.GetT(basisPoints.Count - 1);
				PointPair lastPoint = parametricIntrpl.GetPoint(lastT);
				interpolatedPoints.Add(lastPoint);
			} else {
				for (int xi = 1; xi < basisPoints.Count; xi++) {
					double prevX = basisPoints[xi - 1].X;
					double curX = basisPoints[xi].X;
					double delta = (curX - prevX) / (pointsBetweenBasisNumber + 1);
					// here we should put points for comperison distance beetwen each method
					for (int pbi = 0; pbi <= pointsBetweenBasisNumber; pbi++) { // pbi means Point Between Bassis Iter
						interpolatedPoints.Add(intrplMethod.GetPoint(prevX + pbi * delta));
					}
				}
				double lastX = basisPoints[basisPoints.Count - 1].X;
				PointPair lastPoint = intrplMethod.GetPoint(lastX);
				interpolatedPoints.Add(lastPoint);
			}
			return interpolatedPoints;
		}

		private (PointPairList basisPoints, PointPairList correctFuncValue) constructBasisAndCorrectFuncValues(TestFunctionBase testFunction)
		{
			PointPairList basisPoints = new PointPairList();
			PointPairList correctFuncValue = new PointPairList();

			Func<int, double> getStepFunc = null;
			if (stepFixedMode.Checked) {
				if (!double.TryParse(stepTb.Text, out double fixedStep)) {
					fixedStep = 0.2;
					stepTb.Text = fixedStep.ToString();
				}
				getStepFunc = new Func<int, double>((int _) => fixedStep);
			} else {
				getStepFunc = new Func<int, double>((int iter) => testFunction.GetStep(iter));
			}

			// generate basis points and basis points with middle points from func
			int currIter = 0;
			double correctXMax = testFunction.XMax + 0.0001;
			for (double x = testFunction.XMin; x < correctXMax; x += getStepFunc(currIter)) {
				basisPoints.Add(new PointPair(x, testFunction.GetValue(x)));

				if (double.IsNaN(basisPoints.Last().Y) || double.IsInfinity(basisPoints.Last().Y)) {
					throw new ArgumentException($"XMin == {testFunction.XMin} correctXMax == {correctXMax}\n" +
						$"basisPoints.Last() == {basisPoints.Last()}\n" +
						$"double.IsNaN(basisPoints.Last().Y) == {double.IsNaN(basisPoints.Last().Y)}\n" +
						$"double.IsInfinity(basisPoints.Last().Y) == {double.IsInfinity(basisPoints.Last().Y)}", nameof(testFunction));
				}

				PointPair curPoint = basisPoints.Last();
				correctFuncValue.Add(curPoint);

				double nextX = x + getStepFunc(currIter + 1);
				if (nextX < correctXMax) {
					// here we should put middle points
					double delta = (nextX - x) / (pointsBetweenBasisNumber + 1);
					for (int pbi = 1; pbi <= pointsBetweenBasisNumber; pbi++) { // pbi means Point Between Bassis Iter
						double xMiddle = curPoint.X + pbi * delta;
						correctFuncValue.Add(new PointPair(xMiddle, testFunction.GetValue(xMiddle)));
					}
				}
				currIter++;
			}

			return (basisPoints, correctFuncValue);
		}

		private InterpolationBase[] constructInterpolationList(PointPairList basisPoints, double gaussAlpha)
		{
			return new InterpolationBase[] {
						new LagrangeInterpolation(basisPoints),
						new GaussianInterpolation(basisPoints, gaussAlpha),
						new GaussianParametricInterpolation(basisPoints, gaussAlpha, ParametricType.Normal),
						new GaussianParametricInterpolation(basisPoints, gaussAlpha, ParametricType.Summary)
					};
		}

		private void setScore(double methodMark, InterpolationBase intrpl)
		{
			string text = methodMark.ToString("F16");
			if (intrpl is LagrangeInterpolation) {
				scoreLagrageLb.Text = text;

			} else if (intrpl is GaussianParametricInterpolation) {
				var parametricIntrpl = intrpl as GaussianParametricInterpolation;
				if (parametricIntrpl.Type == ParametricType.Normal) {
					scoreParamNormLb.Text = text;
				} else if (parametricIntrpl.Type == ParametricType.Summary) {
					scoreParamSummLb.Text = text;
				}

			} else if (intrpl is GaussianInterpolation) {
				scoreGausLb.Text = text;
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

		/// Установка свойств оси X
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

		/// <summary>
		/// Метод, который вызывается, когда надо отобразить очередную метку по оси
		/// </summary>
		/// <param name="pane">Указатель на текущий GraphPane</param>
		/// <param name="axis">Указатель на ось</param>
		/// <param name="val">Значение, которое надо отобразить</param>
		/// <param name="index">Порядковый номер данного отсчета</param>
		/// <returns>Метод должен вернуть строку, которая будет отображаться под данной меткой</returns>
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

		private async Task exportToMsWord(ReportData[] reportData)
		{
			try {
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Visible = true;
				button1.Enabled = false;
				await startSTATask(() => buildReport(reportData));
			} catch (Exception ex) {
				MessageBox.Show($"Error while generating Report!\n{ex.Message}");
			}
			Cursor.Current = Cursors.Arrow;
			progressBar1.Visible = false;
			button1.Enabled = true;
		}

		void buildReport(ReportData[] data)
		{
			object oMissing = System.Reflection.Missing.Value;
			object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

			//Start Word and create a new document.
			Word._Application oWord = new Word.Application();
			Word._Document oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
			//oDoc.tit = "Gorodetskiy_Interpolation_Report.docx";

			try {
				data.Where(d => d.MethodsData != null).ToList().ForEach(fucData =>
				{
					// Insert title
					Word.Paragraph oPara;
					object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Интерполяция для Y = {fucData.TestFunction.Name}";
					oPara.Range.Bold = 1;
					oPara.Range.Font.Size = 18;
					oPara.Range.InsertParagraphAfter();

					// Insert Interval
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"X є [{fucData.TestFunction.XMin};{fucData.TestFunction.XMax}]. К-во точек = {fucData.MethodsData[0].Method.InputPoints.Count}";
					oPara.Range.Bold = 0;
					oPara.Range.Font.Size = 14;
					oPara.Range.InsertParagraphAfter();

					string stepForEachBasisPoint = string.Empty;
					for (int i = 1; i <= fucData.MethodsData[0].Method.InputPoints.Count; i++) {
						stepForEachBasisPoint += $"{i} = {fucData.TestFunction.GetStep(i):F2} ";
					}
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Шаг = [{stepForEachBasisPoint}]";
					oPara.Range.Bold = 0;
					oPara.Range.Font.Size = 10;
					oPara.Format.SpaceAfter = 6;
					oPara.Range.InsertParagraphAfter();

					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Оценка алгоритма:";
					oPara.Range.Bold = 1;
					oPara.Range.Font.Size = 14;
					oPara.Range.InsertParagraphAfter();

					fucData.MethodsData.ForEach(method =>
					{
						// Insert Score
						oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
						oPara = oDoc.Content.Paragraphs.Add(ref oRng);
						oPara.Range.Text = $"{method.Method.Name}" +
											$"\t\t\t{(method.Method is LagrangeInterpolation ? "\t\t\t" : "")}" +
											$"{method.MethodMark:F16}";
						oPara.Range.Bold = 0;
						oPara.Range.Font.Size = 14;
						oPara.Range.InsertParagraphAfter();
					});

					// Insert Chart Image

					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					Clipboard.Clear();
					Clipboard.SetImage(fucData.GraphicImage);
					oPara.Range.Paste();
					oPara.Range.InsertParagraphAfter();

					// Insert Space
					object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
					object oPageBreak = Word.WdBreakType.wdPageBreak;
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					((Word.Range)oRng).Collapse(ref oCollapseEnd);
					((Word.Range)oRng).InsertBreak(ref oPageBreak);
					((Word.Range)oRng).Collapse(ref oCollapseEnd);
					oPara.Format.SpaceAfter = 0;
					oPara.Range.InsertParagraphAfter();
				});

				oWord.Visible = true;
			} catch {
				oWord.Visible = true;
				throw;
			}
		}

		private Task startSTATask(Action action)
		{
			TaskCompletionSource<object> source = new TaskCompletionSource<object>();
			Thread thread = new Thread(() =>
			{
				try {
					action();
					source.SetResult(null);
				} catch (Exception ex) {
					source.SetException(ex);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return source.Task;
		}

		private void scoreLagrageLb_Click(object sender, EventArgs e)
		{
			Console.WriteLine(scoreLagrageLb.Text);
			Console.WriteLine(scoreGausLb.Text);
			Console.WriteLine(scoreParamNormLb.Text);
			Console.WriteLine(scoreParamSummLb.Text);
		}

		private (GaussianInterpolation gaussIntrpl, GaussianParametricInterpolation gaussParametricIntrpl, GaussianParametricInterpolation gaussSummaryIntrpl)
			intrplHelper(InterpolationBase[] interpolationArray)
			=> ((GaussianInterpolation)interpolationArray[1], (GaussianParametricInterpolation)interpolationArray[2], (GaussianParametricInterpolation)interpolationArray[3]);

	}

	public static class Extension {
		public static string ToDoubString(this double num) => num.ToString("F18");//.TrimEnd(new char[] { '0' });
	}
}

/*
+ 1. Добавить возможность выбора постоянный или шага с прирощением
+ 2. Для Ковида посчитать погрешность
+ 3. Если не работает интернет, базу с файла выбирать
4. Описать возможность выбора данных из интернета в записке
*/